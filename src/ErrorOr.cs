namespace ErrorOr;

/// <summary>
/// A discriminated union of errors or a value.
/// </summary>
public record struct ErrorOr<TResult>
{
    private readonly TResult? _result = default;
    private readonly List<Error>? _errors = null;

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
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

            return _errors!.AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public TResult Result
    {
        get
        {
            if (IsError)
            {
                throw new InvalidOperationException("Result can be retrieved only when the result not an error.");
            }

            return _result!;
        }
    }

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

    private ErrorOr(TResult result)
    {
        _result = result;
        IsError = false;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TResult}"/> from a result.
    /// </summary>
    public static implicit operator ErrorOr<TResult>(TResult result)
    {
        return new ErrorOr<TResult>(result);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TResult}"/> from an error.
    /// </summary>
    public static implicit operator ErrorOr<TResult>(Error error)
    {
        return new ErrorOr<TResult>(error);
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TResult}"/> from a list of errors.
    /// </summary>
    public static implicit operator ErrorOr<TResult>(List<Error> errors)
    {
        return new ErrorOr<TResult>(errors);
    }
}
