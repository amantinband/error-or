using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ThenAsyncTests
{
    [Fact]
    public async Task CallingThenAsync_WhenIsSuccess_ShouldInvokeNextThen()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => Task.FromResult(num * 2))
            .ThenDoAsync(num => Task.Run(() => { _ = 5; }))
            .ThenAsync(num => ConvertToStringAsync(num));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo("10");
    }

    [Fact]
    public async Task CallingThenAsync_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(str => ConvertToIntAsync(str))
            .ThenAsync(num => Task.FromResult(num * 2))
            .ThenDoAsync(num => Task.Run(() => { _ = 5; }))
            .ThenAsync(num => ConvertToStringAsync(num));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(errorOrString.FirstError);
    }

    private static Task<ErrorOr<string>> ConvertToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));

    private static Task<ErrorOr<int>> ConvertToIntAsync(string str) => Task.FromResult(ErrorOrFactory.From(int.Parse(str)));
}
