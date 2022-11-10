namespace UnitTests;

using ErrorOr;
using FluentAssertions;

public class ErrorsOrEmptyListTests
{
    private record Person(string Name);

    [Fact]
    public void CreateFromValue_WhenAccessingValue_ErrorsOrEmptyList_ShouldBeEmpty()
    {
        // Arrange
        IEnumerable<string> value = new[] { "value" };

        // Act
        var errorOrPerson = ErrorOr.From(value);

        // Assert
        errorOrPerson.ErrorsOrEmptyList.Should().BeEmpty();
        errorOrPerson.Errors.Should().ContainSingle();
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingErrors_ErrorsOrEmptyList_ShouldBeEmpty()
    {
        // Arrange
        var errors = new List<Error> { Error.Validation("User.Name", "Name is too short") };

        // Act
        var errorOrPerson = ErrorOr<Person>.From(errors);

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.Errors.Should().ContainSingle().Which.Should().Be(errors.Single());
        errorOrPerson.ErrorsOrEmptyList.Should().ContainSingle().Which.Should().Be(errors.Single());
    }
}
