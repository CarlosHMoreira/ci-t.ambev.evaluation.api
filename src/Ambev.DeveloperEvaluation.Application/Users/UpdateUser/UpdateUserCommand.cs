using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public sealed class UpdateUserCommand : IRequest<UpdateUserResult>
{
    public Guid Id { get; set; }
    
    public required FullName Name { get; init; }
    
    public required Address Address { get; init; }
    
    public string Phone { get; set; } = string.Empty;
    
    public UserStatus Status { get; set; }
    
    public UserRole Role { get; set; }
    
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateUserCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}