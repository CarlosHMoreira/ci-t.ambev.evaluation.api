using System.Collections;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersResult(IEnumerable<ListUsersItem> items) : IEnumerable<ListUsersItem>
{
    private IEnumerable<ListUsersItem> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    
    public IEnumerator<ListUsersItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}

public class ListUsersItem
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public FullName Name { get; set; } = null!;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
}