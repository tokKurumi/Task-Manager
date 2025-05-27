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

public sealed class Result<TError>
    where TError : IError
{
    private TError? _error;

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public TError? Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Result is successful.");
        private set => _error = value;
    }

    private Result(bool isSuccess, TError? error)
        => (IsSuccess, Error) = (isSuccess, error);

    public static Result<TError> Success()
        => new(true, default);

    public static Result<TError> Failure(TError error)
        => new(false, error);

    public static implicit operator Result<TError>(TError error) => Failure(error);

    public Result<TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        where TNewError : IError
        => IsSuccess
            ? Result<TNewError>.Success()
            : Result<TNewError>.Failure(map(Error!));

    public Result<TError> Bind(Func<Result<TError>> binder)
        => IsSuccess
            ? binder()
            : Result<TError>.Failure(Error!);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure)
        => IsSuccess
            ? onSuccess()
            : onFailure(Error!);

    public Result<TError> Ensure(Func<bool> predicate, Func<TError> errorFactory)
    {
        if (IsFailure)
        {
            return Result<TError>.Failure(Error!);
        }

        if (predicate())
        {
            return this;
        }

        return Result<TError>.Failure(errorFactory());
    }

    public Result<TError> EnsureBy<TOtherError>(Result<TOtherError> other, Func<TOtherError, TError> errorMapper)
        where TOtherError : IError
    {
        if (IsFailure)
        {
            return Result<TError>.Failure(Error!);
        }

        if (other.IsSuccess)
        {
            return this;
        }

        return Result<TError>.Failure(errorMapper(other.Error!));
    }

    public Result<TError> Tap(Action onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess();
        }

        return this;
    }

    public Result<TError> TapError(Action<TError> onError)
    {
        if (IsFailure)
        {
            onError(Error!);
        }

        return this;
    }

    public Task<Result<TError>> ToTask() => Task.FromResult(this);
}


public sealed class Result<T, TError>
    where TError : IError
{
    private T? _value;
    private TError? _error;

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public T? Value
    {
        get => IsSuccess ? _value : throw new InvalidOperationException("Result is not successful.");
        private set => _value = value;
    }
    public TError? Error
    {
        get => IsFailure ? _error : throw new InvalidOperationException("Result is successful.");
        private set => _error = value;
    }

    private Result(bool isSuccess, T? value, TError? error)
        => (IsSuccess, Value, Error) = (isSuccess, value, error);

    public static Result<T, TError> Success(T value)
        => new(true, value, default);

    public static Result<T, TError> Failure(TError error)
        => new(false, default, error);

    public static implicit operator Result<T, TError>(T value) => Success(value);

    public static implicit operator Result<T, TError>(TError error) => Failure(error);

    public static explicit operator T(Result<T, TError> result) => result.Value!;

    public Result<TResult, TError> Map<TResult>(Func<T, TResult> mapper)
        => IsSuccess
            ? Result<TResult, TError>.Success(mapper(Value!))
            : Result<TResult, TError>.Failure(Error!);

    public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        where TNewError : IError
        => IsSuccess
            ? Result<T, TNewError>.Success(Value!)
            : Result<T, TNewError>.Failure(map(Error!));

    public Result<TResult, TError> Bind<TResult>(Func<T, Result<TResult, TError>> binder)
        => IsSuccess
            ? binder(Value!)
            : Result<TResult, TError>.Failure(Error!);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure)
            => IsSuccess
                ? onSuccess(Value!)
                : onFailure(Error!);

    public Result<T, TError> Ensure(Func<T, bool> predicate, Func<T, TError> errorFactory)
    {
        if (IsFailure)
        {
            return Result<T, TError>.Failure(Error!);
        }

        if (predicate(Value!))
        {
            return this;
        }

        return Result<T, TError>.Failure(errorFactory(Value!));
    }

    public Result<T, TError> EnsureBy<TOther, TOtherError>(Func<T, Result<TOther, TOtherError>> other, Func<TOtherError, TError> errorMapper)
        where TOtherError : IError
    {
        if (IsFailure)
        {
            return Result<T, TError>.Failure(Error!);
        }

        var otherResult = other(Value!);
        if (otherResult.IsSuccess)
        {
            return this;
        }

        return Result<T, TError>.Failure(errorMapper(otherResult.Error!));
    }

    public Result<T, TError> EnsureBy<TOtherError>(Func<T, Result<TOtherError>> other, Func<TOtherError, TError> errorMapper)
        where TOtherError : IError
    {
        if (IsFailure)
        {
            return Result<T, TError>.Failure(Error!);
        }

        var otherResult = other(Value!);
        if (otherResult.IsSuccess)
        {
            return this;
        }

        return Result<T, TError>.Failure(errorMapper(otherResult.Error!));
    }

    public Result<T, TError> Tap(Action<T> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(Value!);
        }

        return this;
    }

    public Result<T, TError> TapError(Action<TError> onError)
    {
        if (IsFailure)
        {
            onError(Error!);
        }

        return this;
    }

    public Task<Result<T, TError>> ToTask() => Task.FromResult(this);
}

