using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public class ErrorOrOptions
{
    public static ErrorOrOptions Instance { get; } = new();

    public bool IncludeMetadata { get; set; }
    public bool UseProblemDetailsFactoryInMvc { get; set; }
    public bool UseFirstErrorAsLeadingType { get; set; }
    public Func<List<Error>, IActionResult>? CustomToErrorActionResult { get; set; }
    public Func<List<Error>, IResult>? CustomToErrorResult { get; set; }
    public Func<List<Error>, ProblemDetailsPrototype>? CustomCreatePrototype { get; set; }
    public Dictionary<ErrorType, ProblemDetailInfo> ErrorDefaults { get; set; } =
        new (ErrorOr.ErrorDefaults.DefaultMappings);
}
