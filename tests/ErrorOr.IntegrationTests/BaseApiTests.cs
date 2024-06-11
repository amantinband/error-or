using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace ErrorOr.IntegrationTests;

public abstract class BaseApiTests<TFixture>
    where TFixture : class, IApiFixture<TFixture>
{
    private readonly TFixture _apiFixture;

    protected BaseApiTests(TFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task FailureEndpoint_WhenCalled_ShouldReturnProblemDetails()
    {
        using var response = await _apiFixture.HttpClient.GetAsync("/api/failure");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var errorProblemDetails = await response.Content.ReadFromJsonAsync<ErrorProblemDetails>();
        var expectedProblemDetails = new ErrorProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = "An error occurred while processing your request.",
            Detail = "See the errors property for more information.",
            Errors = [
                new Error(
                    "SomeError",
                    "Some error occurred",
                    ErrorType.Failure,
                    new Dictionary<string, object> { ["key"] = "value" })
            ],
        };
        errorProblemDetails.Should().Be(expectedProblemDetails);
    }

    [Fact]
    public async Task ValidationEndpoint_WhenCalled_ShouldReturnValidationProblemDetails()
    {
        using var response = await _apiFixture.HttpClient.GetAsync("/api/validation");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var expectedProblemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Bad Request",
            Errors = new Dictionary<string, string[]>
            {
                ["email"] = ["Email is required"],
                ["password"] = ["Password needs to have at least 12 characters - use a password manager"],
            },
        };
        errorProblemDetails.Should().Be(expectedProblemDetails);
    }
}
