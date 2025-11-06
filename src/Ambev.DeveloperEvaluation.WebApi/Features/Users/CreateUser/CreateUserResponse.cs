using System.Text.Json.Serialization;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// API response model for CreateUser operation
/// </summary>
public class CreateUserResponse
{
    /// <summary>
    /// The unique identifier of the created user
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The user's email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The user's full name
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    public required FullName Name { get; init; }
    
    public required Address Address { get; init; }

    /// <summary>
    /// The user's phone number
    /// </summary>
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// The user's role in the system
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; init; }

    /// <summary>
    /// The current status of the user
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; init; }
}
