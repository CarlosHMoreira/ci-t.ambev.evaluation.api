using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class SaleService(
    ISaleRepository repository, 
    IProductRepository productRepository,
    IUserRepository userRepository,
    IDomainEventDispatcher dispatcher
    )
{
    private readonly MaxQuantitySpecification _maxQuantitySpec = new();
    private readonly DiscountElegibilityTenPercentSpecification _discountElegibilityTenPercentSpec = new();
    private readonly DiscountElegibilityTwentyPercentSpecification _discountElegibilityTwentyPercentSpec = new();


    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await VerifyAndCalculateSaleTotals(sale, cancellationToken);

        sale = await repository.CreateAsync(sale, cancellationToken);
        await dispatcher.DispatchAsync(new SaleCreatedEvent(sale), cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => 
        await repository.GetByIdAsync(id, cancellationToken);

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await VerifyAndCalculateSaleTotals(sale, cancellationToken);
        
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
    
    private async Task VerifyAndCalculateSaleTotals(Sale sale, CancellationToken cancellationToken)
    {
        VerifySaleItemsSpecifications(sale.Items);
        await CalculateItemsValues(sale, cancellationToken);
        await EnrichCustomerData(sale, cancellationToken);
        sale.RecalculateTotals();
    }

    private async Task EnrichCustomerData(Sale sale, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(sale.CustomerId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with id {sale.CustomerId} not found");
        }
        
        if (user.Status != Enums.UserStatus.Active)
        {
            throw new InvalidOperationException($"User with id {sale.CustomerId} is not active. Current status: {user.Status}");
        }
        
        sale.CustomerName = user.Name.ToString();
    }

    private async Task CalculateItemsValues(Sale sale, CancellationToken cancellationToken)
    {
        var products = await productRepository
            .GetProductsByIdsAsync(sale.Items.Select(i => i.ProductId).ToArray(), cancellationToken);

        foreach (var item in sale.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with id {item.ProductId} not found");
            }
            
            if (product.Price < 0)
            {
                throw new InvalidOperationException($"Product with id {item.ProductId} has invalid price: {product.Price}");
            }
            
            item.UnitPrice = product.Price;
            item.ProductTitle = product.Title;
            item.CalculateSaleValue();
        }
    }

    private void VerifySaleItemsSpecifications(List<SaleItem> items)
    {
        foreach (var item in items)
        {
            if (!_maxQuantitySpec.IsSatisfiedBy(item))
            {
                throw new InvalidOperationException($"Quantity above limit for product {item.ProductId}");
            }
            
            item.DiscountPercent = ResolveDiscountPercent(item);
        }
    }
    private decimal ResolveDiscountPercent(SaleItem item)
    {
        if (_discountElegibilityTenPercentSpec.IsSatisfiedBy(item))
        {
            return 10m;
        }

        if (_discountElegibilityTwentyPercentSpec.IsSatisfiedBy(item))
        {
            return 20m;
        }

        return 0m;
    }
}

