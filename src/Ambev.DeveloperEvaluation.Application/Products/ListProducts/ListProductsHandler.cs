using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsHandler(IProductRepository repository, IMapper mapper)
    : IRequestHandler<ListProductsCommand, ListProductsResult>
{
    public async Task<ListProductsResult> Handle(ListProductsCommand request, CancellationToken cancellationToken)
    {
        var (products, total) = await repository.ListAsync(request.Page, request.Size, request.Order, cancellationToken);
        return new ListProductsResult
        {
            Items = mapper.Map<IEnumerable<ProductResult>>(products),
            TotalCount = total,
            CurrentPage = request.Page,
            PageSize = request.Size,
        };
    }
}

