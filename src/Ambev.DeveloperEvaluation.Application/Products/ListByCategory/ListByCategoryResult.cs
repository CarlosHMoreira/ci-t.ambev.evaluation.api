using Ambev.DeveloperEvaluation.Application.Products.Common;

namespace Ambev.DeveloperEvaluation.Application.Products.ListByCategory;

public class ListByCategoryResult
{
    public IEnumerable<ProductResult> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string Category { get; set; } = string.Empty;
}

