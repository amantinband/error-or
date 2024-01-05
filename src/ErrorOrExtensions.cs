namespace ErrorOr;

public static class ErrorOrExtensions
{
    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <typeparam name="TNextValue">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextValue>> Then<TValue, TNextValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, ErrorOr<TNextValue>> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Then(onValue);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed and its result is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <typeparam name="TNextValue">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextValue>> Then<TValue, TNextValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, TNextValue> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Then(onValue);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided <paramref name="action"/> is invoked.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="action">The action to execute if the state is a value.</param>
    /// <returns>The original <paramref name="errorOr"/>.</returns>
    public static async Task<ErrorOr<TValue>> Then<TValue>(this Task<ErrorOr<TValue>> errorOr, Action<TValue> action)
    {
        var result = await errorOr.ConfigureAwait(false);

        return result.Then(action);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <typeparam name="TNextValue">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextValue>> ThenAsync<TValue, TNextValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task<ErrorOr<TNextValue>>> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ThenAsync(onValue).ConfigureAwait(false);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided function <paramref name="onValue"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <typeparam name="TNextValue">The type of the next result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="onValue">The function to execute if the state is a value.</param>
    /// <returns>The result from calling <paramref name="onValue"/> if state is value; otherwise the original errors.</returns>
    public static async Task<ErrorOr<TNextValue>> ThenAsync<TValue, TNextValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task<TNextValue>> onValue)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ThenAsync(onValue).ConfigureAwait(false);
    }

    /// <summary>
    /// If the state of <paramref name="errorOr"/> is a value, the provided <paramref name="action"/> is executed asynchronously.
    /// </summary>
    /// <typeparam name="TValue">The type of the result.</typeparam>
    /// <param name="errorOr">The error.</param>
    /// <param name="action">The action to execute if the state is a value.</param>
    /// <returns>The original <paramref name="errorOr"/>.</returns>
    public static async Task<ErrorOr<TValue>> ThenAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task> action)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ThenAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    /// If the state is error, the provided function <paramref name="onError"/> is executed asynchronously and its result is returned.
    /// </summary>
    /// <param name="errorOr">The error.</param>
    /// <param name="onError">The function to execute if the state is error.</param>
    /// <returns>The result from calling <paramref name="onError"/> if state is error; otherwise the original value.</returns>
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
    /// <returns>The result from calling <paramref name="onError"/> if state is error; otherwise the original value.</returns>
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
    /// <returns>The result from calling <paramref name="onError"/> if state is error; otherwise the original value.</returns>
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
    /// <returns>The result from calling <paramref name="onError"/> if state is error; otherwise the original value.</returns>
    public static async Task<TValue> ElseAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Task<TValue> onError)
    {
        var result = await errorOr.ConfigureAwait(false);

        return await result.ElseAsync(onError).ConfigureAwait(false);
    }
}
