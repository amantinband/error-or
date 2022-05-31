namespace UnitTests;

using ErrorOr;
using FluentAssertions;

public class ErrorOrTests
{
    private record Person(string Name);

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
    public void ImplicitCastResult_WhenAccessingError_ShouldThrow()
    {
        // Arrange
        var result = new Person("Amichai");

        // Act
        ErrorOr<Person> errorOrPerson = result;
        var accessErrorsAction = () => errorOrPerson.Errors;
        var accessFirstErrorAction = () => errorOrPerson.FirstError;

        // Assert
        accessErrorsAction.Should().ThrowExactly<InvalidOperationException>();
        accessFirstErrorAction.Should().ThrowExactly<InvalidOperationException>();
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
}
