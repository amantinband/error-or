using System.Diagnostics.CodeAnalysis;

namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public readonly record struct ErrorOr<TValue>
{
    private readonly Error[] _errors;

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Errors))]
    [MemberNotNullWhen(true, nameof(FirstError))]
    public bool IsError { get; }

    /// <summary>Gets a value indicating whether the state is error.</summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Errors))]
    [MemberNotNullWhen(false, nameof(FirstError))]
    public bool IsSuccess => !IsError;

    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    public IReadOnlyList<Error>? Errors => _errors;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public TValue? Value { get; } = default;

    /// <summary>
    /// Gets the first error.
    /// </summary>
    public Error? FirstError => Errors?[0];

    internal ErrorOr(TValue value)
    {
        Value = value;
        _errors = Array.Empty<Error>();
        IsError = false;
    }

    private ErrorOr(Error[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(errors), "Provided list of errors must not be empty");
        }

        _errors = errors;
        IsError = true;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from an errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(Error error) => new(new[] { error });

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from multiple errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(Error error, params Error[] errors)
    {
        var errorArray = new Error[errors.Length + 1];
        errorArray[0] = error;
        for (var i = 0; i < errors.Length; i++)
        {
            errorArray[i + 1] = errors[i];
        }

        return new ErrorOr<TValue>(errorArray);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from an enumerable of errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(IEnumerable<Error> errors) => new(errors.ToArray());

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a value.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(TValue value) => ErrorOr.Ok(value);

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from an error.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(Error error) => Fail(error);

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(List<Error> errors) => Fail(errors);

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static implicit operator ErrorOr<TValue>(Error[] errors) => Fail(errors);

    public void Switch(Action<TValue> onValue, Action<IReadOnlyList<Error>> onError)
    {
        if (IsError)
        {
            onError(Errors);
            return;
        }

        onValue(Value);
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

    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<IReadOnlyList<Error>, TResult> onError)
    {
        if (IsError)
        {
            return onError(Errors);
        }

        return onValue(Value);
    }

    public TResult MatchFirst<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onFirstError)
    {
        if (IsError)
        {
            return onFirstError(FirstError);
        }

        return onValue(Value);
    }
}
