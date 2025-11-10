using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.ListByCategory;

public class ListByCategoryHandler(IProductRepository repository, IMapper mapper)
    : IRequestHandler<ListByCategoryCommand, ListByCategoryResult>
{
    public async Task<ListByCategoryResult> Handle(ListByCategoryCommand request, CancellationToken cancellationToken)
    {
        var (products, total) = await repository.ListByCategoryAsync(request.Category, request.Page, request.Size, request.Order, cancellationToken);
        return new ListByCategoryResult
        {
            Items = mapper.Map<IEnumerable<ProductResult>>(products),
            TotalCount = total,
            CurrentPage = request.Page,
            PageSize = request.Size,
            Category = request.Category
        };
    }
}

