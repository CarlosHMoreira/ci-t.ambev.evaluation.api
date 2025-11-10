using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler(ICartRepository repository, IMapper mapper)
    : IRequestHandler<UpdateCartCommand, CartResult>
{
    public async Task<CartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartValidator();
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing == null)
        {
            throw new ValidationException($"Cart with id {command.Id} not found");
        }
        
        var cart = mapper.Map<Cart>(command);
        var updated = await repository.UpdateAsync(cart, cancellationToken);
        return mapper.Map<CartResult>(updated);
    }
}
