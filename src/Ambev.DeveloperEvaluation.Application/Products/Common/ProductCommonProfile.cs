using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.Common;

public class ProductCommonProfile : Profile
{
    public ProductCommonProfile()
    {
        CreateMap<Product, ProductResult>();
    }
}