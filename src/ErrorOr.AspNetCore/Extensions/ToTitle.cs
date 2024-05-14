using Microsoft.AspNetCore.Http;

namespace ErrorOr;

public static class ErrorToTitleExtensions
{
    public static string ToTitle(this Error error)
    {
        foreach (var mapping in ErrorOrOptions.Instance.ErrorToTitleMapper)
        {
            if (mapping(error) is string title)
            {
                return title;
            }
        }

        return error.Description;
    }
}
