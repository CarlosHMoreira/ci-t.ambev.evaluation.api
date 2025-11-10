using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// Handler for processing CreateCartCommand
/// </summary>
public class CreateCartHandler(ICartRepository cartRepository, IMapper mapper)
    : IRequestHandler<CreateCartCommand, CreateCartResult>
{
    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
            

        var cart = mapper.Map<Cart>(command);
        var createdCart = await cartRepository.CreateAsync(cart, cancellationToken);
        return mapper.Map<CreateCartResult>(createdCart);
    }
}

