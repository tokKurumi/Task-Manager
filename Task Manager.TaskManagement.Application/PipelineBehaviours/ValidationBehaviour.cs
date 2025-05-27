using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Riok.Mapperly.Abstractions;
using System.Diagnostics;
using Task_Manager.Common;

namespace Task_Manager.TaskManagement.Application.PipelineBehaviours;

public sealed class ValidationBehaviour<TMessage, TResponse>(
    ActivitySource activitySource,
    IEnumerable<IValidator<TMessage>> validators,
    IValidationErrorFactory<TResponse> errorFactory
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ActivitySource _activitySource = activitySource;
    private readonly IEnumerable<IValidator<TMessage>> _validators = validators;
    private readonly IValidationErrorFactory<TResponse> _errorFactory = errorFactory;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next)
    {
        if (!_validators.Any())
        {
            return await next(message, cancellationToken);
        }

        using var activity = _activitySource.StartActivity($"ValidationBehaviour {typeof(TMessage).Name}");

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
