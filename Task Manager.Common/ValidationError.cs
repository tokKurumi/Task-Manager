namespace Task_Manager.Common;

public sealed record ValidationFailureInfo(
    string? PropertyName,
    string? ErrorMessage
);

public sealed record ValidationError(IReadOnlyCollection<ValidationFailureInfo> InnerFailures) : IError;
