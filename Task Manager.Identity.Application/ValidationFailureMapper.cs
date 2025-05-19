using FluentValidation.Results;
using Riok.Mapperly.Abstractions;
using Task_Manager.Common;

namespace Task_Manager.Identity.Application;

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
