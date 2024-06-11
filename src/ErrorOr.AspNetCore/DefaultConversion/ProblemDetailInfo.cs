namespace ErrorOr;

public readonly record struct ProblemDetailInfo(
    ErrorType ErrorType,
    int StatusCode,
    string Type,
    string Title,
    string? Detail = null
);
