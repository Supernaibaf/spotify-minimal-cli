using System.Diagnostics.CodeAnalysis;

namespace SpotifyMinimalCli;

public readonly struct Result<TSuccess, TError> : IEquatable<Result<TSuccess, TError>>
{
    public TSuccess? Value { get; private init; }

    public TError? Error { get; private init; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    public Result(TSuccess value)
    {
        IsSuccess = true;
        Value = value;
    }

    public Result(TError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public readonly bool Equals(Result<TSuccess, TError> other)
    {
        return EqualityComparer<TSuccess?>.Default.Equals(Value, other.Value) &&
               EqualityComparer<TError?>.Default.Equals(Error, other.Error) && IsSuccess == other.IsSuccess;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Result<TSuccess, TError> other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Value, Error, IsSuccess);
    }

    public static bool operator ==(Result<TSuccess, TError> left, Result<TSuccess, TError> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<TSuccess, TError> left, Result<TSuccess, TError> right)
    {
        return !(left == right);
    }

    public static implicit operator Result<TSuccess, TError>(TSuccess value)
    {
        return new Result<TSuccess, TError>(value);
    }

    public static implicit operator Result<TSuccess, TError>(TError error)
    {
        return new Result<TSuccess, TError>(error);
    }

    public Result<TSuccess, TError> ToResult(TSuccess value)
    {
        return new Result<TSuccess, TError>(value);
    }

    public Result<TSuccess, TError> ToResult(TError error)
    {
        return new Result<TSuccess, TError>(error);
    }

    public Result<TNewSuccess, TNewError> Map<TNewSuccess, TNewError>(
        Func<TSuccess, TNewSuccess> mapSuccess,
        Func<TError, TNewError> mapError)
    {
        if (IsSuccess)
        {
            return mapSuccess(Value);
        }

        return mapError(Error);
    }
}

public static class Result
{
    public static Result<TSuccess, TError> Success<TSuccess, TError>(TSuccess value)
    {
        return new Result<TSuccess, TError>(value);
    }

    public static Result<TSuccess, TError> Failure<TSuccess, TError>(TError error)
    {
        return new Result<TSuccess, TError>(error);
    }
}