namespace ErrorOr;

public record struct Success;
public record struct Created;
public record struct Deleted;
public record struct Updated;

public static class Result
{
    public static Success Success => default;

    public static Created Created => default;

    public static Deleted Deleted => default;

    public static Updated Updated => default;
}
