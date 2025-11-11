using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the UserValidator class.
/// Tests cover validation of all user properties including name (first/last), email,
/// password, phone, status, and role requirements.
/// </summary>
public class UserValidatorTests
{
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _validator = new UserValidator();
    }

    /// <summary>
    /// Tests that validation passes when all user properties are valid.
    /// </summary>
    [Fact(DisplayName = "Valid user should pass all validation rules")]
    public void Given_ValidUser_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests invalid first name scenarios (empty and too short) should fail validation.
    /// </summary>
    [Theory(DisplayName = "Invalid first name formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("A")] // Too short (<2)
    public void Given_InvalidFirstName_When_Validated_Then_ShouldHaveError(string firstName)
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        var invalid = CloneWithName(user, firstName, user.Name.LastName);
        // Act
        var result = _validator.TestValidate(invalid);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name.FirstName);
    }

    /// <summary>
    /// Tests invalid last name scenarios (empty and too short) should fail validation.
    /// </summary>
    [Theory(DisplayName = "Invalid last name formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("B")] // Too short (<2)
    public void Given_InvalidLastName_When_Validated_Then_ShouldHaveError(string lastName)
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        var invalid = CloneWithName(user, user.Name.FirstName, lastName);
        // Act
        var result = _validator.TestValidate(invalid);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name.LastName);
    }

    /// <summary>
    /// Tests that overly long first name should fail validation ( > 100 characters ).
    /// </summary>
    [Fact(DisplayName = "First name longer than maximum length should fail validation")]
    public void Given_FirstNameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        var invalid = CloneWithName(user, new string('X', 101), user.Name.LastName);
        // Act
        var result = _validator.TestValidate(invalid);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name.FirstName);
    }

    /// <summary>
    /// Tests that overly long last name should fail validation ( > 100 characters ).
    /// </summary>
    [Fact(DisplayName = "Last name longer than maximum length should fail validation")]
    public void Given_LastNameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        var invalid = CloneWithName(user, user.Name.FirstName, new string('Y', 101));
        // Act
        var result = _validator.TestValidate(invalid);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name.LastName);
    }

    /// <summary>
    /// Tests that validation fails for invalid email formats.
    /// </summary>
    [Fact(DisplayName = "Invalid email formats should fail validation")]
    public void Given_InvalidEmail_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Email = UserTestData.GenerateInvalidEmail();

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Tests that validation fails for invalid password formats.
    /// </summary>
    [Fact(DisplayName = "Invalid password formats should fail validation")]
    public void Given_InvalidPassword_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Password = UserTestData.GenerateInvalidPassword();

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    /// Tests that validation fails for invalid phone formats.
    /// </summary>
    [Fact(DisplayName = "Invalid phone formats should fail validation")]
    public void Given_InvalidPhone_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Phone = UserTestData.GenerateInvalidPhone();

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    /// <summary>
    /// Tests that validation fails when user status is Unknown.
    /// </summary>
    [Fact(DisplayName = "Unknown status should fail validation")]
    public void Given_UnknownStatus_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Unknown;

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// Tests that validation fails when user role is None.
    /// </summary>
    [Fact(DisplayName = "None role should fail validation")]
    public void Given_NoneRole_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Role = UserRole.None;

        // Act
        var result = _validator.TestValidate(user);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    private static Ambev.DeveloperEvaluation.Domain.Entities.User CloneWithName(Ambev.DeveloperEvaluation.Domain.Entities.User source, string firstName, string lastName)
    {
        return new Ambev.DeveloperEvaluation.Domain.Entities.User
        {
            Name = new Ambev.DeveloperEvaluation.Domain.ValueObjects.FullName { FirstName = firstName, LastName = lastName },
            Email = source.Email,
            Password = source.Password,
            Phone = source.Phone,
            Role = source.Role,
            Status = source.Status,
            Address = source.Address,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
