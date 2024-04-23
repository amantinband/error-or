using ErrorOr;
using FluentAssertions;

namespace Tests;

public class FailIfAsyncTests
{
    private record Person(string Name);

    [Fact]
    public async Task CallingFailIfAsync_WhenFailIf_ShouldReturnError()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = await errorOrInt
            .FailIfAsync(num => Task.FromResult(num > 3), Error.Failure());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public async Task CallingFailIfAsync_WhenFailIf_ShouldReturnValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = await errorOrInt
            .FailIfAsync(num => Task.FromResult(num > 10), Error.Failure());

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(5);
    }

    [Fact]
    public async Task CallingFailIf_WhenIsError_ShouldNotInvoke_FailIfFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .FailIfAsync(str => Task.FromResult(str == string.Empty), Error.Failure());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }
}
