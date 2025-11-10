namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Abstraction to dispatch domain events without exposing infrastructure details.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a single domain event to its handlers.
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

}
