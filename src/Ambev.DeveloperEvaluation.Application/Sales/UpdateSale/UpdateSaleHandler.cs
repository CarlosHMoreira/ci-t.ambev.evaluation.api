using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler(SaleService service, IMapper mapper) : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        var existing = await service.GetByIdAsync(command.Id, cancellationToken) 
                       ?? throw new KeyNotFoundException("Sale not found");
        
        mapper.Map(command, existing);
        var updated = await service.UpdateAsync(existing, cancellationToken);
        return mapper.Map<SaleResult>(updated);
    }
}

