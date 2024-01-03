namespace ErrorOr;

public static class ErrorOrExtensions
{
    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TNextResult">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextResult>> ChainAsync<TResult, TNextResult>(this Task<ErrorOr<TResult>> errorOr, Func<TResult, Task<ErrorOr<TNextResult>>> onValue)
    {
        var result = await errorOr;

        return await result.ChainAsync(onValue).ConfigureAwait(false);
    }
}
