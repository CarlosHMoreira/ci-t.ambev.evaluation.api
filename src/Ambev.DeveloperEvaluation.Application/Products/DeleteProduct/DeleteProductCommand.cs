using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>, IRequest
{
    public Guid Id { get; set; }
}

