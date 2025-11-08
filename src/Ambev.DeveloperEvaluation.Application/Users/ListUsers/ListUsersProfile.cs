using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersProfile : Profile
{
    public ListUsersProfile()
    {
        CreateMap<User, ListUsersItem>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Username));
    }
}

