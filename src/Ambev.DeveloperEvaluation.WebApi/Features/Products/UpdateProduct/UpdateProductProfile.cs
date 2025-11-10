using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductProfile: Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductRequest, UpdateProductCommand>()
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(source => source.Rating.Rate))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(source => source.Rating.Count));
        
        CreateMap<ProductResult, ProductResponse>()
            .ForMember(
                d => d.Rating, 
                opt => opt.MapFrom(s => new RatingResponse { Rate = s.Rating.Rate, Count = s.Rating.Count })
            );
    }
}