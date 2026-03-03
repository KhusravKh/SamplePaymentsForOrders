# SamplePaymentsForOrders

A REST API service for processing payments against orders, built with ASP.NET Core, PostgreSQL, and Redis.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core |
| Database | PostgreSQL (via Entity Framework Core + Npgsql) |
| Auth | JWT Bearer |
| Distributed Lock | Redis |

---

## Architecture & Technical Decisions
## Service Oriented Architecture (SOA) is used to structure the application into distinct services:

Every service has a clear responsibility and interacts with others through well-defined interfaces. This promotes separation of concerns, maintainability, and scalability.

DTOs are used for all external API contracts, while internal domain models are kept separate. This allows for flexibility in changing internal implementations without affecting API consumers.

For the future we can use AutoMapper to reduce boilerplate code for mapping between domain models and DTOs, but for this sample, manual mapping is implemented for clarity.

### Option Pattern is used for configuration.

Allowing for strongly-typed access to settings throughout the application. This promotes type safety and makes it easier to manage configuration values.

For the future we can use secret management solutions (like Azure Key Vault or AWS Secrets Manager) for sensitive configuration values (e.g., JWT signing keys, database connection strings), but for this sample, they are stored in `appsettings.json` for simplicity.

### Database: PostgreSQL

PostgreSQL was chosen for its strong ACID transaction support, which is critical for atomically updating payment and order statuses. The `FOR UPDATE` row-level locking primitive is used directly in key payment flows to prevent race conditions.

### Entity Framework Core is used as the ORM.

For database access, with Npgsql as the provider.

This allows for efficient querying and transaction management while still enabling raw SQL when necessary for locking.

### Atomic Status Transitions

When a payment is confirmed as successful, both the payment status (`Pending → Successful`) and the order status (`Created → Paid`) are updated within the **same database transaction**. This guarantees they are always consistent — there is no state where a payment is `Successful` but the order is still `Created`, or vice versa.

---

### External Payment Provider Resilience

The payment provider is abstracted behind `IMockPaymentProviderService`. The real integration point is in `ConfirmPaymentAsync`, which calls the external provider and handles both success and failure responses gracefully — a failed provider response sets the payment to `Failed` without crashing the system.

---

### Rate Limiting

Three rate limiting policies are configured using ASP.NET Core's built-in `RateLimiter`:

| Policy | Applied To | Limit |
|---|---|---|
| `fixed` | Global (default) | 60 req/min, queue 10 |
| `user-id` | Authenticated endpoints (Orders, Payments) | 30 req/min per user, queue 5 |
| `user-ip` | Registration endpoints | 30 req/min per IP, queue 5 |

- **Order and Payment controllers** use `user-id` policy — limits are enforced per authenticated user, preventing a single account from requesting a lot the system.
- **Registration controller** uses `user-ip` policy — since users are not authenticated at this stage, limiting is done by IP address.
- Exceeding limits returns `429 Too Many Requests`.

---

### Authentication & Brute-Force Protection

JWT tokens are issued on login with configurable expiry (default: 15 minutes). Token validation enforces issuer, audience, lifetime, and signing key.

To protect against brute-force login attacks, failed login attempts are tracked per user:

- After **5 failed attempts**, the account is locked for **30 minutes**.
- On successful login, the attempt counter resets.

And also there we should implement refresh tokens in production, but for simplicity, this sample focuses on access tokens only.

---

### OTP-Based Registration

User registration is a two-step process:

1. **Send OTP** — a 6-digit code is generated and (in production) sent to the user's phone number. Sending is protected by a **Redis distributed lock** to prevent concurrent OTP sends for the same phone number.
2. **Register** — the user submits the OTP along with their details. The OTP record is locked with `SELECT ... FOR UPDATE` during validation to prevent race conditions on the attempt counter.

OTP codes expire after 5 minutes. A phone number is limited to **3 OTP requests per 30 minutes**. After **5 failed attempts**, the OTP is locked for 30 minutes.

---

### Error Handling

All application errors are handled by `ExceptionHandlerMiddleware`. Business logic throws `AppLogicException` with a typed `ExceptionStatus` (400, 404, 409), which is translated into the appropriate HTTP response:

```json
{ "message": "Error description" }
```

Unhandled exceptions return a generic `500` response, avoiding internal detail leakage.

---

## API Endpoints

### Auth
| Method | Route | Description |
|---|---|---|
| POST | `/Auth/login` | Login and receive JWT token |

### Registration
| Method | Route | Description |
|---|---|---|
| POST | `/Register` | Register a new user (requires valid OTP) |
| POST | `/Register/otp` | Send OTP to phone number |
| POST | `/Register/resend-otp` | Resend OTP (after 1 minute cooldown) |

### Orders *(requires JWT)*
| Method | Route | Description |
|---|---|---|
| POST | `/Orders` | Create a new order |
| GET | `/Orders/{orderId}` | Get order by ID |
| GET | `/Orders` | Get all orders for current user |

### Payments *(requires JWT)*
| Method | Route | Description |
|---|---|---|
| POST | `/Payments` | Initiate a payment for an order |
| POST | `/Payments/confirm/{paymentId}` | Confirm a pending payment |
| GET | `/Payments/order/{orderId}` | Get all payments for an order |

---
