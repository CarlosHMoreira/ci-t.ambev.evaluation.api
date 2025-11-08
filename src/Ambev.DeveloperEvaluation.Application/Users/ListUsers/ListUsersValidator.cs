using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersValidator : AbstractValidator<ListUsersCommand>
{
    public ListUsersValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

