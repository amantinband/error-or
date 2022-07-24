namespace ErrorOr;

public static class ErrorOr
{
    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from a value.
    /// </summary>
    public static ErrorOr<TValue> Ok<TValue>(TValue value) => new(value);
}
