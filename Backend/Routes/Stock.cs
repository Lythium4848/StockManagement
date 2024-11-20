using Microsoft.EntityFrameworkCore;

namespace Backend.Routes;

public class StockHandler
{
    public static async Task<IResult> GetAll(DatabaseContext db)
    {
        var stocks = await db.Stocks.ToListAsync();
        return Results.Ok(stocks);
    }

    public static async Task<IResult> Get(DatabaseContext db, string id)
    {
        if (!int.TryParse(id, out var stockId))
        {
            return Results.BadRequest("Invalid stock ID");
        }

        var stock = await db.Stocks.FindAsync(stockId);
        return stock == null ? Results.NotFound() : Results.Ok(stock);
    }

    public static async Task<IResult> Create(DatabaseContext db, Stock stock)
    {
        await db.Stocks.AddAsync(stock);
        await db.SaveChangesAsync();
        return Results.Created($"/api/stock/{stock.Id}", stock);
    }

    public static async Task<IResult> Update(DatabaseContext db, string id, Stock stock)
    {
        if (!int.TryParse(id, out var stockId))
        {
            return Results.BadRequest("Invalid stock ID");
        }

        var existingStock = await db.Stocks.FindAsync(stockId);
        if (existingStock == null)
        {
            return Results.NotFound();
        }

        existingStock.Name = stock.Name;
        existingStock.Quantity = stock.Quantity;
        existingStock.TransactionDate = stock.TransactionDate;
        existingStock.Status = stock.Status;

        await db.SaveChangesAsync();

        return Results.Ok(existingStock);
    }

    public static async Task<IResult> Delete(DatabaseContext db, string id)
    {
        if (!int.TryParse(id, out var stockId))
        {
            return Results.BadRequest("Invalid stock ID");
        }

        var stock = await db.Stocks.FindAsync(stockId);
        if (stock == null)
        {
            return Results.NotFound();
        }

        db.Stocks.Remove(stock);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}