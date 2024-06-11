using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorOr.IntegrationTests.Mvc;

public sealed class MvcApiFixture : IApiFixture<MvcApiFixture>, IAsyncLifetime
{
    private readonly WebApplication _app;

    public MvcApiFixture()
    {
        const string url = "http://localhost:5072";
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Services.AddControllers().AddApplicationPart(typeof(SomeController).Assembly);
        _app = builder.Build();
        _app.MapControllers();
        _app.Urls.Add(url);
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
