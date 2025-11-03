using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public sealed class FullNameValidator : AbstractValidator<FullName>
{
    public FullNameValidator()
    {
        RuleFor(name => name.FirstName).NotEmpty().Length(2, 100);
        RuleFor(name => name.LastName).NotEmpty().Length(2, 100);
    }
}