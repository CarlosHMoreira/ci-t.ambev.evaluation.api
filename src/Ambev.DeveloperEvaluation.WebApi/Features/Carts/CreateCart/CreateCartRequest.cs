namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Request model for creating a cart
/// </summary>
public class CreateCartRequest
{
    /// <summary>
    /// Gets or sets the user ID who owns this cart.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the date when the cart was created.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the list of products in this cart.
    /// </summary>
    public List<CartProductRequest> Products { get; set; } = new();
}

/// <summary>
/// Represents a product in the cart request
/// </summary>
public class CartProductRequest
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }
}

