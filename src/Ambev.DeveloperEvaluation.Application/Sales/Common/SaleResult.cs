namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

public class SaleResult
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public decimal TotalAmount { get; set; }
    public IEnumerable<SaleItemResult> Items { get; set; } = [];
}

