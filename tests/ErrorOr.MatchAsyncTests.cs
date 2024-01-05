using ErrorOr;
using FluentAssertions;

namespace Tests;

public class MatchAsyncTests
{
    private record Person(string Name);

    [Fact]
    public async Task MatchAsyncErrorOr_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task<string> OnValueAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return Task.FromResult("Nice");
        }

        Task<string> OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        var action = async () => await errorOrPerson.MatchAsync(
            OnValueAction,
            OnErrorsAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }

    [Fact]
    public async Task MatchAsyncErrorOr_WhenIsError_ShouldExecuteOnErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> OnValueAction(Person _) => throw new Exception("Should not be called");

        Task<string> OnErrorsAction(IReadOnlyList<Error> errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors);
            return Task.FromResult("Nice");
        }

        // Act
        var action = async () => await errorOrPerson.MatchAsync(
            OnValueAction,
            OnErrorsAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }

    [Fact]
    public async Task MatchFirstAsyncErrorOr_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task<string> OnValueAction(Person person)
        {
            person.Should().BeEquivalentTo(errorOrPerson.Value);
            return Task.FromResult("Nice");
        }

        Task<string> OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        var action = async () => await errorOrPerson.MatchFirstAsync(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }

    [Fact]
    public async Task MatchFirstAsyncErrorOr_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> OnValueAction(Person _) => throw new Exception("Should not be called");
        Task<string> OnFirstErrorAction(Error errors)
        {
            errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError);

            return Task.FromResult("Nice");
        }

        // Act
        var action = async () => await errorOrPerson.MatchFirstAsync(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        (await action.Should().NotThrowAsync()).Subject.Should().Be("Nice");
    }
}
