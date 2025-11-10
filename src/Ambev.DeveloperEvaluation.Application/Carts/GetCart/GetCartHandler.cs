using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartHandler(ICartRepository repository, IMapper mapper)
    : IRequestHandler<GetCartCommand, CartResult>
{
    public async Task<CartResult> Handle(GetCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
            throw new ValidationException($"Cart with id {request.Id} not found");
        return mapper.Map<CartResult>(cart);
    }
}
