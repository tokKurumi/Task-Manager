using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Riok.Mapperly.Abstractions;
using System.Diagnostics;
using Task_Manager.Common;

namespace Task_Manager.Identity.Application;

public abstract class ValidationBehaviorBase<TMessage, TResponse>(
    ActivitySource activitySource,
    IEnumerable<IValidator<TMessage>> validators,
    IValidationErrorFactory<TResponse> errorFactory
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
    where TResponse : notnull
{
    protected readonly ActivitySource _activitySource = activitySource;
    protected readonly IEnumerable<IValidator<TMessage>> _validators = validators;
    private readonly IValidationErrorFactory<TResponse> _errorFactory = errorFactory;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next
    )
    {
        if (!_validators.Any())
        {
            return await next(message, cancellationToken);
        }

        using var activity = _activitySource.StartActivity($"Validating {typeof(TMessage).Name}");

        var context = new ValidationContext<TMessage>(message);
        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(message, cancellationToken);
        }

        var validationErrorInfos = failures.ConvertAll(ValidationFailureMapper.ToGeneralValidationFailure);
        var error = new ValidationError(validationErrorInfos);

        return _errorFactory.Create(error);
    }
}

public class RequestValidationBehavior<TMessage, TResponse>(
    ActivitySource activitySource,
    IEnumerable<IValidator<TMessage>> validators,
    IValidationErrorFactory<TResponse> errorFactory
) : ValidationBehaviorBase<TMessage, TResponse>(activitySource, validators, errorFactory)
    where TMessage : notnull, IRequest<TResponse>
    where TResponse : notnull;

public class CommandValidationBehavior<TMessage, TResponse>(
    ActivitySource activitySource,
    IEnumerable<IValidator<TMessage>> validators,
    IValidationErrorFactory<TResponse> errorFactory
) : ValidationBehaviorBase<TMessage, TResponse>(activitySource, validators, errorFactory)
    where TMessage : notnull, ICommand<TResponse>
    where TResponse : notnull;

[Mapper]
public static partial class ValidationFailureMapper
{
    [MapperIgnoreSource(nameof(ValidationFailure.ErrorCode))]
    [MapperIgnoreSource(nameof(ValidationFailure.AttemptedValue))]
    [MapperIgnoreSource(nameof(ValidationFailure.CustomState))]
    [MapperIgnoreSource(nameof(ValidationFailure.Severity))]
    [MapperIgnoreSource(nameof(ValidationFailure.ErrorCode))]
    [MapperIgnoreSource(nameof(ValidationFailure.FormattedMessagePlaceholderValues))]
    public static partial ValidationFailureInfo ToGeneralValidationFailure(ValidationFailure failure);
}
