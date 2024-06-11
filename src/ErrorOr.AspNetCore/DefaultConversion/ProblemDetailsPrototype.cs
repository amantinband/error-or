namespace ErrorOr;

public readonly record struct ProblemDetailsPrototype(
    List<Error> Errors,
    ErrorType LeadingErrorType,
    int? StatusCode = null,
    string? Title = null,
    string? Detail = null,
    string? Type = null,
    string? Instance = null
)
{
    public static ProblemDetailsPrototype CreateDefaultFromErrors(
        List<Error> errors,
        Dictionary<ErrorType, ProblemDetailInfo>? errorDefaults = null,
        bool useFirstErrorAsLeadingType = false)
    {
        errorDefaults ??= ErrorDefaults.DefaultMappings;
        var leadingErrorType = errors.GetLeadingErrorType(useFirstErrorAsLeadingType);
        if (!errorDefaults.TryGetValue(leadingErrorType, out var problemDetailInfo))
        {
            problemDetailInfo = ErrorDefaults.Failure;
        }

        return new ProblemDetailsPrototype(
            errors,
            problemDetailInfo.ErrorType,
            problemDetailInfo.StatusCode,
            problemDetailInfo.Title,
            Type: problemDetailInfo.Type);
    }
}
