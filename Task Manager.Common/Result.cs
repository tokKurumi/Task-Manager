namespace Task_Manager.Common;

public abstract record Error(string Code);

public abstract class ResultBase<TError>
    where TError : Error
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
    where TError : Error
{
    private Result(bool isSuccess, TError? error)
        : base(isSuccess, error) { }

    public static Result<TError> Success()
        => new(true, default);

    public static Result<TError> Failure(TError error)
        => new(false, error);

    public static implicit operator Result<TError>(TError error) => Failure(error);

    public static implicit operator bool(Result<TError> result) => result.IsSuccess;
}

public sealed class Result<T, TError> : ResultBase<TError>
    where TError : Error
{
    private T? _value;

    public T? Value
    {
        get => IsSuccess ? _value : throw new InvalidOperationException("Result is not successful.");
        private set => _value = value;
    }

    private Result(bool isSuccess, T? value, TError? error)
        : base(isSuccess, error) => Value = value;

    public static Result<T, TError> Success(T value)
        => new(true, value, default);

    public static Result<T, TError> Failure(TError error)
        => new(false, default, error);

    public static implicit operator Result<T, TError>(T value) => Success(value);

    public static implicit operator Result<T, TError>(TError error) => Failure(error);

    public static explicit operator T(Result<T, TError> result) => result.Value!;

    public Result<TResult, TError> Map<TResult>(Func<T, TResult> mapper) =>
        IsSuccess ? Result<TResult, TError>.Success(mapper(Value!)) : Result<TResult, TError>.Failure(Error!);

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
