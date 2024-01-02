using ErrorOr;
using FluentAssertions;

namespace Tests;

public class ChainTests
{
    record Person(string Name);

    [Fact]
    public void ChainErrorOrs_WhenHasValue_ShouldInvokeNextInChain()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        static ErrorOr<string> GetName(Person person) => person.Name;
        static ErrorOr<Person> CreatePersonFromName(string name) => new Person(name);

        // Act
        ErrorOr<Person> result = errorOrPerson
            .Chain(person => GetName(person))
            .Chain(name => CreatePersonFromName(name));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(errorOrPerson.Value);
    }

    [Fact]
    public void ChainErrorOrs_WhenHasError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = Error.NotFound();

        static ErrorOr<string> GetName(Person person) => person.Name;
        static ErrorOr<Person> CreatePersonFromName(string name) => new Person(name);

        // Act
        ErrorOr<Person> result = errorOrPerson
            .Chain(person => GetName(person))
            .Chain(name => CreatePersonFromName(name));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.NotFound());
    }
}
