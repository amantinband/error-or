using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ThenTests
{
    [Fact]
    public void CallingThen_WhenIsSuccess_ShouldInvokeGivenFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => num * 2)
            .Then(num => ConvertToString(num));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo("10");
    }

    [Fact]
    public void CallingThen_WhenIsSuccess_ShouldInvokeGivenAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<int> result = errorOrString
            .ThenDo(str => { _ = 5; })
            .Then(str => ConvertToInt(str))
            .ThenDo(str => { _ = 5; });

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(5);
    }

    [Fact]
    public void CallingThen_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(str => ConvertToInt(str))
            .Then(num => num * 2)
            .ThenDo(str => { _ = 5; })
            .Then(num => ConvertToString(num));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(errorOrString.FirstError);
    }

    [Fact]
    public async Task CallingThenAfterThenAsync_WhenIsSuccess_ShouldInvokeGivenFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .Then(num => num * 2)
            .ThenAsync(num => ConvertToStringAsync(num))
            .Then(str => ConvertToInt(str))
            .ThenAsync(num => ConvertToStringAsync(num))
            .ThenDo(num => { _ = 5; });

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("10");
    }

    [Fact]
    public async Task CallingThenAfterThenAsync_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .Then(num => ConvertToString(num));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(errorOrString.FirstError);
    }

    private static ErrorOr<string> ConvertToString(int num) => num.ToString();

    private static ErrorOr<int> ConvertToInt(string str) => int.Parse(str);

    private static Task<ErrorOr<int>> ConvertToIntAsync(string str) => Task.FromResult(ErrorOrFactory.From(int.Parse(str)));

    private static Task<ErrorOr<string>> ConvertToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));
}
