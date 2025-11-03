namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record FullName
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public override string ToString() => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";
}