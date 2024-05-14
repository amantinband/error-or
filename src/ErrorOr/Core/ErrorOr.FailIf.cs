namespace ErrorOr;

public readonly partial record struct ErrorOr<TValue> : IErrorOr<TValue>
{
    /// <summary>
    /// If the state is value, the provided function <paramref name="onValue"/> is invoked.
    /// If <paramref name="onValue"/> returns true, the given <paramref name="error"/> will be returned, and the state will be error.
    /// </summary>
    /// <param name="onValue">The function to execute if the state is value.</param>
    /// <param name="error">The <see cref="Error"/> to return if the given <paramref name="onValue"/> function returned true.</param>
    /// <returns>The given <paramref name="error"/> if <paramref name="onValue"/> returns true; otherwise, the original <see cref="ErrorOr"/> instance.</returns>
    public ErrorOr<TValue> FailIf(Func<TValue, bool> onValue, Error error)
    {
        if (IsError)
        {
            return this;
        }

        return onValue(Value) ? error : this;
    }

    /// <summary>
    /// If the state is value, the provided function <paramref name="onValue"/> is invoked asynchronously.
    /// If <paramref name="onValue"/> returns true, the given <paramref name="error"/> will be returned, and the state will be error.
    /// </summary>
    /// <param name="onValue">The function to execute if the statement is value.</param>
    /// <param name="error">The <see cref="Error"/> to return if the given <paramref name="onValue"/> function returned true.</param>
    /// <returns>The given <paramref name="error"/> if <paramref name="onValue"/> returns true; otherwise, the original <see cref="ErrorOr"/> instance.</returns>
    public async Task<ErrorOr<TValue>> FailIfAsync(Func<TValue, Task<bool>> onValue, Error error)
    {
        if (IsError)
        {
            return this;
        }

        return await onValue(Value).ConfigureAwait(false) ? error : this;
    }
}
