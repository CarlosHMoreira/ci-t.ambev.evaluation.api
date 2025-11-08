using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<CreateProductCommand, Product>()
            .ForMember(d => d.Rating, opt => opt.MapFrom(s => new Rating { Rate = s.Rate, Count = s.Count }));
        CreateMap<Product, CreateProductResult>()
            .ForMember(d => d.Rate, opt => opt.MapFrom(s => s.Rating.Rate))
            .ForMember(d => d.Count, opt => opt.MapFrom(s => s.Rating.Count));
    }
}