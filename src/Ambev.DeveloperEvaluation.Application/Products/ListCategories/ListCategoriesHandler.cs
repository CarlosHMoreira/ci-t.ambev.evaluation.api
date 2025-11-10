using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

public class ListCategoriesHandler(IProductRepository repository)
    : IRequestHandler<ListCategoriesCommand, ListCategoriesResult>
{
    public async Task<ListCategoriesResult> Handle(ListCategoriesCommand request, CancellationToken cancellationToken)
    {
        var categories = await repository.ListCategoriesAsync(cancellationToken);
        return new ListCategoriesResult
        {
            
            Categories = categories
        };
    }
}

