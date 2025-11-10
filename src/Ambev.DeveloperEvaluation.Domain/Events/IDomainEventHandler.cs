namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Defines a contract for handling a specific domain event type.
/// </summary>
/// <typeparam name="TEvent">Concrete domain event type.</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event.
    /// </summary>
    /// <param name="domainEvent">Event instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
