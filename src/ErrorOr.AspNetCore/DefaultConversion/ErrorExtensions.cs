namespace ErrorOr;

public static class ErrorExtensions
{
    public static ErrorType GetLeadingErrorType(this List<Error> errors, bool firstTypeIsLeadingType = false)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0)
        {
            throw new ArgumentException("errors must have at least one item", nameof(errors));
        }

        var firstType = errors[0].Type;
        if (firstTypeIsLeadingType || errors.Count == 1)
        {
            return firstType;
        }

        for (var i = 1; i < errors.Count; i++)
        {
            if (firstType != errors[i].Type)
            {
                return ErrorType.Failure;
            }
        }

        return firstType;
    }
}
