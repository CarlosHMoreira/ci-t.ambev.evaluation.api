using Ambev.DeveloperEvaluation.Application.Carts.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommand : IRequest<CartResult>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductUpdateDto> Products { get; set; } = new();
}

public class CartProductUpdateDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
