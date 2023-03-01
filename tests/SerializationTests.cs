using System.Text.Json;
using ErrorOr;
using FluentAssertions;

namespace Tests;

public class SerializationTests
{
    private record Person(string Name);

    [Fact]
    public void SerializedErrorOrIntegerValue_WhenDeserialized_ShouldReturnSameValue()
    {
        // Arrange
        var value = 42;
        ErrorOr<int> errorOr = value;
        string serializedErrorOr = JsonSerializer.Serialize(errorOr);

        // Act
        ErrorOr<int> deserializedErrorOr = JsonSerializer.Deserialize<ErrorOr<int>>(serializedErrorOr);

        // Assert
        deserializedErrorOr.Value.Should().Be(value);
        deserializedErrorOr.IsError.Should().Be(false);
        deserializedErrorOr.ErrorsOrEmptyList.Should().BeEmpty();
    }

    [Fact]
    public void SerializedErrorOrIntegerErrors_WhenDeserialized_ShouldReturnErrors()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Age", "User is too young"),
            Error.Validation("User.Name", "Name is too short"),
        };

        ErrorOr<int> errorOr = errors;
        string serializedErrorOr = JsonSerializer.Serialize(errorOr);

        // Act
        ErrorOr<int> deserializedErrorOr = JsonSerializer.Deserialize<ErrorOr<int>>(serializedErrorOr);

        // Assert
        deserializedErrorOr.IsError.Should().Be(true);
        deserializedErrorOr.FirstError.Should().Be(errors[0]);
        deserializedErrorOr.ErrorsOrEmptyList.Should().NotBeEmpty();
        deserializedErrorOr.Value.Should().Be(errorOr.Value);
    }

    [Fact]
    public void SerializedErrorOrPersonValue_WhenDeserialized_ShouldReturnSameValue()
    {
        // Arrange
        var value = new Person("Alp");
        ErrorOr<Person> errorOr = value;
        string serializedErrorOr = JsonSerializer.Serialize(errorOr);

        // Act
        ErrorOr<Person> deserializedErrorOr = JsonSerializer.Deserialize<ErrorOr<Person>>(serializedErrorOr);

        // Assert
        deserializedErrorOr.Value.Should().Be(value);
        deserializedErrorOr.IsError.Should().Be(false);
        deserializedErrorOr.ErrorsOrEmptyList.Should().BeEmpty();
    }

    [Fact]
    public void SerializedErrorOrPersonErrors_WhenDeserialized_ShouldReturnErrors()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        ErrorOr<Person> errorOrPerson = errors;
        string serializedErrorOr = JsonSerializer.Serialize(errorOrPerson);

        // Act
        var deserializedErrorOr = JsonSerializer.Deserialize<ErrorOr<Person>>(serializedErrorOr);

        // Assert
        deserializedErrorOr.IsError.Should().Be(true);
        deserializedErrorOr.FirstError.Should().Be(errors[0]);
        deserializedErrorOr.ErrorsOrEmptyList.Should().NotBeEmpty();
        deserializedErrorOr.Value.Should().Be(errorOrPerson.Value);
    }
}
