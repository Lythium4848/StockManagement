using System;

namespace StockManagement_WinUI3;

public class Product
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required bool Status { get; init; }
}

public class Stock
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTime TransactionDate { get; init; }
    public required int Quantity { get; init; }
    public required bool Status { get; init; }
}

public class User
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required DateTime LastLoginAt { get; init; }
}

public class UserSession
{
    public required string Id { get; init; }
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}