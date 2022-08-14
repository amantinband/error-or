namespace UnitTests;

using ErrorOr;
using FluentAssertions;

public class ErrorOrTests
{
    private record Person(string Name);

    [Fact]
    public void CreateFromValue_WhenAccessingValue_ShouldReturnValue()
    {
        // Arrange
        IEnumerable<string> value = new[] { "value" };

        // Act
        var errorOrPerson = ErrorOr.From(value);

        // Assert
        errorOrPerson.IsError.Should().BeFalse();
        errorOrPerson.Value.Should().BeSameAs(value);
    }

    [Fact]
    public void CreateFromValue_WhenAccessingErrors_ShouldReturnUnexpectedError()
    {
        // Arrange
        IEnumerable<string> value = new[] { "value" };
        var errorOrPerson = ErrorOr.From(value);

        // Act
        var errors = errorOrPerson.Errors;

        // Assert
        errors.Should().ContainSingle().Which.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public void CreateFromValue_WhenAccessingFirstError_ShouldReturnUnexpectedError()
    {
        // Arrange
        IEnumerable<string> value = new[] { "value" };
        var errorOrPerson = ErrorOr.From(value);

        // Act
        var firstError = errorOrPerson.FirstError;

        // Assert
        firstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        var errors = new List<Error> { Error.Validation("User.Name", "Name is too short") };

        // Act
        var errorOrPerson = ErrorOr<Person>.From(errors);

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.Errors.Should().ContainSingle().Which.Should().Be(errors.Single());
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingValue_ShouldReturnDefault()
    {
        // Arrange
        var errors = new List<Error> { Error.Validation("User.Name", "Name is too short") };
        var errorOrPerson = ErrorOr<Person>.From(errors);

        // Act
        var value = errorOrPerson.Value;

        // Assert
        value.Should().Be(default);
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        var result = new Person("Amichai");

        // Act
        ErrorOr<Person> errorOr = result;

        // Assert
        errorOr.IsError.Should().BeFalse();
        errorOr.Value.Should().Be(result);
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingErrors_ShouldReturnUnexpectedError()
    {
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        // Act
        var errors = errorOrPerson.Errors;

        // Assert
        errors.Should().ContainSingle().Which.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingFirstError_ShouldReturnUnexpectedError()
    {
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        // Act
        var firstError = errorOrPerson.FirstError;

        // Assert
        firstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public void ImplicitCastPrimitiveResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        const int result = 4;

        // Act
        ErrorOr<int> errorOrInt = result;

        // Assert
        errorOrInt.IsError.Should().BeFalse();
        errorOrInt.Value.Should().Be(result);
    }

    [Fact]
    public void ImplicitCastErrorOrType_WhenAccessingResult_ShouldReturnValue()
    {
        // Act
        ErrorOr<Success> errorOrSuccess = Result.Success;
        ErrorOr<Created> errorOrCreated = Result.Created;
        ErrorOr<Deleted> errorOrDeleted = Result.Deleted;
        ErrorOr<Updated> errorOrUpdated = Result.Updated;

        // Assert
        errorOrSuccess.IsError.Should().BeFalse();
        errorOrSuccess.Value.Should().Be(Result.Success);

        errorOrCreated.IsError.Should().BeFalse();
        errorOrCreated.Value.Should().Be(Result.Created);

        errorOrDeleted.IsError.Should().BeFalse();
        errorOrDeleted.Value.Should().Be(Result.Deleted);

        errorOrUpdated.IsError.Should().BeFalse();
        errorOrUpdated.Value.Should().Be(Result.Updated);
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        var error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.Errors.Should().ContainSingle().Which.Should().Be(error);
    }

    [Fact]
    public void ImplicitCastError_WhenAccessingValue_ShouldReturnDefault()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = Error.Validation("User.Name", "Name is too short");

        // Act
        var value = errorOrPerson.Value;

        // Assert
        value.Should().Be(default);
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingFirstError_ShouldReturnError()
    {
        // Arrange
        var error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.FirstError.Should().Be(error);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.Errors.Should().HaveCount(errors.Count).And.BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingErrors_ShouldReturnErrorArray()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.Errors.Should().HaveCount(errors.Length).And.BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.FirstError.Should().Be(errors[0]);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.FirstError.Should().Be(errors[0]);
    }
}
