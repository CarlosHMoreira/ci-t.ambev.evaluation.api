namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public sealed class SaleItemCancelledEvent(Guid saleId, Guid productId) : DomainEvent
{
    public Guid SaleId { get; } = saleId;
    public Guid ProductId { get; } = productId;
}

