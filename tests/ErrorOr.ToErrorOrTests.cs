using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ToErrorOrTests
{
    [Fact]
    public void ValueToErrorOr_WhenAccessingValue_ShouldReturnValue()
    {
        // Arrange
        var value = 5;

        // Act
        var result = value.ToErrorOr();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ErrorToErrorOr_WhenAccessingFirstError_ShouldReturnSameError()
    {
        // Arrange
        var error = Error.Unauthorized();

        // Act
        var result = error.ToErrorOr<int>();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(error);
    }

    [Fact]
    public void ListOfErrorsToErrorOr_WhenAccessingErrors_ShouldReturnSameErrors()
    {
        // Arrange
        var errors = new List<Error> { Error.Unauthorized(), Error.Validation() };

        // Act
        var result = errors.ToErrorOr<int>();

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }
}
