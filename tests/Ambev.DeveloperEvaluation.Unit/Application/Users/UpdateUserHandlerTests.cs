using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly UserService _userService;
    private readonly IMapper _mapper;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _userService = new UserService(_userRepository, _domainEventDispatcher);
        _handler = new UpdateUserHandler(_userService, _mapper);
    }

    [Fact(DisplayName = "Given valid update When handling Then returns mapped result")]
    public async Task Handle_ValidUpdate_ReturnsResult()
    {
        var id = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = id,
            Name = new FullName
            {
                FirstName = "John",
                LastName = "Doe"
            },
            Address = new Address
            {
                City = "City",
                Street = "Street",
                Number = 1,
                ZipCode = "11111111",
                Geolocation = new Geolocation
                {
                    Latitude = 0,
                    Longitude = 0
                }
            },
            Phone = "99999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };
        var existing = new User { Id = id, Email = "test@mail.com", Name = command.Name, Address = command.Address };
        var updated = new User { Id = id, Email = "changed@mail.com", Name = command.Name, Address = command.Address };
        var mapped = new UpdateUserResult { Id = id, Name = command.Name, Address = command.Address };

        _userRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map(command, existing).Returns(updated);
        _userRepository.UpdateAsync(updated, Arg.Any<CancellationToken>()).Returns(updated);
        _mapper.Map<UpdateUserResult>(updated).Returns(mapped);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().BeEquivalentTo(command.Name);
        result.Address.Should().BeEquivalentTo(command.Address);
        await _userRepository.Received(1).UpdateAsync(updated, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given empty id When updating Then throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidation()
    {
        var command = new UpdateUserCommand { Id = Guid.Empty, Name = new FullName { FirstName = "A", LastName = "B" }, Address = new Address { City = "C", Street = "S", Number = 1, ZipCode = "00000", Geolocation = new Geolocation { Latitude = 0, Longitude = 0 } } };
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given not found user When updating Then throws validation exception")]
    public async Task Handle_UserNotFound_ThrowsValidation()
    {
        var command = new UpdateUserCommand { Id = Guid.NewGuid(), Name = new FullName { FirstName = "A", LastName = "B" }, Address = new Address { City = "C", Street = "S", Number = 1, ZipCode = "00000", Geolocation = new Geolocation { Latitude = 0, Longitude = 0 } } };
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
