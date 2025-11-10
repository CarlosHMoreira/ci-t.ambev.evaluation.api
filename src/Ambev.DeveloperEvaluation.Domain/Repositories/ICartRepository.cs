using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Cart entity
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Creates a new cart in the database
    /// </summary>
    /// <param name="cart">The cart to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created cart</returns>
    Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a cart by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cart if found, null otherwise</returns>
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cart
    /// </summary>
    /// <param name="cart">The cart to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated cart</returns>
    Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cart from the database
    /// </summary>
    /// <param name="id">The unique identifier of the cart to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the cart was deleted, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists carts with pagination and ordering
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="size">Items per page</param>
    /// <param name="order">Order string e.g. "date desc, userId asc"</param>
    /// <returns>Tuple of carts and total count</returns>
    Task<(IEnumerable<Cart> Items, int TotalCount)> ListAsync(int page, int size, string? order, CancellationToken cancellationToken = default);
}
