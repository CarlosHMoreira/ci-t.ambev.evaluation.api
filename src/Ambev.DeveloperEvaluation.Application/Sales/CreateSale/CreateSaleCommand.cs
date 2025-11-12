using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateSaleCommand : IRequest<SaleResult>
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<CreateSaleItemDto> Items { get; set; } = new();
}

