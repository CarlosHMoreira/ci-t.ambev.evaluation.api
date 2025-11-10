using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// Profile for mapping CreateCart related objects
/// </summary>
public class CreateCartProfile : Profile
{
    public CreateCartProfile()
    {
        CreateMap<CreateCartCommand, Cart>();
        CreateMap<CartResult.CartProductDto, CartItem>();
        CreateMap<Cart, CreateCartResult>();
        CreateMap<CartItem, CartResult.CartProductDto>();
        CreateMap<Cart, CartResult>();
    }
}
