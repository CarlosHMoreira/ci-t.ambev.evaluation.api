namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record Geolocation
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}