using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public sealed class GeolocationValidator : AbstractValidator<Geolocation>
{
    public GeolocationValidator()
    {
        RuleFor(geo => geo.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(geo => geo.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}