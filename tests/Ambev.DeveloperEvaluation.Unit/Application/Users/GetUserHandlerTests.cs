using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly UserService _userService;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _userService = new UserService(_userRepository, _domainEventDispatcher);
        _handler = new GetUserHandler(_userService, _mapper);
    }

    [Fact(DisplayName = "Given valid id When handling Then returns mapped user result")]
    public async Task Handle_ValidId_ReturnsResult()
    {
        var id = Guid.NewGuid();
        var command = new GetUserCommand(id);
        var user = new User { Id = id, Email = "test@mail.com", Name = new FullName { FirstName = "John", LastName = "Doe" }, Address = new Address { City = "City", Street = "Street", Number = 1, ZipCode = "12345", Geolocation = new Geolocation { Latitude = 0, Longitude = 0 } } };
        var expected = new GetUserResult { Id = id, Name = user.Name, Address = user.Address };
        _userRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact(DisplayName = "Given empty id When handling Then throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidation()
    {
        var command = new GetUserCommand(Guid.Empty);
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given non existing user When handling Then throws KeyNotFoundException")]
    public async Task Handle_NotFound_ThrowsKeyNotFound()
    {
        var command = new GetUserCommand(Guid.NewGuid());
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
