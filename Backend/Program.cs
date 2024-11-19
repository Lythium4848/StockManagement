using Backend;
using Backend.Middleware;
using Backend.Routes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => Results.Ok("Login"));

app.MapGet("/api/products", Products.GetAll).UseAuthenticatedMiddleware();
app.MapGet("/api/products/{id}", Products.Get).UseAuthenticatedMiddleware();
app.MapPatch("/api/products/{id}", Products.Update).UseAuthenticatedMiddleware();
app.MapDelete("/api/products/{id}", Products.Delete).UseAuthenticatedMiddleware();
app.MapPost("/api/products", Products.Create).UseAuthenticatedMiddleware();

app.MapGet("/api/stock", StockHandler.GetAll).UseAuthenticatedMiddleware();
app.MapGet("/api/stock/{id}", StockHandler.Get).UseAuthenticatedMiddleware();
app.MapPatch("/api/stock/{id}", StockHandler.Update).UseAuthenticatedMiddleware();
app.MapDelete("/api/stock/{id}", StockHandler.Delete).UseAuthenticatedMiddleware();
app.MapPost("/api/stock", StockHandler.Create).UseAuthenticatedMiddleware();

app.MapPost("/api/auth/login", Auth.Login).UseUnauthenticatedMiddleware();
app.MapPost("/api/auth/register", Auth.Register).UseUnauthenticatedMiddleware();
app.MapPost("/api/auth/logout", Auth.Logout).UseAuthenticatedMiddleware();

app.Run();
