using System.Text.Json.Serialization;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;


public class ListUsersItemResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
}

