using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserHandler(
    UserService userService,
    IMapper mapper
) : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var existingUser = await userService.GetByIdAsync(command.Id, cancellationToken);
        if (existingUser is null)
        {
            throw new ValidationException($"User with id {command.Id} not found");
        }
        var user = mapper.Map(command, existingUser!);

        var updatedUser = await userService.UpdateAsync(user, cancellationToken);
        
        return mapper.Map<UpdateUserResult>(updatedUser);
    }
}