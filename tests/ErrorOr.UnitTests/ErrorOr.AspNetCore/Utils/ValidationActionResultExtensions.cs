using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Tests.ErrorOr.AspNetCore.Utils;

public static class ValidationActionResultExtensions
{
    public static void Validate(this IActionResult actionResult, int expectedStatusCode, string expectedTitle)
    {
        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;

        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(expectedStatusCode);

        var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;

        problemDetails.Status.Should().Be(expectedStatusCode);
        problemDetails.Title.Should().Be(expectedTitle);
    }
}
