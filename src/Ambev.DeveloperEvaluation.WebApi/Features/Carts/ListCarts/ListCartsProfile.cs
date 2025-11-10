using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public class ListCartsProfile : Profile
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    public ListCartsProfile()
    {
        CreateMap<ListCartsRequest, ListCartsCommand>()
            .ForMember(d => d.Page, opt => opt.MapFrom(s => s.Page ?? DefaultPage))
            .ForMember(d => d.Size, opt => opt.MapFrom(s => s.Size ?? DefaultSize));

        CreateMap<ListCartsResult, PaginatedList<CartResponse>>()
            .ConvertUsing((src, _, ctx) => new PaginatedList<CartResponse>(
                ctx.Mapper.Map<List<CartResponse>>(src.Items),
                src.TotalCount,
                src.CurrentPage,
                src.PageSize
            ));
    }
}
