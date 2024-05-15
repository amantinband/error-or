using Microsoft.AspNetCore.Http;
using Tests.ErrorOr.AspNetCore.Utils;

namespace Tests.ErrorOr.AspNetCore.Extensions;

public class ErrorOrToActionResultExtensions
{
    [Fact]
    public void ErrorToProblemResult_WhenNoCustomizations_ShouldUseDefaultProblemDetailsFactory()
    {
        // Arrange
        Error error = Error.NotFound();

        // Act
        var result = error.ToActionResult();

        // Assert
        result.Validate(
            expectedStatusCode: StatusCodes.Status404NotFound,
            error.Description);
    }

    [Fact]
    public void ListOfErrorsToProblemResult_WhenNoCustomizations_ShouldUseDefaultProblemDetailsFactory()
    {
        // Arrange
        List<Error> errors = [Error.Validation()];

        // Act
        var result = errors.ToActionResult();

        // Assert
        result.Validate(
            expectedStatusCode: StatusCodes.Status400BadRequest,
            expectedTitle: TestConstants.DefaultValidationErrorTitle);
    }
}
