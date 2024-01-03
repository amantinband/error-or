namespace ErrorOr;

public static class ErrorOrExtensions
{
    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TNextResult">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextResult>> Then<TResult, TNextResult>(this Task<ErrorOr<TResult>> errorOr, Func<TResult, ErrorOr<TNextResult>> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Then(onValue);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TNextResult">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextResult>> ThenAsync<TResult, TNextResult>(this Task<ErrorOr<TResult>> errorOr, Func<TResult, Task<ErrorOr<TNextResult>>> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ThenAsync(onValue).ConfigureAwait(false);
    }

    /// <summary>
    /// If the state is error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <param name="errorOr">The error.</param>
    /// <param name="onError">The function to execute if the state is error.</param>
    /// <returns>The result from calling <paramref name="onError"/> if state is value; otherwise the original errors.</returns>
    public static async Task<TValue> Else<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<List<Error>, TValue> onError)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Else(onError);
    }

    /// <summary>
    /// If the state is error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <param name="errorOr">The error.</param>
    /// <param name="onError">The function to execute if the state is error.</param>
    /// <returns>The result from calling <paramref name="onError"/> if state is value; otherwise the original errors.</returns>
    public static async Task<TValue> Else<TValue>(this Task<ErrorOr<TValue>> errorOr, TValue onError)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Else(onError);
    }

    /// <summary>
    /// If the state is error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <param name="errorOr">The error.</param>
    /// <param name="onError">The function to execute if the state is error.</param>
    /// <returns>The result from calling <paramref name="onError"/> if state is value; otherwise the original errors.</returns>
    public static async Task<TValue> ElseAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<List<Error>, Task<TValue>> onError)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ElseAsync(onError).ConfigureAwait(false);
    }

    /// <summary>
    /// If the state is error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <param name="errorOr">The error.</param>
    /// <param name="onError">The function to execute if the state is error.</param>
    /// <returns>The result from calling <paramref name="onError"/> if state is value; otherwise the original errors.</returns>
    public static async Task<TValue> ElseAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Task<TValue> onError)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ElseAsync(onError).ConfigureAwait(false);
    }
}
