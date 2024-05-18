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
    UnsupportedMediaType,
    UnavailableForLegalReasons,
    BadGateway,
    ServiceUnavailable,
    GatewayTimeout,
}
