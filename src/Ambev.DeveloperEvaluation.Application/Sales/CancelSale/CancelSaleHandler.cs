using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler(SaleService service) : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) throw new ValidationException("Invalid id");
        var success = await service.CancelAsync(request.Id, cancellationToken);
        if (!success) throw new KeyNotFoundException("Sale not found");
        return new CancelSaleResult { Id = request.Id, IsCancelled = true };
    }
}

