using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListByCategory;

public class ListByCategoryCommand : IRequest<ListByCategoryResult>
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
}

