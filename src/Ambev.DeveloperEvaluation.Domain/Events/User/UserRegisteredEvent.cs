namespace Ambev.DeveloperEvaluation.Domain.Events.User;

public sealed class UserRegisteredEvent(Entities.User user) : DomainEvent
{
    public Guid UserId { get; } = user.Id;
    public string Email { get; } = user.Email;
}