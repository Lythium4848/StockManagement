using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSessions> UserSessions { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Status { get; set; }
}

public class Stock
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime TransactionDate { get; set; }
    public int Quantity { get; set; }
    public bool Status { get; set; }
}

public class User
{
    [Key]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class UserSessions
{
    [Key]
    public string Id { get; set; }
    public string Token { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
}