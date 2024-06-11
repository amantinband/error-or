using Microsoft.AspNetCore.Mvc;

namespace ErrorOr.IntegrationTests.Mvc;

[ApiController]
public sealed class SomeController : ControllerBase
{
    [HttpGet("/api/failure")]
    public IActionResult Failure()
    {
        ErrorOr<Success> errorOr = Error.Failure(
            "SomeError",
            "Some error occurred",
            new Dictionary<string, object> { ["key"] = "value" });

        return errorOr.Match(
            _ => Ok(),
            errors => errors.ToErrorActionResult());
    }

    [HttpGet("/api/validation")]
    public IActionResult Validation()
    {
        var validationErrors = new List<Error>
        {
            Error.Validation("email", "Email is required"),
            Error.Validation(
                "password",
                "Password needs to have at least 12 characters - use a password manager"),
        };

        return validationErrors.ToErrorActionResult();
    }
}
