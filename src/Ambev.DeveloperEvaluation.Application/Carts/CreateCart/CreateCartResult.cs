using Ambev.DeveloperEvaluation.Application.Carts.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// Response model for CreateCart operation
/// </summary>
public class CreateCartResult
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public Guid Id { get; set; }

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
    public List<CartResult.CartProductDto> Products { get; set; } = new();
}

