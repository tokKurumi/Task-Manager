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
}
