namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public sealed class SaleModifiedEvent(Entities.Sale sale) : DomainEvent
{
    public Guid SaleId { get; } = sale.Id;
    public string Number { get; } = sale.Number;
    public decimal Total { get; } = sale.TotalAmount;
}

