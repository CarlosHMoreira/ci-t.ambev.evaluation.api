using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductHandler(IProductRepository repository, IMapper mapper)
    : IRequestHandler<GetProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(GetProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new ValidationException($"Product with id {request.Id} not found");
        }
            
        return mapper.Map<ProductResult>(product);
    }
}

