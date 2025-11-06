using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;


public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(user => user.Id).NotEmpty();
        RuleFor(user => user.Name).SetValidator(new FullNameValidator());
        RuleFor(user => user.Address).SetValidator(new AddressValidator());
        RuleFor(user => user.Phone).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(user => user.Status).IsInEnum().NotEqual(UserStatus.Unknown);
        RuleFor(user => user.Role).IsInEnum().NotEqual(UserRole.None);
    }
}