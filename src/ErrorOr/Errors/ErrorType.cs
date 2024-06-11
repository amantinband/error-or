namespace ErrorOr;

/// <summary>
/// Error types.
/// </summary>
public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden,
    Gone,
    PreconditionFailed,
    UnsupportedMediaType,
    UnprocessableEntity,
    UnavailableForLegalReasons,
    BadGateway,
    ServiceUnavailable,
    GatewayTimeout,
}
