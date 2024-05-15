using Microsoft.AspNetCore.Http;

namespace ErrorOr;

public static class ErrorToResultExtensions
{
    public static IResult ToResult(this Error error)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorToResultMapper)
        {
            if (mapping(error) is IResult result)
            {
                return result;
            }
        }

        return ToResult([error]);
    }

    public static IResult ToResult(this List<Error> errors)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorListToResultMapper)
        {
            if (mapping(errors) is IResult result)
            {
                return result;
            }
        }

        return Results.Problem(errors.ToProblemDetails());
    }
}
