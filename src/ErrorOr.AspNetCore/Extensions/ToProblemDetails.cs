using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public static class ErrorToProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this List<Error> errors)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorListToProblemDetailsMapper)
        {
            if (mapping(errors) is ProblemDetails problemDetails)
            {
                return ErrorOrOptions.Instance.IncludeMetadataInProblemDetails
                    ? problemDetails.AddExtensions(errors)
                    : problemDetails;
            }
        }

        return errors switch
        {
            { Count: 0 } => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Something went wrong" },
            var _ when errors.All(error => error.Type == ErrorType.Validation) => errors.ToValidationProblemDetails(),
            _ => errors[0].ToProblemDetails(),
        };
    }

    public static ProblemDetails ToProblemDetails(this Error error)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorToProblemDetailsMapper)
        {
            if (mapping(error) is ProblemDetails problemDetails)
            {
                return problemDetails;
            }
        }

        return new ProblemDetails { Status = error.ToHttpStatsCode(), Title = error.ToTitle() }.AddExtensions(error);
    }

    public static ProblemDetails ToValidationProblemDetails(this List<Error> errors)
    {
        var problemDetails = new HttpValidationProblemDetails
        {
            Errors = errors
                .GroupBy(error => error.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Description)
                .ToArray()),
        };

        return problemDetails.AddExtensions(errors);
    }
}
