namespace Ambev.DeveloperEvaluation.Domain.Events.User;

public sealed class UserDeletedEvent(Guid id) : DomainEvent
{
    public Guid UserId { get; } = id;
}