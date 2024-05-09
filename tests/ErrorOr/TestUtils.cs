using ErrorOr;

namespace Tests;

public static class Convert
{
    public static ErrorOr<string> ToString(int num) => num.ToString();

    public static ErrorOr<int> ToInt(string str) => int.Parse(str);

    public static Task<ErrorOr<int>> ToIntAsync(string str) => Task.FromResult(ErrorOrFactory.From(int.Parse(str)));

    public static Task<ErrorOr<string>> ToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));
}
