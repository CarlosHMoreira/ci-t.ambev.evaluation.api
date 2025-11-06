using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserResult
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public required FullName Name { get; set; }
    public required Address Address { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Domain.Enums.UserRole Role { get; set; }
    public Domain.Enums.UserStatus Status { get; set; }
}
