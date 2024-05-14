using Microsoft.AspNetCore.Http;

namespace ErrorOr;

public static class ErrorToHttpStatusCodeExtensions
{
    public static int ToHttpStatsCode(this Error error)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorToStatusCodeMapper)
        {
            if (mapping(error) is int statusCode)
            {
                return statusCode;
            }
        }

        return error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
