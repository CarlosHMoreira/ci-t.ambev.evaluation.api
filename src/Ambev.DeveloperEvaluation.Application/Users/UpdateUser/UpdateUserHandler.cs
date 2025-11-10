using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserHandler(
    IUserRepository userRepository,
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
        
        var existingUser = await userRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingUser is null)
        {
            throw new ValidationException($"User with id {command.Id} not found");
        }
        var user = mapper.Map(command, existingUser!);

        var updatedUser = await userRepository.UpdateAsync(user, cancellationToken);
        
        return mapper.Map<UpdateUserResult>(updatedUser);
    }
}