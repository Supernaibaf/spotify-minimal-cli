using System.Diagnostics.CodeAnalysis;

namespace SpotifyMinimalCli;

public readonly struct VoidResult<TError> : IEquatable<VoidResult<TError>>
{
    public TError? Error { get; private init; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    public VoidResult()
    {
        IsSuccess = true;
    }

    public VoidResult(TError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool Equals(VoidResult<TError> other)
    {
        return EqualityComparer<TError?>.Default.Equals(Error, other.Error) && IsSuccess == other.IsSuccess;
    }

    public override bool Equals(object? obj)
    {
        return obj is VoidResult<TError> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Error, IsSuccess);
    }

    public static bool operator ==(VoidResult<TError> left, VoidResult<TError> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VoidResult<TError> left, VoidResult<TError> right)
    {
        return !(left == right);
    }

    public static implicit operator VoidResult<TError>(TError error)
    {
        return new VoidResult<TError>(error);
    }

    public VoidResult<TError> ToVoidResult(TError error)
    {
        return new VoidResult<TError>(error);
    }

    public VoidResult<TNewError> Map<TNewError>(
        Func<TError, TNewError> mapError)
    {
        if (IsSuccess)
        {
            return new VoidResult<TNewError>();
        }

        return mapError(Error);
    }
}

public static class VoidResult
{
    public static VoidResult<TError> Success<TError>()
    {
        return new VoidResult<TError>();
    }

    public static VoidResult<TError> Failure<TError>(TError error)
    {
        return new VoidResult<TError>(error);
    }
}