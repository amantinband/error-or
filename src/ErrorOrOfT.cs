using System.Diagnostics.CodeAnalysis;

namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public readonly record struct ErrorOr<TValue>
{
    private readonly List<Error> _errors;

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsError { get; }

    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    public IReadOnlyList<Error> Errors
    {
        get
        {
            if (!IsError)
            {
                throw new InvalidOperationException("Errors can be retrieved only when the result is an error.");
            }

            return _errors!;
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public TValue? Value { get; } = default;

    /// <summary>
    /// Gets the first error.
    /// </summary>
    public Error FirstError
    {
        get
        {
            if (!IsError)
            {
                throw new InvalidOperationException();
            }

            return _errors[0];
        }
    }

    internal ErrorOr(TValue value)
    {
        Value = value;
        _errors = new List<Error>();
        IsError = false;
    }

    private ErrorOr(List<Error> errors)
    {
        _errors = errors;
        IsError = true;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from an errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(Error error) => new(new List<Error> { error });

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from multiple errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(Error error, params Error[] errors)
    {
        var errorList = new List<Error> { error };
        errorList.AddRange(errors);
        return new ErrorOr<TValue>(errorList);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}" /> from an enumerable of errors.
    /// </summary>
    public static ErrorOr<TValue> Fail(IEnumerable<Error> errors) => new(errors.ToList());

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
    public static implicit operator ErrorOr<TValue>(Error[] errors) => Fail(errors.ToList());

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
