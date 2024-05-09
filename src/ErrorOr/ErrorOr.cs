using System.Diagnostics.CodeAnalysis;

namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
/// <typeparam name="TValue">The type of the underlying <see cref="Value"/>.</typeparam>
public readonly partial record struct ErrorOr<TValue> : IErrorOr<TValue>
{
    private readonly TValue? _value = default;
    private readonly List<Error>? _errors = null;

    /// <summary>
    /// Prevents a default <see cref="ErrorOr"/> struct from being created.
    /// </summary>
    public ErrorOr()
    {
        throw new InvalidOperationException("Default construction of ErrorOr<TValue> is invalid. Please use provided factory methods to instantiate.");
    }

    private ErrorOr(Error error)
    {
        _errors = [error];
    }

    private ErrorOr(List<Error> errors)
    {
        _errors = errors;
    }

    private ErrorOr(TValue value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_errors))]
    [MemberNotNullWhen(true, nameof(Errors))]
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(false, nameof(_value))]
    public bool IsError => _errors is not null;

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will contain a single error representing the state.
    /// </summary>
    public List<Error> Errors => IsError ? _errors : throw new InvalidOperationException("The Errors property cannot be accessed when no errors have been recorded. Check IsError before accessing Errors.");

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will be empty.
    /// </summary>
    public List<Error> ErrorsOrEmptyList => IsError ? _errors : EmptyErrors.Instance;

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
                throw new InvalidOperationException("The FirstError property cannot be accessed when no errors have been recorded. Check IsError before accessing FirstError.");
            }

            return _errors[0];
        }
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a list of errors.
    /// </summary>
    public static ErrorOr<TValue> From(List<Error> errors)
    {
        return errors;
    }
}