public static class AsyncResultExtensions
{
    #region Bind & BindAsync
    public static async Task<Result<TError>> Bind<TError>(
        this Task<Result<TError>> taskResult,
        Func<Result<TError>> binder
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            return binder();
        }
        return Result<TError>.Failure(result.Error!);
    }

    public static async Task<Result<TResult, TError>> Bind<T, TError, TResult>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Result<TResult, TError>> binder
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            return binder(result.Value!);
        }
        return Result<TResult, TError>.Failure(result.Error!);
    }

    public static async Task<Result<TError>> BindAsync<TError>(
        this Task<Result<TError>> taskResult,
        Func<Task<Result<TError>>> binder
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            return await binder();
        }
        return Result<TError>.Failure(result.Error!);
    }

    public static async Task<Result<TResult, TError>> BindAsync<T, TError, TResult>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Task<Result<TResult, TError>>> binder
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            return await binder(result.Value!);
        }
        return Result<TResult, TError>.Failure(result.Error!);
    }

    #endregion

    #region Map & MapAsync
    public static async Task<Result<TNew, TError>> Map<T, TError, TNew>(
        this Task<Result<T, TError>> taskResult,
        Func<T, TNew> mapper
    )
        where TError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<TNew, TError>.Success(mapper(result.Value!))
            : Result<TNew, TError>.Failure(result.Error!);
    }

    public static async Task<Result<TResult, TError>> MapAsync<T, TError, TResult>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Task<TResult>> mapper
    )
        where TError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<TResult, TError>.Success(await mapper(result.Value!))
            : Result<TResult, TError>.Failure(result.Error!);
    }
    #endregion

    #region MapError & MapErrorAsync
    public static async Task<Result<TNewError>> MapError<TError, TNewError>(
        this Task<Result<TError>> taskResult,
        Func<TError, TNewError> map
    )
        where TError : IError
        where TNewError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<TNewError>.Success()
            : Result<TNewError>.Failure(map(result.Error!));
    }

    public static async Task<Result<T, TNewError>> MapError<T, TError, TNewError>(
        this Task<Result<T, TError>> taskResult,
        Func<TError, TNewError> map
    )
        where TError : IError
        where TNewError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<T, TNewError>.Success(result.Value!)
            : Result<T, TNewError>.Failure(map(result.Error!));
    }

    public static async Task<Result<TNewError>> MapErrorAsync<TError, TNewError>(
        this Task<Result<TError>> taskResult,
        Func<TError, Task<TNewError>> map
    )
        where TError : IError
        where TNewError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<TNewError>.Success()
            : Result<TNewError>.Failure(await map(result.Error!));
    }

    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> taskResult,
        Func<TError, Task<TNewError>> map
    )
        where TError : IError
        where TNewError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<T, TNewError>.Success(result.Value!)
            : Result<T, TNewError>.Failure(await map(result.Error!));
    }
    #endregion

    #region Tap & TapAsync
    public static async Task<Result<TError>> Tap<TError>(
        this Task<Result<TError>> taskResult,
        Action onSuccess
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            onSuccess();
        }
        return result;
    }

    public static async Task<Result<T, TError>> Tap<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Action> onSuccess
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            onSuccess(result.Value!);
        }
        return result;
    }

    public static async Task<Result<TError>> TapAsync<TError>(
        this Task<Result<TError>> taskResult,
        Func<Task> onSuccess
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            await onSuccess();
        }
        return result;
    }

    public static async Task<Result<T, TError>> TapAsync<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Task> onSuccess
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsSuccess)
        {
            await onSuccess(result.Value!);
        }
        return result;
    }
    #endregion

    #region TapError & TapErrorAsync
    public static async Task<Result<TError>> TapError<TError>(
        this Task<Result<TError>> taskResult,
        Func<TError, Action> onError
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            onError(result.Error!);
        }
        return result;
    }

    public static async Task<Result<T, TError>> TapError<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<TError, Action> onError
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            onError(result.Error!);
        }
        return result;
    }

    public static async Task<Result<TError>> TapErrorAsync<TError>(
        this Task<Result<TError>> taskResult,
        Func<TError, Task> onError
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            await onError(result.Error!);
        }
        return result;
    }

    public static async Task<Result<T, TError>> TapErrorAsync<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<TError, Task> onError
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            await onError(result.Error!);
        }
        return result;
    }
    #endregion

    #region Ensure & EnsureAsync & EnsureNotNull
    public static async Task<Result<T, TError>> Ensure<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<T, bool> predicate,
        Func<T, TError> errorFactory
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            return Result<T, TError>.Failure(result.Error!);
        }
        return predicate(result.Value!)
            ? result
            : Result<T, TError>.Failure(errorFactory(result.Value!));
    }

    public static async Task<Result<T, TError>> EnsureAsync<T, TError>(
        this Task<Result<T, TError>> taskResult,
        Func<T, Task<bool>> predicateTask,
        Func<T, TError> errorFactory
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            return Result<T, TError>.Failure(result.Error!);
        }
        return await predicateTask(result.Value!)
            ? result
            : Result<T, TError>.Failure(errorFactory(result.Value!));
    }

    public static async Task<Result<T, TError>> EnsureNotNull<T, TError>(
        this Task<Result<T?, TError>> taskResult,
        Func<TError> errorFactory
    )
        where TError : IError
    {
        var result = await taskResult;
        if (result.IsFailure)
        {
            return Result<T, TError>.Failure(result.Error!);
        }
        return result.Value is not null
            ? Result<T, TError>.Success(result.Value)
            : Result<T, TError>.Failure(errorFactory());
    }
    #endregion

    #region ToTaskResultAsync & ToUnitResult
    public static async Task<Result<T, TError>> ToTaskResult<T, TError>(
        this Task<T> taskValue,
        Func<T, bool> predicate,
        Func<T, TError> errorFactory
    )
        where TError : IError
    {
        var value = await taskValue;
        return predicate(value)
            ? Result<T, TError>.Success(value)
            : Result<T, TError>.Failure(errorFactory(value));
    }

    public static async Task<Result<TError>> ToUnitResult<T, TError>(
        this Task<Result<T, TError>> taskResult
    )
        where TError : IError
    {
        var result = await taskResult;
        return result.IsSuccess
            ? Result<TError>.Success()
            : Result<TError>.Failure(result.Error!);
    }
    #endregion
}
