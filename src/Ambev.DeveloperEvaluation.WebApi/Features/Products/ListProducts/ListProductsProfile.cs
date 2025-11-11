using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsProfile : Profile
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    public ListProductsProfile()
    {
        CreateMap<ListProductsRequest, ListProductsCommand>()
            .ForMember(d => d.Page, opt => opt.MapFrom(s => s.Page ?? DefaultPage))
            .ForMember(d => d.Size, opt => opt.MapFrom(s => s.Size ?? DefaultSize))
            .ForMember(d => d.Filters, opt => opt.MapFrom(s => s));
        
        CreateMap<ListProductsResult, PaginatedList<ProductResponse>>()
            .ConvertUsing((src, _, ctx) => new PaginatedList<ProductResponse>(
                ctx.Mapper.Map<List<ProductResponse>>(src.Items),
                src.TotalCount,
                src.CurrentPage,
                src.PageSize
            ));
    }
}
