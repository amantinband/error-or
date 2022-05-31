namespace ErrorOr;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Error types.
/// </summary>
public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
}
