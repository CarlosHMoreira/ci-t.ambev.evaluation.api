using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler(SaleService service, IMapper mapper) : IRequestHandler<CreateSaleCommand, SaleResult>
{
    public async Task<SaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var sale = mapper.Map<Sale>(request);
        var created = await service.CreateAsync(sale, cancellationToken);
        
        return mapper.Map<SaleResult>(created);
    }
}

