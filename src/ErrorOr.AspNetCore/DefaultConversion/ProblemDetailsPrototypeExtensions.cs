using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ErrorOr;

public static class ProblemDetailsPrototypeExtensions
{
    public static ProblemDetails ConvertToProblemDetails(
        this ProblemDetailsPrototype prototype,
        bool includeErrorMetadata = false) =>
        prototype.LeadingErrorType == ErrorType.Validation ?
            prototype.ConvertToValidationProblemDetails(includeErrorMetadata) :
            prototype.ToErrorProblemDetails();

    public static ProblemDetails ConvertToProblemDetails(
        this ProblemDetailsPrototype prototype,
        HttpContext httpContext,
        ProblemDetailsFactory factory,
        bool includeErrorMetadata = false,
        ModelStateDictionary? modelStateDictionary = null)
    {
        ProblemDetails problemDetails;
        if (prototype.LeadingErrorType == ErrorType.Validation)
        {
            modelStateDictionary ??= new ModelStateDictionary();
            modelStateDictionary.AddErrors(prototype.Errors);
            problemDetails = factory.CreateValidationProblemDetails(
                httpContext,
                modelStateDictionary,
                prototype.StatusCode,
                prototype.Title,
                prototype.Type,
                prototype.Detail,
                prototype.Instance);
        }
        else
        {
            problemDetails = factory.CreateProblemDetails(
                httpContext,
                prototype.StatusCode,
                prototype.Title,
                prototype.Type,
                prototype.Detail,
                prototype.Instance);
        }

        if (includeErrorMetadata)
        {
            problemDetails.AddExtensions(prototype.Errors);
        }

        return problemDetails;
    }

    private static ErrorProblemDetails ToErrorProblemDetails(this ProblemDetailsPrototype prototype)
    {
        return new ErrorProblemDetails(prototype.Errors)
        {
            Status = prototype.StatusCode,
            Type = prototype.Type,
            Title = prototype.Title,
            Instance = prototype.Instance,
            Detail = prototype.Detail ?? "See the errors property for more information.",
        };
    }

    private static ValidationProblemDetails ConvertToValidationProblemDetails(this ProblemDetailsPrototype prototype, bool includeErrorMetadata)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Type = prototype.Type,
            Status = prototype.StatusCode,
            Detail = prototype.Detail,
            Instance = prototype.Instance,
            Errors = prototype
               .Errors
               .GroupBy(error => error.Code)
               .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Description).ToArray()),
        };

        if (prototype.Title is not null)
        {
            problemDetails.Title = prototype.Title;
        }

        if (includeErrorMetadata)
        {
            problemDetails.AddExtensions(prototype.Errors);
        }

        return problemDetails;
    }
}
