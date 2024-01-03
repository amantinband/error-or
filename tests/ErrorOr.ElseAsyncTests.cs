using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ElseAsyncTests
{
    [Fact]
    public async Task CallingElseAsync_WhenIsSuccess_ShouldNotInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        string result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .ElseAsync(errors => Task.FromResult($"Error count: {errors.Count}"));

        // Assert
        result.Should().BeEquivalentTo(errorOrString.Value);
    }

    [Fact]
    public async Task CallingElseAsync_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .ElseAsync(errors => Task.FromResult($"Error count: {errors.Count}"));

        // Assert
        result.Should().BeEquivalentTo("Error count: 1");
    }

    [Fact]
    public async Task CallingElseAsync_WhenIsSuccess_ShouldNotReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        string result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .ElseAsync(Task.FromResult("oh no"));

        // Assert
        result.Should().BeEquivalentTo(errorOrString.Value);
    }

    [Fact]
    public async Task CallingElseAsync_WhenIsError_ShouldReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .ElseAsync(Task.FromResult("oh no"));

        // Assert
        result.Should().BeEquivalentTo("oh no");
    }

    private static Task<ErrorOr<string>> ConvertToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));

    private static Task<ErrorOr<int>> ConvertToIntAsync(string str) => Task.FromResult(ErrorOrFactory.From(int.Parse(str)));
}
