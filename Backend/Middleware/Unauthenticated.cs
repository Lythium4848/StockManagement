using Microsoft.IdentityModel.Tokens;

namespace Backend.Middleware;

public class UnauthenticatedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;
        var authHeader = headers.Authorization;

        if (!authHeader.IsNullOrEmpty())
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        await next(context);
    }
}