using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public class ErrorProblemDetails : ProblemDetails
{
    /// <summary>
    /// Initializes a new instance of <see cref="ErrorProblemDetails" />.
    /// </summary>
    /// <param name="errors">The dictionary that contains the errors for the problem details.</param>
    public ErrorProblemDetails(List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count is 0)
        {
            throw new ArgumentException("The errors list must contain at least one error.", nameof(errors));
        }

        Errors = errors;
    }

    /// <summary>
    /// Gets the errors associated with this instance of <see cref="ErrorProblemDetails" />.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<Error> Errors { get; }
}
