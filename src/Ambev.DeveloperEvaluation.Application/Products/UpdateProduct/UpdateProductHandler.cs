using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductHandler(IProductRepository repository, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductValidator();
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing == null)
        {
            throw new ValidationException($"Product with id {command.Id} not found");
        }

        var product = mapper.Map(command, existing);

        await repository.UpdateAsync(product, cancellationToken);
        return mapper.Map<ProductResult>(existing);
    }
}

