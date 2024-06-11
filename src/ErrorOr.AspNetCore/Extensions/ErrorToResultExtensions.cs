using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public static class ErrorToResultExtensions
{
    public static IResult ToErrorResult(this Error error, ErrorOrOptions? options = null) =>
        ToErrorResult([error], options);

    public static IResult ToErrorResult(this List<Error> errors, ErrorOrOptions? options = null)
    {
        options ??= ErrorOrOptions.Instance;
        if (options.CustomToErrorResult is not null)
        {
            return options.CustomToErrorResult(errors);
        }

        var prototype = options.CustomCreatePrototype?.Invoke(errors) ??
            ProblemDetailsPrototype.CreateDefaultFromErrors(errors);
        var problemDetails = prototype.ConvertToProblemDetails();
        return problemDetails.CreateDefaultErrorResult();
    }

    public static IResult CreateDefaultErrorResult(this ProblemDetails problemDetails)
    {
        return problemDetails.Status switch
        {
            // TODO: Unauthorized and forbid do not support problem details, should we process it differently?
            StatusCodes.Status404NotFound => TypedResults.NotFound(problemDetails),
            StatusCodes.Status409Conflict => TypedResults.Conflict(problemDetails),
            StatusCodes.Status422UnprocessableEntity => TypedResults.UnprocessableEntity(problemDetails),
            _ => Results.Problem(problemDetails),
        };
    }
}
