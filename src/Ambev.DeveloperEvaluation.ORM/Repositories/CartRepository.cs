using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ICartRepository using Entity Framework Core
/// </summary>
public class CartRepository(DefaultContext context) : ICartRepository
{
    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await context.Carts.AddAsync(cart, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        context.Carts.Update(cart);
        await context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(id, cancellationToken);
        if (cart == null)
            return false;

        context.Carts.Remove(cart);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Cart> Items, int TotalCount)> ListAsync(int page,
        int size,
        string? order,
        CancellationToken cancellationToken = default
    )
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 100 : size;

        var query = context.Carts
            .Include(c => c.Products)
            .AsQueryable();

        
        query = OrderBy(order, query).AsNoTracking();

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    private static IQueryable<Cart> OrderBy(string? order, IQueryable<Cart> query)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return query;
        }
        
        IOrderedQueryable<Cart>? orderedQuery = null;
        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var segments = part.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var field = segments[0].ToLowerInvariant();
            var isDesc = segments.Length > 1 && segments[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            
            var selector = OrdeByFieldSelector(field);
            
            if (orderedQuery == null)
            {
                orderedQuery = isDesc ? query.OrderByDescending(selector) : query.OrderBy(selector);
            }
            else
            {
                orderedQuery = isDesc ? orderedQuery.ThenByDescending(selector) : orderedQuery.ThenBy(selector);
            }
        }

        return orderedQuery ?? query;
    }

    private static Expression<Func<Cart, object>> OrdeByFieldSelector(string field) =>
        field switch
        {
            "userid" => c => c.UserId,
            "date" => c => c.Date,
            "id" => c => c.Id,
            _ => c => c.Id
        };
}
