using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartCommand : IRequest<Unit>, IRequest
{
    public Guid Id { get; set; }
}
