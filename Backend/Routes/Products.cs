using Microsoft.EntityFrameworkCore;

namespace Backend.Routes;

public static class Products
{
    public static async Task<IResult> GetAll(DatabaseContext db)
    {
        var products = await db.Products.ToListAsync();
        return Results.Ok(products);
    }

    public static async Task<IResult> Get(DatabaseContext db, string id)
    {
        if (!int.TryParse(id, out var productId))
        {
            return Results.BadRequest("Invalid product ID");
        }

        var product = await db.Products.FindAsync(productId);
        return product == null ? Results.NotFound() : Results.Ok(product);
    }

    public static async Task<IResult> Create(DatabaseContext db, Product product)
    {
        await db.Products.AddAsync(product);
        await db.SaveChangesAsync();
        return Results.Created($"/api/products/{product.Id}", product);
    }

    public static async Task<IResult> Update(DatabaseContext db, string id, Product product)
    {
        if (!int.TryParse(id, out var productId))
        {
            return Results.BadRequest("Invalid product ID");
        }

        var existingProduct = await db.Products
            .Where(p => p.Id == productId)
            .FirstOrDefaultAsync();
        if (existingProduct == null)
        {
            return Results.NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Status = product.Status;

        await db.SaveChangesAsync();

        return Results.Ok(existingProduct);
    }

    public static async Task<IResult> Delete(DatabaseContext db, string id)
    {
        if (!int.TryParse(id, out var productId))
        {
            return Results.BadRequest("Invalid product ID");
        }

        var product = await db.Products
            .Where(p => p.Id == productId)
            .FirstOrDefaultAsync();
        if (product == null)
        {
            return Results.NotFound();
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}