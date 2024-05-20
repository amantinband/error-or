namespace ErrorOr.IntegrationTests;

public sealed record ValidationProblemDetails : ProblemDetails
{
    public Dictionary<string, string[]>? Errors { get; init; }

    public bool Equals(ValidationProblemDetails? other)
    {
        if (!base.Equals(other))
        {
            return false;
        }

        if (Errors is null)
        {
            return other.Errors is null;
        }

        return other.Errors is not null && CompareErrors(Errors, other.Errors);
    }

    public override int GetHashCode()
    {
        if (Errors is null)
        {
            return base.GetHashCode();
        }

#pragma warning disable SA1129
        var hashCodeBuilder = new HashCode();
#pragma warning restore SA1129
        hashCodeBuilder.Add(base.GetHashCode());

        foreach (var error in Errors)
        {
            hashCodeBuilder.Add(error.Key);
            foreach (var value in error.Value)
            {
                hashCodeBuilder.Add(value);
            }
        }

        return hashCodeBuilder.ToHashCode();
    }

    private static bool CompareErrors(Dictionary<string, string[]> errors, Dictionary<string, string[]> otherErrors)
    {
        if (errors.Count != otherErrors.Count)
        {
            return false;
        }

        foreach (var (key, value) in errors)
        {
            if (!otherErrors.TryGetValue(key, out var otherValue))
            {
                return false;
            }

            if (!value.SequenceEqual(otherValue))
            {
                return false;
            }
        }

        return true;
    }
}
