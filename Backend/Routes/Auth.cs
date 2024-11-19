using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Visus.Cuid;

namespace Backend.Routes;

public class LoginBody
{
    public string username { get; set; }
    public string password { get; set; }
}

public class UserReturn
{
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SessionReturn
{
    public string Id { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class LoginReturn
{
    public UserReturn User { get; set; }
    public SessionReturn Session { get; set; }
}

public partial class Auth
{
    public static async Task<IResult> Login(DatabaseContext db, LoginBody body)
    {
        var usernamePassRegex = UsernameRegex().IsMatch(body.username);

        if (!usernamePassRegex)
        {
            return Results.BadRequest("Username must only contain letters A-z, 0-9, _, & .");
        }

        var user = await db.Users
            .Where(user => user.Username == body.username)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            Console.WriteLine("no user");
            return Results.Unauthorized();
        }

        var passwordBytes = Encoding.UTF8.GetBytes(body.password);
        var passwordHash = SHA512.HashData(passwordBytes);
        var passwordHexEncoding = Convert.ToHexStringLower(passwordHash);

        Console.WriteLine(passwordHexEncoding);

        if (user.Password != passwordHexEncoding)
        {
            Console.WriteLine("bad pass");
            return Results.Unauthorized();
        }

        var time = DateTime.UtcNow;

        user.LastLoginAt = time;

        await db.SaveChangesAsync();

        var sessionToken = new Cuid2().ToString();
        var sessionTokenBytes = Encoding.UTF8.GetBytes(sessionToken);
        var sessionTokenHash = SHA512.HashData(sessionTokenBytes);
        var hexEncoding = Convert.ToHexStringLower(sessionTokenHash);

        var sessionData = new UserSessions
        {
            Id = new Cuid2().ToString(),
            Token = hexEncoding,
            UserId = user.Id,
            ExpiresAt = time.AddDays(14),
            CreatedAt = time
        };

        await db.UserSessions.AddAsync(sessionData);

        await db.SaveChangesAsync();

        var returnSessionData = new LoginReturn
        {
            User = new UserReturn
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            },
            Session = new SessionReturn
            {
                Id = sessionData.Id,
                Token = sessionToken,
                ExpiresAt = sessionData.ExpiresAt,
                CreatedAt = sessionData.CreatedAt,
            }
        };

        return Results.Ok(returnSessionData);
    }

    public static async Task<IResult> Register(DatabaseContext db, LoginBody body)
    {
        var usernamePassRegex = UsernameRegex().IsMatch(body.username);

        if (!usernamePassRegex)
        {
            return Results.BadRequest("Username must only contain letters A-z, 0-9, _, & .");
        }

        var userExists = await db.Users
            .Where(User => User.Username == body.username)
            .FirstOrDefaultAsync();

        Console.WriteLine(userExists);

        if (userExists != null)
        {
            return Results.BadRequest("Username already in use.");
        }

        var passwordBytes = Encoding.UTF8.GetBytes(body.password);
        var passwordHash = SHA512.HashData(passwordBytes);
        var passwordHexEncoding = Convert.ToHexStringLower(passwordHash);

        Console.WriteLine(passwordHexEncoding);

        var time = DateTime.UtcNow;

        var userData = new User
        {
            Id = new Cuid2().ToString(),
            Username = body.username,
            Password = passwordHexEncoding,
            CreatedAt = time,
            UpdatedAt = time,
            LastLoginAt = time
        };

        await db.Users.AddAsync(userData);

        await db.SaveChangesAsync();

        var sessionToken = new Cuid2().ToString();
        var sessionTokenBytes = Encoding.UTF8.GetBytes(sessionToken);
        var sessionTokenHash = SHA512.HashData(sessionTokenBytes);
        var hexEncoding = Convert.ToHexStringLower(sessionTokenHash);

        var sessionData = new UserSessions
        {
            Id = new Cuid2().ToString(),
            Token = hexEncoding,
            UserId = userData.Id,
            ExpiresAt = time.AddDays(14),
            CreatedAt = time
        };

        await db.UserSessions.AddAsync(sessionData);

        await db.SaveChangesAsync();

        var returnSessionData = new LoginReturn
        {
            User = new UserReturn
            {
                Id = userData.Id,
                Username = userData.Username,
                CreatedAt = userData.CreatedAt,
                UpdatedAt = userData.UpdatedAt
            },
            Session = new SessionReturn
            {
                Id = sessionData.Id,
                Token = sessionToken,
                ExpiresAt = sessionData.ExpiresAt,
                CreatedAt = sessionData.CreatedAt,
            }
        };

        return Results.Ok(returnSessionData);
    }

    public static async Task<IResult> Logout(HttpContext context, DatabaseContext db)
    {
        if (context.Items["Session"] is not UserSessions session)
        {
            return Results.BadRequest("Invalid session");
        }

        db.UserSessions.Remove(session);

        await db.SaveChangesAsync();

        return Results.Ok();
    }


    [GeneratedRegex("^[A-Za-z0-9_.]+$")]
    private static partial Regex UsernameRegex();
}