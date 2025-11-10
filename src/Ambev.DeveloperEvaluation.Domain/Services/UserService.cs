using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.User;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class UserService(IUserRepository userRepository, IDomainEventDispatcher domainEventDispatcher)
{
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        user = await userRepository.CreateAsync(user, cancellationToken);
        await domainEventDispatcher.DispatchAsync(new UserRegisteredEvent(user), cancellationToken);
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted =  await userRepository.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            await domainEventDispatcher.DispatchAsync(new UserDeletedEvent(id), cancellationToken);    
        }
        return deleted;
    }

    public async Task<object> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        user = await userRepository.UpdateAsync(user, cancellationToken);
        await domainEventDispatcher.DispatchAsync(new UserUpdatedEvent(user), cancellationToken);
        return user;
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> ListAsync(int page, int size, string? order, CancellationToken cancellationToken = default)
    {
        return await userRepository.ListAsync(page, size, order, cancellationToken);
    }
}