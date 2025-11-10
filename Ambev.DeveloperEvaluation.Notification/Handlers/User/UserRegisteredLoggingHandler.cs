using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Notification.Handlers.User;

public class UserRegisteredLoggingHandler(ILogger<UserRegisteredLoggingHandler> logger)
    : IDomainEventHandler<UserRegisteredEvent>
{
    public Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("User registered event handled: {UserId} {Email}", domainEvent.UserId, domainEvent.Email);
        return Task.CompletedTask;
    }
}
