namespace Task_Manager.Common;

public sealed record ValidationFailureInfo(
    string? PropertyName,
    string? ErrorMessage
);

public sealed record ValidationError(IReadOnlyCollection<ValidationFailureInfo> InnerFailures) : IError;

public interface IValidationErrorFactory<out TResponse>
{
    TResponse Create(ValidationError error);
}

public class GenericValidationErrorFactory<T, TError>
    : IValidationErrorFactory<Result<T, OneOfError<TError, ValidationError>>>
    where TError : IError
{
    public Result<T, OneOfError<TError, ValidationError>> Create(ValidationError error)
    {
        return new OneOfError<TError, ValidationError>(error);
    }
}
