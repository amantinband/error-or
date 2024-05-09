using ErrorOr;
using FluentAssertions;

namespace Tests;

public class SwitchAsyncTests
{
    private record Person(string Name);

    [Fact]
    public async Task CallingSwitchAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task ThenAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task ElsesAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchAsync(
            ThenAction,
            ElsesAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchAsync_WhenIsError_ShouldExecuteElseAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task ThenAction(Person _) => throw new Exception("Should not be called");
        Task ElsesAction(IReadOnlyList<Error> errors) => Task.FromResult(errors.Should().BeEquivalentTo(errorOrPerson.Errors));

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchAsync(
            ThenAction,
            ElsesAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task ThenAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchFirstAsync(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsync_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task ThenAction(Person _) => throw new Exception("Should not be called");
        Task OnFirstErrorAction(Error errors)
            => Task.FromResult(errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError));

        // Act
        Func<Task> action = async () => await errorOrPerson.SwitchFirstAsync(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchFirstAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task ThenAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .SwitchFirstAsync(
                ThenAction,
                OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task ThenAction(Person person) => Task.FromResult(person.Should().BeEquivalentTo(errorOrPerson.Value));
        Task ElsesAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = async () => await errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .SwitchAsync(ThenAction, ElsesAction);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
