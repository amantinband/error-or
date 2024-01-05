using ErrorOr;
using FluentAssertions;

namespace Tests;

public class MatchTests
{
    private record Person(string Name);

    [Fact]
    public void MatchErrorOr_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string OnValueAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<string> action = () => errorOrPerson.Match(
            OnValueAction,
            OnErrorsAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void MatchErrorOr_WhenIsError_ShouldExecuteOnErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        string OnValueAction(Person _) => throw new Exception("Should not be called");

        string OnErrorsAction(IReadOnlyList<Error> errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors);
            return "Nice";
        }

        // Act
        Func<string> action = () => errorOrPerson.Match(
            OnValueAction,
            OnErrorsAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void MatchFirstErrorOr_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        string OnValueAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return "Nice";
        }

        string OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        var action = () => errorOrPerson.MatchFirst(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }

    [Fact]
    public void MatchFirstErrorOr_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        string OnValueAction(Person _) => throw new Exception("Should not be called");
        string OnFirstErrorAction(Error errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError);

            return "Nice";
        }

        // Act
        var action = () => errorOrPerson.MatchFirst(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow().Subject.Should().Be("Nice");
    }
}
