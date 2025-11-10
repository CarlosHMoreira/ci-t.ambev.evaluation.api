using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Products, int TotalCount)> ListAsync(int page, int size, string? order, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Products, int TotalCount)> ListByCategoryAsync(string category, int page, int size, string? order, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> ListCategoriesAsync(CancellationToken cancellationToken = default);
}