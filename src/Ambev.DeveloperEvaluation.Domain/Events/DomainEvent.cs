namespace Ambev.DeveloperEvaluation.Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    public readonly Guid Id = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}