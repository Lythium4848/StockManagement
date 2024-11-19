using System;

namespace StockManagement_WinUI3;

public class Product
{
    public int id { get; set; }
    public string name { get; set; }
    public bool status { get; set; }
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
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class UserSession
{
    public string Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}