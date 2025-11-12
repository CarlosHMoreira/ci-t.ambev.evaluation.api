using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Ambev.DeveloperEvaluation.ORM.ListCriteriaFilter;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository(DefaultContext context) : ISaleRepository
{
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await context.Sales.AddAsync(sale, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        context.Sales.Update(sale);
        await context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        context.Sales.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> ListAsync(
        int page,
        int size,
        string? order,
        Dictionary<string, string> filters,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Sales.Include(s => s.Items).AsQueryable();

        query = CriteriaFilters
            .FromDictionary(filters)
            .WhenMaxNumeric<Sale>(
                forProperty: s => s.Number,
                applyFilter: (q, max) => q.Where(s => s.Number <= max)
            )
            .WhenSearchingTerm<Sale>(s => s.BranchName, (q, term, match) => match switch
            {
                SearchPatternMatch.Exact => q.Where(s => s.BranchName == term),
                SearchPatternMatch.StartsWith => q.Where(s => s.BranchName.StartsWith(term)),
                SearchPatternMatch.EndsWith => q.Where(s => s.BranchName.EndsWith(term)),
                SearchPatternMatch.Contains => q.Where(s => s.BranchName.Contains(term)),
                _ => q
            })
            .WhenSearchingTerm<Sale>(s => s.CustomerName, (q, term, match) => match switch
            {
                SearchPatternMatch.Exact => q.Where(s => s.CustomerName == term),
                SearchPatternMatch.StartsWith => q.Where(s => s.CustomerName.StartsWith(term)),
                SearchPatternMatch.EndsWith => q.Where(s => s.CustomerName.EndsWith(term)),
                SearchPatternMatch.Contains => q.Where(s => s.CustomerName.Contains(term)),
                _ => q
            })
            .WhenMinNumeric<Sale>(s => s.TotalAmount, (q, min) => q.Where(s => s.TotalAmount >= min))
            .WhenMaxNumeric<Sale>(s => s.TotalAmount, (q, max) => q.Where(s => s.TotalAmount <= max))
            .Apply(query);


        query = ApplyOrdering(query, order);

        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> source, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return source.OrderByDescending(s => s.Date);
        }
        
        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var segments = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = segments[0].ToLowerInvariant();
            var isDesc = segments.Length > 1 && segments[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            source = field switch
            {
                "number" => isDesc ? source.OrderByDescending(s => s.Number) : source.OrderBy(s => s.Number),
                "date" => isDesc ? source.OrderByDescending(s => s.Date) : source.OrderBy(s => s.Date),
                "total" => isDesc ? source.OrderByDescending(s => s.TotalAmount) : source.OrderBy(s => s.TotalAmount),
                _ => source
            };
        }
        return source;
    }
}
