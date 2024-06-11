namespace ErrorOr.IntegrationTests;

public abstract record ProblemDetails
{
    public int? Status { get; init; }
    public string? Type { get; init; }
    public string? Title { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
}
