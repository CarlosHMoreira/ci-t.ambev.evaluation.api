using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public record CancelSaleCommand(Guid Id) : IRequest<CancelSaleResult>;

public class CancelSaleResult
{
    public Guid Id { get; set; }
    public bool IsCancelled { get; set; }
}

