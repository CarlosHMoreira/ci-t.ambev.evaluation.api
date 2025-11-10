using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<CreateProductRequest, CreateProductCommand>()
            .ForMember(d => d.Rate, opt => opt.MapFrom(s => s.Rating.Rate))
            .ForMember(d => d.Count, opt => opt.MapFrom(s => s.Rating.Count));
        CreateMap<CreateProductResult, ProductResponse>()
            .ForMember(d => d.Rating, opt => opt.MapFrom(s => new RatingResponse { Rate = s.Rate, Count = s.Count }));
    }
}
