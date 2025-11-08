using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

public class ListUsersProfile : Profile
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    public ListUsersProfile()
    {
        CreateMap<ListUsersRequest, ListUsersCommand>()
            .ForMember(d => d.Page, options => options.NullSubstitute(DefaultPage))
            .ForMember(d => d.Size, options => options.NullSubstitute(DefaultSize));
        
        CreateMap<ListUsersItem, ListUsersItemResponse>();

        CreateMap<ListUsersResult, PaginatedList<ListUsersItemResponse>>()
            .ConvertUsing((source, _, resolutionCtx) => new PaginatedList<ListUsersItemResponse>(
                resolutionCtx.Mapper.Map<List<ListUsersItemResponse>>(source),
                source.TotalCount,
                source.CurrentPage,
                source.PageSize
            ));
    }
}

