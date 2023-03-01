namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public record struct ErrorOr<TValue> : IErrorOr
{
    private static readonly Error NoFirstError = Error.Unexpected(
        code: "ErrorOr.NoFirstError",
        description: "First error cannot be retrieved from a successful ErrorOr.");

    private static readonly Error NoErrors = Error.Unexpected(
        code: "ErrorOr.NoErrors",
        description: "Error list cannot be retrieved from a successful ErrorOr.");

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError { get; init; }

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will contain a single error representing the state.
    /// </summary>
    public List<Error> Errors { get; init; }

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will be empty.
    /// </summary>
    public List<Error> ErrorsOrEmptyList { get; init; }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static ErrorOr<TValue> From(List<Error> errors)
    {
        return errors;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public TValue Value { get; init; } = default!;

    /// <summary>
    /// Gets the first error.
    /// </summary>
    public Error FirstError
    {
        get
        {
            if (!IsError)
            {
                return NoFirstError;
            }

            return Errors![0];
        }
    }

    private ErrorOr(Error error)
    {
        Errors = new List<Error> { error };
        ErrorsOrEmptyList = Errors;
        IsError = true;
    }

    private ErrorOr(List<Error> errors)
    {
        Errors = errors;
        ErrorsOrEmptyList = errors;
        IsError = true;
    }

    private ErrorOr(TValue value)
    {
        Value = value;
        IsError = false;
        Errors = new List<Error> { NoErrors };
        ErrorsOrEmptyList = new();
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a value.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(TValue value)
    {
        return new ErrorOr<TValue>(value);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from an error.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(Error error)
    {
        return new ErrorOr<TValue>(error);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(List<Error> errors)
    {
        return new ErrorOr<TValue>(errors);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(Error[] errors)
    {
        return new ErrorOr<TValue>(errors.ToList());
    }

    public void Switch(Action<TValue> onValue, Action<List<Error>> onError)
    {
        if (IsError)
        {
            onError(Errors);
            return;
        }

        onValue(Value);
    }

    public async Task SwitchAsync(Func<TValue, Task> onValue, Func<List<Error>, Task> onError)
    {
        if (IsError)
        {
            await onError(Errors).ConfigureAwait(false);
            return;
        }

        await onValue(Value).ConfigureAwait(false);
    }

    public void SwitchFirst(Action<TValue> onValue, Action<Error> onFirstError)
    {
        if (IsError)
        {
            onFirstError(FirstError);
            return;
        }

        onValue(Value);
    }

    public async Task SwitchFirstAsync(Func<TValue, Task> onValue, Func<Error, Task> onFirstError)
    {
        if (IsError)
        {
            await onFirstError(FirstError).ConfigureAwait(false);
            return;
        }

        await onValue(Value).ConfigureAwait(false);
    }

    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<List<Error>, TResult> onError)
    {
        if (IsError)
        {
            return onError(Errors);
        }

        return onValue(Value);
    }

    public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<List<Error>, Task<TResult>> onError)
    {
        if (IsError)
        {
            return await onError(Errors).ConfigureAwait(false);
        }

        return await onValue(Value).ConfigureAwait(false);
    }

    public TResult MatchFirst<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onFirstError)
    {
        if (IsError)
        {
            return onFirstError(FirstError);
        }

        return onValue(Value);
    }

    public async Task<TResult> MatchFirstAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<Error, Task<TResult>> onFirstError)
    {
        if (IsError)
        {
            return await onFirstError(FirstError).ConfigureAwait(false);
        }

        return await onValue(Value).ConfigureAwait(false);
    }
}

public static class ErrorOr
{
    public static ErrorOr<TValue> From<TValue>(TValue value)
    {
        return value;
    }
}
