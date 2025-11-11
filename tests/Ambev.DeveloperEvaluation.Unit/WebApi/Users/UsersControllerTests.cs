using Ambev.DeveloperEvaluation.WebApi.Features.Users;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using MediatR;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Users;

public class UsersControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = Substitute.For<IMapper>();
        _controller = new UsersController(_mediator, _mapper);
    }

    [Fact(DisplayName = "POST /users Given valid request When creating Then returns 201 with data")]
    public async Task CreateUser_Valid_ReturnsCreated()
    {
        var request = new CreateUserRequest
        {
            Email = "user@mail.com",
            Password = "Test@12345",
            Phone = "+5511999999999",
            Name = new FullName { FirstName = "John", LastName = "Doe" },
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
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };
        var command = new CreateUserCommand
        {
            Name = request.Name!,
            Address = request.Address!,
            Password = request.Password,
            Email = request.Email,
            Phone = request.Phone,
            Status = request.Status,
            Role = request.Role
        };
        var appResult = new CreateUserResult { Id = Guid.NewGuid(), Name = request.Name!, Address = request.Address!, Email = request.Email, Phone = request.Phone, Role = request.Role, Status = request.Status };
        var responseDto = new CreateUserResponse { Id = appResult.Id, Name = appResult.Name, Address = appResult.Address, Email = appResult.Email, Phone = appResult.Phone, Role = appResult.Role, Status = appResult.Status };
        _mapper.Map<CreateUserCommand>(request).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(appResult);
        _mapper.Map<CreateUserResponse>(appResult).Returns(responseDto);

        var actionResult = await _controller.CreateUser(request, CancellationToken.None);

        actionResult.Should().BeOfType<CreatedResult>();
        var created = (CreatedResult)actionResult;
        created.Value.Should().NotBeNull();
    }

    [Fact(DisplayName = "POST /users Given invalid request When creating Then returns 400")]
    public async Task CreateUser_Invalid_ReturnsBadRequest()
    {
        var request = new CreateUserRequest(); // invalid (missing fields)
        var actionResult = await _controller.CreateUser(request, CancellationToken.None);
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "GET /users/{id} Given valid id When getting Then returns 200 with data")]
    public async Task GetUser_Valid_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var command = new GetUserCommand(id);
        var result = new GetUserResult { Id = id, Name = new FullName { FirstName = "John", LastName = "Doe" }, Address = new Address { City = "City", Street = "Street", Number = 1, ZipCode = "12345", Geolocation = new Geolocation { Latitude = 0, Longitude = 0 } } };
        var response = new GetUserResponse { Id = id, Name = result.Name, Address = result.Address };
        _mapper.Map<GetUserCommand>(id).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(result);
        _mapper.Map<GetUserResponse>(result).Returns(response);

        var actionResult = await _controller.GetUser(id, CancellationToken.None);
        actionResult.Should().BeOfType<OkObjectResult>();
    }

    [Fact(DisplayName = "GET /users/{id} Given empty id When getting Then returns 400")]
    public async Task GetUser_InvalidId_ReturnsBadRequest()
    {
        var actionResult = await _controller.GetUser(Guid.Empty, CancellationToken.None);
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "DELETE /users/{id} Given valid id When deleting Then returns 204")]
    public async Task DeleteUser_Valid_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        var command = new DeleteUserCommand(id);
        _mapper.Map<DeleteUserCommand>(id).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var actionResult = await _controller.DeleteUser(id, CancellationToken.None);
        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Fact(DisplayName = "DELETE /users/{id} Given empty id When deleting Then returns 400")]
    public async Task DeleteUser_Invalid_ReturnsBadRequest()
    {
        var actionResult = await _controller.DeleteUser(Guid.Empty, CancellationToken.None);
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "PUT /users/{id} Given valid request When updating Then returns 200")]
    public async Task UpdateUser_Valid_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            Id = id,
            Phone = "+5511999999999",
            Name = new FullName { FirstName = "John", LastName = "Doe" },
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
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };
        var command = new UpdateUserCommand { Id = id, Name = request.Name!, Address = request.Address!, Phone = request.Phone, Status = request.Status, Role = request.Role };
        var appResult = new UpdateUserResult { Id = id, Name = request.Name!, Address = request.Address!, Phone = request.Phone, Role = request.Role, Status = request.Status };
        var responseDto = new UpdateUserResponse { Id = id, Name = request.Name!, Address = request.Address!, Phone = request.Phone, Role = request.Role, Status = request.Status };
        _mapper.Map<UpdateUserCommand>(request).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(appResult);
        _mapper.Map<UpdateUserResponse>(appResult).Returns(responseDto);

        var actionResult = await _controller.UpdateUser(id, request, CancellationToken.None);
        actionResult.Should().BeOfType<OkObjectResult>();
    }

    [Fact(DisplayName = "PUT /users/{id} Given invalid request When updating Then returns 400")]
    public async Task UpdateUser_Invalid_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        var request = new UpdateUserRequest();
        var actionResult = await _controller.UpdateUser(id, request, CancellationToken.None);
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "GET /users Given valid query When listing Then returns 200")]
    public async Task ListUsers_Valid_ReturnsOk()
    {
        var request = new ListUsersRequest { Page = 1, Size = 10 };
        var command = new ListUsersCommand { Page = 1, Size = 10 };
        var listResult = new ListUsersResult(new [] { new ListUsersItem { Id = Guid.NewGuid() } }) { CurrentPage = 1, PageSize = 10, TotalCount = 1 };
        var firstId = listResult.First().Id;
        var paginated = new PaginatedList<ListUsersItemResponse>(new List<ListUsersItemResponse> { new ListUsersItemResponse { Id = firstId } }, 1, 1, 10);

        _mapper.Map<ListUsersCommand>(request).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(listResult);
        _mapper.Map<PaginatedList<ListUsersItemResponse>>(listResult).Returns(paginated);

        var actionResult = await _controller.ListUsers(request, CancellationToken.None);
        actionResult.Should().BeOfType<OkObjectResult>();
    }

    [Fact(DisplayName = "GET /users Given invalid query When listing Then returns 400")]
    public async Task ListUsers_Invalid_ReturnsBadRequest()
    {
        var request = new ListUsersRequest { Page = 0, Size = -1 };
        var actionResult = await _controller.ListUsers(request, CancellationToken.None);
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }
}
