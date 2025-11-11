using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly UserService _userService;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _userService = new UserService(_userRepository, _domainEventDispatcher);
        _handler = new DeleteUserHandler(_userService);
    }

    [Fact(DisplayName = "Given valid id When deleting Then calls service")]
    public async Task Handle_ValidId_CallsService()
    {
        var id = Guid.NewGuid();
        var command = new DeleteUserCommand(id);

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given empty id When deleting Then throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidation()
    {
        var command = new DeleteUserCommand(Guid.Empty);
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
