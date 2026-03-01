namespace SamplePaymentsForOrders.Common;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppLogicException ex)
        {
            context.Response.StatusCode = (int)ex.Status;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
        }
    }
}