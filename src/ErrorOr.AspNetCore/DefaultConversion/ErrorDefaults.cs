using Microsoft.AspNetCore.Http;

namespace ErrorOr;

public static class ErrorDefaults
{
    public static ProblemDetailInfo Validation { get; } = new (
        ErrorType.Validation,
        StatusCodes.Status400BadRequest,
        "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "Bad Request");

    public static ProblemDetailInfo Unauthorized { get; } = new (
        ErrorType.Unauthorized,
        StatusCodes.Status401Unauthorized,
        "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        "Unauthorized");

    public static ProblemDetailInfo Forbidden { get; } = new (
        ErrorType.Forbidden,
        StatusCodes.Status403Forbidden,
        "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        "Forbidden");

    public static ProblemDetailInfo NotFound { get; } = new (
        ErrorType.NotFound,
        StatusCodes.Status404NotFound,
        "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        "Not Found");

    public static ProblemDetailInfo Conflict { get; } = new (
        ErrorType.Conflict,
        StatusCodes.Status409Conflict,
        "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        "Conflict");

    public static ProblemDetailInfo Gone { get; } = new (
        ErrorType.Gone,
        StatusCodes.Status410Gone,
        "https://tools.ietf.org/html/rfc9110#section-15.5.11",
        "Gone");

    public static ProblemDetailInfo PreconditionFailed { get; } = new (
        ErrorType.PreconditionFailed,
        StatusCodes.Status412PreconditionFailed,
        "https://tools.ietf.org/html/rfc9110#section-15.5.13",
        "Precondition Failed");

    public static ProblemDetailInfo UnsupportedMediaType { get; } = new (
        ErrorType.UnsupportedMediaType,
        StatusCodes.Status415UnsupportedMediaType,
        "https://tools.ietf.org/html/rfc9110#section-15.5.16",
        "Unsupported Media Type");

    public static ProblemDetailInfo UnprocessableEntity { get; } = new (
        ErrorType.UnprocessableEntity,
        StatusCodes.Status422UnprocessableEntity,
        "https://tools.ietf.org/html/rfc4918#section-11.2",
        "Unprocessable Entity");

    public static ProblemDetailInfo UnavailableForLegalReasons { get; } = new (
        ErrorType.UnavailableForLegalReasons,
        StatusCodes.Status451UnavailableForLegalReasons,
        "https://tools.ietf.org/html/rfc7725#section-3",
        "Unavailable for Legal Reasons");

    public static ProblemDetailInfo Failure { get; } = new (
        ErrorType.Failure,
        StatusCodes.Status500InternalServerError,
        "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        "An error occurred while processing your request.");

    public static ProblemDetailInfo BadGateway { get; } = new (
        ErrorType.BadGateway,
        StatusCodes.Status502BadGateway,
        "https://tools.ietf.org/html/rfc9110#section-15.6.3",
        "Bad Gateway");

    public static ProblemDetailInfo ServiceUnavailable { get; } = new (
        ErrorType.ServiceUnavailable,
        StatusCodes.Status503ServiceUnavailable,
        "https://tools.ietf.org/html/rfc9110#section-15.6.4",
        "Service Unavailable");

    public static ProblemDetailInfo GatewayTimeout { get; } = new (
        ErrorType.GatewayTimeout,
        StatusCodes.Status504GatewayTimeout,
        "https://tools.ietf.org/html/rfc9110#section-15.6.5",
        "Gateway Timeout");

    public static Dictionary<ErrorType, ProblemDetailInfo> DefaultMappings { get; } =
        new[]
            {
                Validation,
                Unauthorized,
                Forbidden,
                NotFound,
                Conflict,
                Gone,
                PreconditionFailed,
                UnsupportedMediaType,
                UnprocessableEntity,
                Failure,
                BadGateway,
                ServiceUnavailable,
                GatewayTimeout,
            }
           .ToDictionary(i => i.ErrorType);
}
