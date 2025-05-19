using OneOf;

namespace Task_Manager.Common;

public interface IError;

public sealed record OneOfError<TError1, TError2>(
    OneOf<TError1, TError2> Value
) : IError
    where TError1 : IError
    where TError2 : IError
{
    public static implicit operator OneOfError<TError1, TError2>(TError1 error)
        => new(OneOf<TError1, TError2>.FromT0(error));

    public static implicit operator OneOfError<TError1, TError2>(TError2 error)
        => new(OneOf<TError1, TError2>.FromT1(error));
}

public abstract class ResultBase<TError>
    where TError : IError
{
    protected TError? _error;

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public TError? Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Result is successful.");
        protected set => _error = value;
    }

    protected ResultBase(bool isSuccess, TError? error)
        => (IsSuccess, Error) = (isSuccess, error);
}

public sealed class Result<TError> : ResultBase<TError>
    where TError : IError
{
    private Result(bool isSuccess, TError? error)
        : base(isSuccess, error) { }

    public static Result<TError> Success()
        => new(true, default);

    public static Result<TError> Failure(TError error)
        => new(false, error);

    public Result<TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        where TNewError : IError
        => IsSuccess
            ? Result<TNewError>.Success()
            : Result<TNewError>.Failure(map(Error!));

    public static implicit operator Result<TError>(TError error) => Failure(error);
}

public abstract class ResultBase<T, TError>
    where TError : IError
{
    protected T? _value;
    protected TError? _error;

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public T? Value
    {
        get => IsSuccess ? _value : throw new InvalidOperationException("Result is not successful.");
        protected set => _value = value;
    }
    public TError? Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Result is successful.");
        protected set => _error = value;
    }

    protected ResultBase(bool isSuccess, T? value, TError? error)
        => (IsSuccess, Value, Error) = (isSuccess, value, error);
}

public sealed class Result<T, TError> : ResultBase<T, TError>
    where TError : IError
{
    private Result(bool isSuccess, T? value, TError? error)
        : base(isSuccess, value, error)
    { }

    public static Result<T, TError> Success(T value)
        => new(true, value, default);

    public static Result<T, TError> Failure(TError error)
        => new(false, default, error);

    public static implicit operator Result<T, TError>(T value) => Success(value);

    public static implicit operator Result<T, TError>(TError error) => Failure(error);

    public static explicit operator T(Result<T, TError> result) => result.Value!;

    public Result<TResult, TError> Map<TResult>(Func<T, TResult> mapper) =>
        IsSuccess ? Result<TResult, TError>.Success(mapper(Value!)) : Result<TResult, TError>.Failure(Error!);

    public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        where TNewError : IError
        => IsSuccess
            ? Result<T, TNewError>.Success(Value!)
            : Result<T, TNewError>.Failure(map(Error!));

    public Result<TResult, TError> Bind<TResult>(Func<T, Result<TResult, TError>> binder) =>
        IsSuccess ? binder(Value!) : Result<TResult, TError>.Failure(Error!);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);

    public Result<T, TError> Tap(Action<T> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(Value!);
        }

        return this;
    }
}
