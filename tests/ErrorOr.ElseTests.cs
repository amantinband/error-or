using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ElseTests
{
    [Fact]
    public void CallingElse_WhenIsSuccess_ShouldNotInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        string result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => ConvertToString(num))
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.Should().BeEquivalentTo(errorOrString.Value);
    }

    [Fact]
    public void CallingElse_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => ConvertToString(num))
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.Should().BeEquivalentTo("Error count: 1");
    }

    [Fact]
    public void CallingElse_WhenIsSuccess_ShouldNotReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        string result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => ConvertToString(num))
            .Else("oh no");

        // Assert
        result.Should().BeEquivalentTo(errorOrString.Value);
    }

    [Fact]
    public void CallingElse_WhenIsError_ShouldReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => ConvertToString(num))
            .Else("oh no");

        // Assert
        result.Should().BeEquivalentTo("oh no");
    }

    [Fact]
    public async Task CallingElseAfterThenAsync_WhenIsError_ShouldReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = await errorOrString
            .Then(str => ConvertToInt(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .Else("oh no");

        // Assert
        result.Should().BeEquivalentTo("oh no");
    }

    [Fact]
    public async Task CallingElseAfterThenAsync_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        string result = await errorOrString
            .Then(str => ConvertToInt(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.Should().BeEquivalentTo("Error count: 1");
    }

    private static ErrorOr<string> ConvertToString(int num) => num.ToString();

    private static ErrorOr<int> ConvertToInt(string str) => int.Parse(str);

    private static Task<ErrorOr<string>> ConvertToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));
}
