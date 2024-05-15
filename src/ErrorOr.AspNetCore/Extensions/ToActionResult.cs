using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorOr;

public static class ErrorToActionResultExtensions
{
    public static IActionResult ToActionResult(this Error error, HttpContext? httpContext = null)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorToActionResultMapper)
        {
            if (mapping(error) is IActionResult actionResult)
            {
                return actionResult;
            }
        }

        return ToActionResult([error], httpContext);
    }

    public static IActionResult ToActionResult(this List<Error> errors, HttpContext? httpContext = null)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorListToActionResultMapper)
        {
            if (mapping(errors) is IActionResult actionResult)
            {
                return actionResult;
            }
        }

        var problemDetails = errors.ToProblemDetails();

        if (httpContext?.RequestServices.GetService<ProblemDetailsFactory>() is ProblemDetailsFactory factory)
        {
            problemDetails = factory.CreateProblemDetails(
                httpContext,
                problemDetails.Status,
                problemDetails.Title,
                problemDetails.Type,
                problemDetails.Detail,
                problemDetails.Instance);
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
        };
    }
}
