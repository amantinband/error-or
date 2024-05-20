using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ErrorOr.IntegrationTests.MinimalApi;

public sealed class MinimalApiFixture : IAsyncLifetime
{
    private readonly WebApplication _app;

    public MinimalApiFixture()
    {
        const string url = "http://localhost:5071";
        var builder = WebApplication.CreateBuilder();
#pragma warning disable ASP0013 // we cannot simply call Configuration.Clear() because it is not available
        builder.Host.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.Sources.Clear());
#pragma warning restore ASP0013
        _app = builder.Build();
        _app.Urls.Add(url);
        _app.MapGet(
            "/api/failure",
            () =>
            {
                ErrorOr<Success> errorOr = Error.Failure(
                    "SomeError",
                    "Some error occurred",
                    new Dictionary<string, object> { ["key"] = "value" });

                return errorOr.Match(
                    onValue: _ => TypedResults.Ok(),
                    onError: errors => errors.ToErrorResult());
            });
        _app.MapGet(
            "/api/validation",
            () =>
            {
                var validationErrors = new List<Error>
                {
                    Error.Validation("email", "Email is required"),
                    Error.Validation(
                        "password",
                        "Password needs to have at least 12 characters - use a password manager"),
                };

                return validationErrors.ToErrorResult();
            });

        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri(url);
    }

    public HttpClient HttpClient { get; }

    public Task InitializeAsync() => _app.StartAsync();

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await _app.StopAsync();
        await _app.DisposeAsync();
    }
}
