﻿namespace Task_Manager.TaskManagement.Application.Services.Contracts;

public record CreateTaskCommand(
    Guid UserPerformerId,
    string Title,
    string Description,
    string Notes,
    DateTimeOffset? ApproximateCompletedAt
);
