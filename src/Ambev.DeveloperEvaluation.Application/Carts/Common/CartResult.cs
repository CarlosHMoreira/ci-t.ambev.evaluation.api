namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

public class CartResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductDto> Products { get; set; } = new();
    
    /// <summary>
    /// Represents a product in the cart.
    /// </summary>
    public class CartProductDto
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
}


