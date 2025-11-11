using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.ListCriteriaFilter;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository(DefaultContext context) : IProductRepository
{
    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        context.Products.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> ListAsync(int page, int size, string? order, Dictionary<string, string> filters, CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsQueryable();
        
        query = CriteriaFilters
            .FromDictionary(filters)
            .WhenMinNumeric<Product>(
                forProperty: p => p.Price,
                applyFilter: (queryable, minPrice) => queryable.Where(p => p.Price >= minPrice)
            )
            .WhenMaxNumeric<Product>(
                forProperty: p => p.Price,
                applyFilter: (queryable, maxPrice) => queryable.Where(p => p.Price <= maxPrice)
            )
            .WhenSearchingTerm<Product>(
                forProperty: p => p.Title,
                applyFilter: (queryable, term, match) => match switch
                {
                    SearchPatternMatch.Exact => queryable.Where(p => p.Title == term),
                    SearchPatternMatch.StartsWith => queryable.Where(p => p.Title.StartsWith(term)),
                    SearchPatternMatch.EndsWith => queryable.Where(p => p.Title.EndsWith(term)),
                    SearchPatternMatch.Contains => queryable.Where(p => p.Title.Contains(term)),
                    _ => queryable,
                }
            )
            .WhenSearchingTerm<Product>(
                forProperty: p => p.Category,
                applyFilter: (queryable, term, match) => match switch
                {
                    SearchPatternMatch.Exact => queryable.Where(p => p.Category == term),
                    SearchPatternMatch.StartsWith => queryable.Where(p => p.Category.StartsWith(term)),
                    SearchPatternMatch.EndsWith => queryable.Where(p => p.Category.EndsWith(term)),
                    SearchPatternMatch.Contains => queryable.Where(p => p.Category.Contains(term)),
                    _ => queryable,
                }
            )
            .Apply(query);
        
        return await PaginatedQuery(query, page, size, order, cancellationToken);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> ListByCategoryAsync(string category, int page, int size, string? order, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Where(p => p.Category == category);
        return await PaginatedQuery(query, page, size, order, cancellationToken);
    }

    public async Task<IEnumerable<string>> ListCategoriesAsync(CancellationToken cancellationToken = default) => 
        await context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(category => category)
            .ToListAsync(cancellationToken);

    private static async Task<(IEnumerable<Product> Products, int TotalCount)> PaginatedQuery(
        IQueryable<Product> query,
        int page,
        int size,
        string? order,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;

        query = ApplyOrdering(query, order).AsNoTracking(); 
        
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> source, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return source;
        }

        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var segments = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                continue;
            }

            var field = segments[0].ToLowerInvariant();
            var isDesc = segments.Length > 1 && segments[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            source = field switch
            {
                "title" => isDesc ? source.OrderByDescending(p => p.Title) : source.OrderBy(p => p.Title),
                "price" => isDesc ? source.OrderByDescending(p => p.Price) : source.OrderBy(p => p.Price),
                "category" => isDesc ? source.OrderByDescending(p => p.Category) : source.OrderBy(p => p.Category),
                _ => source
            };
        }
        return source;
    }
}