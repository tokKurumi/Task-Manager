namespace Task_Manager.Identity.Core.Contracts;

public sealed record LoginUserResponse(
    Guid UserId,
    string Token,
    DateTimeOffset IssuedAt,
    TimeSpan ExpiresIn
);
