namespace ErrorOr.IntegrationTests;

public sealed record ErrorProblemDetails : ProblemDetails
{
    public List<Error>? Errors { get; init; }

    public bool Equals(ErrorProblemDetails? other)
    {
        if (!base.Equals(other))
        {
            return false;
        }

        if (Errors is null)
        {
            return other.Errors is null;
        }

        return other.Errors is not null && Errors.SequenceEqual(other.Errors);
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
            hashCodeBuilder.Add(error);
        }

        return hashCodeBuilder.ToHashCode();
    }
}
