using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

/// <summary>
/// Command to list users with pagination and ordering
/// </summary>
public record ListUsersCommand : IRequest<ListUsersResult>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
}

