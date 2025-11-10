using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersHandler(UserService userService, IMapper mapper)
    : IRequestHandler<ListUsersCommand, ListUsersResult>
{
    public async Task<ListUsersResult> Handle(ListUsersCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListUsersValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var (users, total) = await userService.ListAsync(request.Page, request.Size, request.Order, cancellationToken);
        var items = mapper.Map<IEnumerable<ListUsersItem>>(users);

        return new ListUsersResult(items)
        {
            CurrentPage = request.Page,
            TotalCount = total,
            PageSize = request.Size,
        };
    }
}

