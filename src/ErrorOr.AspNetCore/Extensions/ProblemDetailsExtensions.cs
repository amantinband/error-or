using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails AddExtensions(
        this ProblemDetails problemDetails,
        List<Error> errors)
    {
        foreach (var error in errors)
        {
            problemDetails.AddExtensions(error);
        }

        return problemDetails;
    }

    public static ProblemDetails AddExtensions(
        this ProblemDetails problemDetails,
        Error error)
    {
        foreach (var metadata in error.Metadata)
        {
            problemDetails.Extensions.Add(metadata.Key, metadata.Value);
        }

        return problemDetails;
    }
}
