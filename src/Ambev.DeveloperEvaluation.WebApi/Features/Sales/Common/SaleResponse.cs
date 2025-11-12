namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

public class SaleItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal TotalGrossAmount { get; set; }
    public decimal TotalNetAmount { get; set; }
    public bool IsCancelled { get; set; }
}

public class SaleResponse
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public DateTime Date { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public decimal TotalAmount { get; set; }
    public IEnumerable<SaleItemResponse> Items { get; set; } = Enumerable.Empty<SaleItemResponse>();
}

