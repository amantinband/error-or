using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErrorOr;

public class ErrorOrOptions
{
    public static readonly ErrorOrOptions Instance = new();

    public bool IncludeMetadataInProblemDetails { get; set; } = false;
    public List<Func<List<Error>, IResult?>> ErrorListToResultMapper { get; set; } = [];
    public List<Func<Error, IResult?>> ErrorToResultMapper { get; set; } = [];
    public List<Func<List<Error>, IActionResult?>> ErrorListToActionResultMapper { get; set; } = [];
    public List<Func<Error, IActionResult?>> ErrorToActionResultMapper { get; set; } = [];
    public List<Func<List<Error>, ProblemDetails?>> ErrorListToProblemDetailsMapper { get; set; } = [];
    public List<Func<Error, ProblemDetails?>> ErrorToProblemDetailsMapper { get; set; } = [];
    public List<Func<Error, int?>> ErrorToStatusCodeMapper { get; set; } = [];
    public List<Func<Error, string?>> ErrorToTitleMapper { get; set; } = [];
}
