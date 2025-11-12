namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem
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
    
    public Product? Product { get; set; }

    public void CalculateSaleValue()
    {
        TotalGrossAmount = Quantity * UnitPrice;
        DiscountValue = (TotalGrossAmount * DiscountPercent) / 100m;
        TotalNetAmount = TotalGrossAmount - DiscountValue;
    }
}
