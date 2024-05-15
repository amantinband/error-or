using ErrorOr;
using FluentAssertions;

namespace Tests;

public class FailIfTests
{
    [Fact]
    public void CallingFailIf_WhenFailsIf_ShouldReturnError()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = errorOrInt
            .FailIf(num => num > 3, Error.Failure());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public async Task CallingFailIfExtensionMethod_WhenFailsIf_ShouldReturnError()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = await errorOrInt
            .ThenAsync(num => Task.FromResult(num))
            .FailIf(num => num > 3, Error.Failure());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void CallingFailIf_WhenDoesNotFailIf_ShouldReturnValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = errorOrInt
            .FailIf(num => num > 10, Error.Failure());

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(5);
    }

    [Fact]
    public void CallingFailIf_WhenIsError_ShouldNotInvokeFailIfFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .FailIf(str => str == string.Empty, Error.Failure());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }
}
