using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects; // adiciona Rating

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductCommand, Product>()
            .ForMember(
                dest => dest.Rating, 
                opt => opt.MapFrom(src => new Rating { Rate = src.Rate, Count = src.Count })
            );
        
    }
}
