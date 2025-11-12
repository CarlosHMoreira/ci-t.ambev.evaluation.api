using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class SaleService(ISaleRepository repository, IDomainEventDispatcher dispatcher)
{
    private readonly MaxQuantitySpecification _maxQuantitySpec = new();

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            if (!_maxQuantitySpec.IsSatisfiedBy(item))
                throw new InvalidOperationException($"Quantity above limit for product {item.ProductId}");
            item.ApplyRules();
        }
        sale.RecalculateTotals();
        sale = await repository.CreateAsync(sale, cancellationToken);
        await dispatcher.DispatchAsync(new SaleCreatedEvent(sale), cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => 
        await repository.GetByIdAsync(id, cancellationToken);

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            if (!_maxQuantitySpec.IsSatisfiedBy(item))
                throw new InvalidOperationException($"Quantity above limit for product {item.ProductId}");
            item.ApplyRules();
        }
        sale.RecalculateTotals();
        sale = await repository.UpdateAsync(sale, cancellationToken);
        await dispatcher.DispatchAsync(new SaleModifiedEvent(sale), cancellationToken);
        return sale;
    }

    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await repository.GetByIdAsync(id, cancellationToken);
        if (sale == null) return false;
        if (sale.IsCancelled) return true;
        sale.IsCancelled = true;
        sale.RecalculateTotals();
        sale = await repository.UpdateAsync(sale, cancellationToken);
        await dispatcher.DispatchAsync(new SaleCancelledEvent(sale), cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> ListAsync(
        int page,
        int size,
        string? order,
        Dictionary<string, string> filters,
        CancellationToken cancellationToken = default
    ) => await repository.ListAsync(page, size, order, filters, cancellationToken);
}

