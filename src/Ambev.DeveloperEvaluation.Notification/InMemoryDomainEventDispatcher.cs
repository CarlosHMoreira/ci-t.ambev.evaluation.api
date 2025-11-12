using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Notification;

public class InMemoryDomainEventDispatcher(IServiceScopeFactory scopeFactory) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateScope();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = scope.ServiceProvider.GetServices(handlerType);
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("HandleAsync");
            if (method == null)
            {
                continue;
            }
            var task = (Task?)method.Invoke(handler, new object?[] { domainEvent, cancellationToken });
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
