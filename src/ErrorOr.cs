namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public readonly record struct ErrorOr<TValue> : IErrorOr<TValue>
{
    private readonly TValue? _value = default;
    private readonly List<Error>? _errors = null;

    private static readonly Error NoFirstError = Error.Unexpected(
        code: "ErrorOr.NoFirstError",
        description: "First error cannot be retrieved from a successful ErrorOr.");

    private static readonly Error NoErrors = Error.Unexpected(
        code: "ErrorOr.NoErrors",
        description: "Error list cannot be retrieved from a successful ErrorOr.");

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will contain a single error representing the state.
    /// </summary>
    public List<Error> Errors => IsError ? _errors! : new List<Error> { NoErrors };

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will be empty.
    /// </summary>
    public List<Error> ErrorsOrEmptyList => IsError ? _errors! : new();

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
    public TValue Value => _value!;

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

            return _errors![0];
        }
    }

    private ErrorOr(Error error)
    {
        _errors = new List<Error> { error };
        IsError = true;
    }

    private ErrorOr(List<Error> errors)
    {
        _errors = errors;
        IsError = true;
    }

    private ErrorOr(TValue value)
    {
        _value = value;
        IsError = false;
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

    /// <summary>
    /// Executes the appropriate action based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is an error, the provided action <paramref name="onError"/> is executed.
    /// If the state is a value, the provided action <paramref name="onValue"/> is executed.
    /// </summary>
    /// <param name="onValue">The action to execute if the state is a value.</param>
    /// <param name="onError">The action to execute if the state is an error.</param>
    public void Switch(Action<TValue> onValue, Action<List<Error>> onError)
    {
        if (IsError)
        {
            onError(Errors);
            return;
        }

        onValue(Value);
    }

    /// <summary>
    /// Asynchronously executes the appropriate action based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is an error, the provided action <paramref name="onError"/> is executed asynchronously.
    /// If the state is a value, the provided action <paramref name="onValue"/> is executed asynchronously.
    /// </summary>
    /// <param name="onValue">The asynchronous action to execute if the state is a value.</param>
    /// <param name="onError">The asynchronous action to execute if the state is an error.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SwitchAsync(Func<TValue, Task> onValue, Func<List<Error>, Task> onError)
    {
        if (IsError)
        {
            await onError(Errors).ConfigureAwait(false);
            return;
        }

        await onValue(Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the appropriate action based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is an error, the provided action <paramref name="onFirstError"/> is executed using the first error as input.
    /// If the state is a value, the provided action <paramref name="onValue"/> is executed.
    /// </summary>
    /// <param name="onValue">The action to execute if the state is a value.</param>
    /// <param name="onFirstError">The action to execute with the first error if the state is an error.</param>
    public void SwitchFirst(Action<TValue> onValue, Action<Error> onFirstError)
    {
        if (IsError)
        {
            onFirstError(FirstError);
            return;
        }

        onValue(Value);
    }

    /// <summary>
    /// Asynchronously executes the appropriate action based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is an error, the provided action <paramref name="onFirstError"/> is executed asynchronously using the first error as input.
    /// If the state is a value, the provided action <paramref name="onValue"/> is executed asynchronously.
    /// </summary>
    /// <param name="onValue">The asynchronous action to execute if the state is a value.</param>
    /// <param name="onFirstError">The asynchronous action to execute with the first error if the state is an error.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SwitchFirstAsync(Func<TValue, Task> onValue, Func<Error, Task> onFirstError)
    {
        if (IsError)
        {
            await onFirstError(FirstError).ConfigureAwait(false);
            return;
        }

        await onValue(Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the appropriate function based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/> is executed and its result is returned.
    /// If the state is an error, the provided function <paramref name="onError"/> is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <param name="onError">The function to execute if the state is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<List<Error>, TResult> onError)
    {
        if (IsError)
        {
            return onError(Errors);
        }

        return onValue(Value);
    }

    /// <summary>
    /// Asynchronously executes the appropriate function based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// If the state is an error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The asynchronous function to execute if the state is a value.</param>
    /// <param name="onError">The asynchronous function to execute if the state is an error.</param>
    /// <returns>A task representing the asynchronous operation that yields the result of the executed function.</returns>
    public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<List<Error>, Task<TResult>> onError)
    {
        if (IsError)
        {
            return await onError(Errors).ConfigureAwait(false);
        }

        return await onValue(Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the appropriate function based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/> is executed and its result is returned.
    /// If the state is an error, the provided function <paramref name="onFirstError"/> is executed using the first error, and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <param name="onFirstError">The function to execute with the first error if the state is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult MatchFirst<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onFirstError)
    {
        if (IsError)
        {
            return onFirstError(FirstError);
        }

        return onValue(Value);
    }

    /// <summary>
    /// Asynchronously executes the appropriate function based on the state of the <see cref="ErrorOr{TValue}"/>.
    /// If the state is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// If the state is an error, the provided function <paramref name="onFirstError"/> is executed asynchronously using the first error, and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The asynchronous function to execute if the state is a value.</param>
    /// <param name="onFirstError">The asynchronous function to execute with the first error if the state is an error.</param>
    /// <returns>A task representing the asynchronous operation that yields the result of the executed function.</returns>
    public async Task<TResult> MatchFirstAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<Error, Task<TResult>> onFirstError)
    {
        if (IsError)
        {
            return await onFirstError(FirstError).ConfigureAwait(false);
        }

        return await onValue(Value).ConfigureAwait(false);
    }
}

/// <summary>
/// Provides utility methods for creating instances of <see ref="ErrorOr{T}"/>.
/// </summary>
public static class ErrorOr
{
    /// <summary>
    /// Creates an <see ref="ErrorOr{TValue}"/> instance from a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to create an ErrorOr instance.</param>
    /// <returns>An <see ref="ErrorOr{TValue}"/> instance containing the specified value.</returns>
    public static ErrorOr<TValue> From<TValue>(TValue value)
    {
        return value;
    }
}
