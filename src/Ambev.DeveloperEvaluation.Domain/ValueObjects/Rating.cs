namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Value Object to represent product rating details.
/// </summary>
public class Rating
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
}