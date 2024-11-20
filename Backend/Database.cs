using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Backend;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// fuck the warnings, i dont care

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; init; }
    public DbSet<Stock> Stocks { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<UserSessions> UserSessions { get; init; }
}

public class Product
{
    public int Id { get; init; }
    public string Name { get; set; }
    public bool Status { get; set; }
}

public class Stock
{
    public int Id { get; init; }
    public string Name { get; set; }
    public DateTime TransactionDate { get; set; }
    public int Quantity { get; set; }
    public bool Status { get; set; }
}

public class User
{
    [Key]
    public string Id { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime LastLoginAt { get; set; }
}

public class UserSessions
{
    [Key]
    public string Id { get; init; }
    public string Token { get; init; }
    [ForeignKey("User")]
    public string UserId { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; }

    public User User { get; init; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
