namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public readonly partial record struct ErrorOr<TValue> : IErrorOr<TValue>
{
    private readonly TValue? _value = default;
    private readonly List<Error>? _errors = null;

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will contain a single error representing the state.
    /// </summary>
    public List<Error> Errors => IsError ? _errors! : KnownErrors.CachedNoErrorsList;

    /// <summary>
    /// Gets the list of errors. If the state is not error, the list will be empty.
    /// </summary>
    public List<Error> ErrorsOrEmptyList => IsError ? _errors! : KnownErrors.CachedEmptyErrorsList;

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
                return KnownErrors.NoFirstError;
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
}
