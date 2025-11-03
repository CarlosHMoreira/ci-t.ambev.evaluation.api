namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record Address()
{
    public required string City { get; init; }
    public required string Street { get; init; }
    public int Number { get; init; }
    public required string ZipCode { get; init; }    
    public required Geolocation Geolocation { get; init; }
}