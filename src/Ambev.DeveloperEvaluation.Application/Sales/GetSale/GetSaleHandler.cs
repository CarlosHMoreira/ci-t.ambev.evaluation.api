using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler(SaleService service, IMapper mapper) : IRequestHandler<GetSaleCommand, SaleResult?>
{
    public async Task<SaleResult?> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ValidationException("Invalid id");
        }

        var sale = await service.GetByIdAsync(request.Id, cancellationToken);
        
        return sale is null ? null : mapper.Map<SaleResult>(sale);
    }
}

