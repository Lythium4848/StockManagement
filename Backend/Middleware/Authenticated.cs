using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Middleware;

public class AuthenticatedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;
        var authHeader = headers.Authorization;

        if (authHeader.IsNullOrEmpty())
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var authHeaderStr = authHeader.ToString();

        var tokenParts = authHeaderStr.Split(".");
        if (tokenParts.Length != 2)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var sessionId = tokenParts[0];
        var sessionToken = tokenParts[1];

        Console.WriteLine(sessionId);
        Console.WriteLine(sessionToken);

        var db = context.RequestServices.GetService<DatabaseContext>();

        var session = await db!.UserSessions
            .Where(session => session.Id == sessionId)
            .Include(userSessions => userSessions.User)
            .FirstOrDefaultAsync();

        if (session == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var sessionTokenBytes = Encoding.UTF8.GetBytes(sessionToken);
        var sessionTokenHash = SHA512.HashData(sessionTokenBytes);
        var hexEncoding = Convert.ToHexStringLower(sessionTokenHash);
        Console.WriteLine(hexEncoding);
        Console.WriteLine(session.Token);

        if (hexEncoding != session.Token)
        {
            Console.WriteLine("bad token");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        context.Items["Session"] = session;
        context.Items["User"] = session.User;

        await next(context);
    }
}