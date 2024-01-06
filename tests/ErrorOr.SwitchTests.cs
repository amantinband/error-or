using ErrorOr;
using FluentAssertions;

namespace Tests;

public class SwitchTests
{
    private record Person(string Name);

    [Fact]
    public void CallingSwitch_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        void OnValueAction(Person person) => person.Should().BeEquivalentTo(errorOrPerson.Value);
        void OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Action action = () => errorOrPerson.Switch(
            OnValueAction,
            OnErrorsAction);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void CallingSwitch_WhenIsError_ShouldExecuteOnErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        void OnValueAction(Person _) => throw new Exception("Should not be called");
        void OnErrorsAction(IReadOnlyList<Error> errors) => errors.Should().BeEquivalentTo(errorOrPerson.Errors);

        // Act
        Action action = () => errorOrPerson.Switch(
            OnValueAction,
            OnErrorsAction);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void CallingSwitchFirst_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        void OnValueAction(Person person) => person.Should().BeEquivalentTo(errorOrPerson.Value);
        void OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Action action = () => errorOrPerson.SwitchFirst(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void CallingSwitchFirst_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        void OnValueAction(Person _) => throw new Exception("Should not be called");
        void OnFirstErrorAction(Error errors)
            => errors.Should().BeEquivalentTo(errorOrPerson.Errors[0])
                .And.BeEquivalentTo(errorOrPerson.FirstError);

        // Act
        Action action = () => errorOrPerson.SwitchFirst(
            OnValueAction,
            OnFirstErrorAction);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public async Task CallingSwitchFirstAfterThenAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        void OnValueAction(Person person) => person.Should().BeEquivalentTo(errorOrPerson.Value);
        void OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .SwitchFirst(OnValueAction, OnFirstErrorAction);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CallingSwitchAfterThenAsync_WhenIsSuccess_ShouldExecuteOnValueAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        void OnValueAction(Person person) => person.Should().BeEquivalentTo(errorOrPerson.Value);
        void OnErrorsAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .Switch(OnValueAction, OnErrorsAction);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
