using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public sealed class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(address => address.City).NotEmpty().Length(2, 100);
        RuleFor(address => address.Street).NotEmpty().Length(2, 200);
        RuleFor(address => address.Number).GreaterThan(0);
        RuleFor(address => address.ZipCode).NotEmpty().Matches(@"^\d{8}$").WithMessage("ZipCode must be in the format 99999999.");
        RuleFor(address => address.Geolocation).SetValidator(new GeolocationValidator());
    }
    
}