using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ChainAsyncTests
{
    record Person(string Name);

    [Fact]
    public async Task ChainErrorOrsAsync_WhenStateIsValue_ShouldInvokeNextInChain()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        static Task<ErrorOr<string>> GetNameAsync(Person person) => Task.FromResult(ErrorOrFactory.From(person.Name));
        static Task<ErrorOr<Person>> CreatePersonFromNameAsync(string name) => Task.FromResult(ErrorOrFactory.From(new Person(name)));

        // Act
        var result = await errorOrPerson
            .ChainAsync(person => GetNameAsync(person))
            .ChainAsync(name => CreatePersonFromNameAsync(name))
            .ChainAsync(person => GetNameAsync(person))
            .ChainAsync(name => CreatePersonFromNameAsync(name));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(errorOrPerson.Value);
    }

    [Fact]
    public async Task ChainErrorOrsAsync_WhenStateIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = Error.NotFound();

        static Task<ErrorOr<string>> GetNameAsync(Person person) => Task.FromResult(ErrorOrFactory.From(person.Name));
        static Task<ErrorOr<Person>> CreatePersonFromNameAsync(string name) => Task.FromResult(ErrorOrFactory.From(new Person(name)));

        // Act
        var result = await errorOrPerson
            .ChainAsync(person => GetNameAsync(person))
            .ChainAsync(name => CreatePersonFromNameAsync(name))
            .ChainAsync(person => GetNameAsync(person))
            .ChainAsync(name => CreatePersonFromNameAsync(name));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.NotFound());
    }
}
