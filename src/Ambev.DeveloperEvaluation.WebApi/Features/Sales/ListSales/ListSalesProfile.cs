using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public class ListSalesProfile : Profile
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    public ListSalesProfile()
    {
        CreateMap<ListSalesRequest, ListSalesCommand>()
            .ForMember(d => d.Page, opt => opt.MapFrom(s => s.Page ?? DefaultPage))
            .ForMember(d => d.Size, opt => opt.MapFrom(s => s.Size ?? DefaultSize))
            .ForMember(d => d.Filters, opt => opt.MapFrom(s => s));
        
        CreateMap<ListSalesResult, PaginatedList<SaleResponse>>()
            .ConvertUsing((src, _, ctx) => new PaginatedList<SaleResponse>(
                ctx.Mapper.Map<List<SaleResponse>>(src),
                src.TotalCount,
                src.CurrentPage,
                src.PageSize
            ));
    }
}

