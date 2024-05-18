using Microsoft.AspNetCore.Http;

namespace ErrorOr;

public static class ErrorToHttpStatusCodeExtensions
{
    public static int ToHttpStatsCode(this Error error)
    {
        return error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.UnsupportedMediaType => StatusCodes.Status415UnsupportedMediaType,
            ErrorType.UnavailableForLegalReasons => StatusCodes.Status451UnavailableForLegalReasons,
            ErrorType.BadGateway => StatusCodes.Status502BadGateway,
            ErrorType.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
            ErrorType.GatewayTimeout => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
