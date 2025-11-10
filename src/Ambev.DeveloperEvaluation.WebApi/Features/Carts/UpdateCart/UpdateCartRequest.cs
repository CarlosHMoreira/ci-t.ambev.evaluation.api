namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartProductRequest> Products { get; set; } = new();
}

public class UpdateCartProductRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
