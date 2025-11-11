using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateUserHandler"/> class.
/// </summary>
public class CreateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly UserService _userService;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly CreateUserHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _userService = new UserService(_userRepository, _domainEventDispatcher);
        _handler = new CreateUserHandler(_userService, _mapper, _passwordHasher);
    }

    /// <summary>
    /// Tests that a valid user creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When creating user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Password = command.Password,
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };

        var created = new User
        {
            Id = user.Id,
            Name = user.Name,
            Address = user.Address,
            Password = "hashedPassword",
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status,
            Role = user.Role
        };

        var result = new CreateUserResult
        {
            Id = created.Id,
            Name = created.Name,
            Address = created.Address,
            Email = created.Email,
            Phone = created.Phone,
            Role = created.Role,
            Status = created.Status
        };

        _mapper.Map<User>(command).Returns(user);
        _passwordHasher.HashPassword(command.Password).Returns("hashedPassword");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(created);
        _mapper.Map<CreateUserResult>(created).Returns(result);
        _domainEventDispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // When
        var createUserResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createUserResult.Should().NotBeNull();
        createUserResult.Id.Should().Be(created.Id);
        await _userRepository.Received(1).CreateAsync(Arg.Is<User>(u => u.Password == "hashedPassword"), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid user creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid user data When creating user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateUserCommand { Name = new FullName { FirstName = string.Empty, LastName = "Doe" }, Address = new Address { City = "", Street = "", Number = 0, ZipCode = "", Geolocation = new Geolocation { Latitude = 0, Longitude = 0 } } }; // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that the password is hashed before saving the user.
    /// </summary>
    [Fact(DisplayName = "Given user creation request When handling Then password is hashed")]
    public async Task Handle_ValidRequest_HashesPassword()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        var originalPassword = command.Password;
        const string hashedPassword = "h@shedPassw0rd";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Password = command.Password,
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };
        var created = new User
        {
            Id = user.Id,
            Name = user.Name,
            Address = user.Address,
            Password = hashedPassword,
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status,
            Role = user.Role
        };

        _mapper.Map<User>(command).Returns(user);
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.HashPassword(originalPassword).Returns(hashedPassword);
        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(created);
        _mapper.Map<CreateUserResult>(created).Returns(new CreateUserResult { Id = created.Id, Name = created.Name, Address = created.Address, Email = created.Email, Phone = created.Phone, Role = created.Role, Status = created.Status });
        _domainEventDispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _passwordHasher.Received(1).HashPassword(originalPassword);
        await _userRepository.Received(1).CreateAsync(
            Arg.Is<User>(u => u.Password == hashedPassword),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to user entity")]
    public async Task Handle_ValidRequest_MapsCommandToUser()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Address = command.Address,
            Password = command.Password,
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };
        var created = new User
        {
            Id = user.Id,
            Name = user.Name,
            Address = user.Address,
            Password = "hashedPassword",
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status,
            Role = user.Role
        };

        _mapper.Map<User>(command).Returns(user);
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");
        _userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(created);
        _mapper.Map<CreateUserResult>(created).Returns(new CreateUserResult { Id = created.Id, Name = created.Name, Address = created.Address, Email = created.Email, Phone = created.Phone, Role = created.Role, Status = created.Status });
        _domainEventDispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<User>(Arg.Is<CreateUserCommand>(c =>
            c.Email == command.Email &&
            c.Phone == command.Phone &&
            c.Status == command.Status &&
            c.Role == command.Role &&
            c.Name.FirstName == command.Name.FirstName));
    }
}
