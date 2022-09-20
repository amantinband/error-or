namespace ErrorOr;

public interface IErrorOr
{
    IReadOnlyList<Error>? Errors { get; }
}
