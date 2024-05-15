using ErrorOr;
using FluentAssertions;

namespace Tests;

public class MatchTests
{
    private record Person(string Name);

    [Fact]
    public void CallingMatch_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string ThenAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string ElsesAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<string> action = () => errorOrPerson.Match(
            ThenAction,
            ElsesAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void CallingMatch_WhenIsError_ShouldExecuteElseAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        string ThenAction(Person _) => throw new Exception("Should not be called");

        string ElsesAction(IReadOnlyList<Error> errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors);
            return "Nice";
        }

        // Act
        Func<string> action = () => errorOrPerson.Match(
            ThenAction,
            ElsesAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void CallingMatchFirst_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string ThenAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<string> action = () => errorOrPerson.MatchFirst(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void CallingMatchFirst_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        string ThenAction(Person _) => throw new Exception("Should not be called");
        string OnFirstErrorAction(Error errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError);

            return "Nice";
        }

        // Act
        Func<string> action = () => errorOrPerson.MatchFirst(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public async Task CallingMatchFirstAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string ThenAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task<string>> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .MatchFirst(ThenAction, OnFirstErrorAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }

    [Fact]
    public async Task CallingMatchAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string ThenAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string ElsesAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task<string>> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .Match(ThenAction, ElsesAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }
}
