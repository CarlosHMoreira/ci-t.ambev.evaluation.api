using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class ListUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly UserService _userService;
    private readonly IMapper _mapper;
    private readonly ListUsersHandler _handler;

    public ListUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _mapper = Substitute.For<IMapper>();
        _userService = new UserService(_userRepository, _domainEventDispatcher);
        _handler = new ListUsersHandler(_userService, _mapper);
    }

    [Fact(DisplayName = "Given valid paging When listing Then returns result with total")]
    public async Task Handle_ValidPaging_ReturnsResult()
    {
        var command = new ListUsersCommand { Page = 1, Size = 10 };
        var users = new List<User> { new User { Id = Guid.NewGuid(), Email = "a@b.com" } } as IEnumerable<User>;
        _userRepository.ListAsync(1, 10, null, Arg.Any<CancellationToken>()).Returns((users, 1));
        _mapper.Map<IEnumerable<ListUsersItem>>(users).Returns(new [] { new ListUsersItem { Id = users.First().Id } });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact(DisplayName = "Given invalid paging When listing Then throws validation exception")]
    public async Task Handle_InvalidPaging_ThrowsValidation()
    {
        var command = new ListUsersCommand { Page = 0, Size = -1 };
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
