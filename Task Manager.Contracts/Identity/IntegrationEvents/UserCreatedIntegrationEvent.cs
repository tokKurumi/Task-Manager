namespace Task_Manager.Contracts.Identity.IntegrationEvents;

public sealed record UserCreatedIntegrationEvent(Guid Id, string Email, string DisplayName, DateTimeOffset CreatedAt) : IIntegrationEvent;
