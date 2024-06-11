namespace ErrorOr.IntegrationTests;

public interface IApiFixture<out T>
    where T : IApiFixture<T>
{
    HttpClient HttpClient { get; }
}
