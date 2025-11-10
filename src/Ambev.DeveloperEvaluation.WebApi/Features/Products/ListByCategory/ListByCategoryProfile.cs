using Ambev.DeveloperEvaluation.Application.Products.ListByCategory;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListByCategory;

public class ListByCategoryProfile : Profile
{
    public ListByCategoryProfile()
    {
        CreateMap<ListByCategoryRequest, ListByCategoryCommand>();
        CreateMap<ListByCategoryResult, PaginatedList<ProductResponse>>()
            .ConvertUsing((src, _, ctx) => new PaginatedList<ProductResponse>(
                ctx.Mapper.Map<List<ProductResponse>>(src.Items),
                src.TotalCount,
                src.CurrentPage,
                src.PageSize
            ));
    }
}