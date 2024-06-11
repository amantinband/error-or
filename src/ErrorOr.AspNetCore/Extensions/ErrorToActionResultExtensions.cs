using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorOr;

public static class ErrorToActionResultExtensions
{
    public static IActionResult CreateErrorActionResult(
        this ControllerBase controller,
        Error error,
        ErrorOrOptions? options = null) =>
        error.ToErrorActionResult(controller.HttpContext, options);

    public static IActionResult CreateErrorActionResult(
        this ControllerBase controller,
        List<Error> errors,
        ErrorOrOptions? options = null) =>
        errors.ToErrorActionResult(controller.HttpContext, options);

    public static IActionResult ToErrorActionResult(
        this Error error,
        HttpContext? httpContext = null,
        ErrorOrOptions? options = null) =>
        ToErrorActionResult([error], httpContext, options);

    public static IActionResult ToErrorActionResult(
        this List<Error> errors,
        HttpContext? httpContext = null,
        ErrorOrOptions? options = null)
    {
        options ??= ErrorOrOptions.Instance;
        if (options.CustomToErrorActionResult is not null)
        {
            return options.CustomToErrorActionResult(errors);
        }

        var prototype =
            options.CustomCreatePrototype?.Invoke(errors) ??
            ProblemDetailsPrototype.CreateDefaultFromErrors(
                errors,
                options.ErrorDefaults,
                options.UseFirstErrorAsLeadingType);

        ProblemDetails problemDetails;
        if (options.UseProblemDetailsFactoryInMvc &&
            httpContext?.RequestServices.GetService<ProblemDetailsFactory>() is { } factory)
        {
            problemDetails = prototype.ConvertToProblemDetails(
                httpContext,
                factory,
                options.IncludeMetadata);
        }
        else
        {
            problemDetails = prototype.ConvertToProblemDetails(options.IncludeMetadata);
        }

        return problemDetails.CreateDefaultErrorActionResult();
    }

    public static IActionResult CreateDefaultErrorActionResult(this ProblemDetails problemDetails)
    {
        return problemDetails.Status switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(problemDetails),
            StatusCodes.Status401Unauthorized => new UnauthorizedObjectResult(problemDetails),

            // TODO: ForbidResult does not support ProblemDetails - should we process it differently?
            StatusCodes.Status404NotFound => new NotFoundObjectResult(problemDetails),
            StatusCodes.Status409Conflict => new ConflictObjectResult(problemDetails),
            StatusCodes.Status422UnprocessableEntity => new UnprocessableEntityObjectResult(problemDetails),
            _ => new ObjectResult(problemDetails),
        };
    }
}
