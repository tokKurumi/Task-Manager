﻿using Microsoft.EntityFrameworkCore;
using Task_Manager.Common;
using Task_Manager.TaskManagement.Application.Services.Abstractions;
using Task_Manager.TaskManagement.Core.Entities;
using Task_Manager.TaskManagement.Infrastructure.Data;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Repositories;

public class TaskItemRepository(
    TaskManagementDbContext context,
    Func<Guid, TaskItemRepositoryError> notFoundErrorFactory
) : GenericRepository<TaskItem, TaskItemEntity, TaskItemRepositoryError>(context, notFoundErrorFactory), ITaskItemRepository
{
    private readonly TaskManagementDbContext _context = context;

    public async Task<Result<TaskItem, TaskItemRepositoryError>> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var taskEntity = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);
        if (taskEntity is null)
        {
            return new TaskNotFoundError(task.Id);
        }

        taskEntity.UserId = task.UserId;
        taskEntity.Title = task.Title;
        taskEntity.Description = task.Description;
        taskEntity.Notes = task.Notes;
        taskEntity.Status = task.Status.Status;
        taskEntity.ApproximateCompletedAt = task.Status.ApproximateCompletedAt;
        taskEntity.CompletedAt = task.Status.CompletedAt;

        return TaskItem.ConvertFromData(taskEntity);
    }

    public async Task<Result<TaskComment, TaskItemRepositoryError>> AddCommentAsync(Guid taskId, TaskComment comment, CancellationToken cancellationToken = default)
    {
        var taskEntity = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == taskId, cancellationToken);
        if (taskEntity is null)
        {
            return new TaskNotFoundError(taskId);
        }

        var commentEntity = TaskCommentEntity.Create(comment);
        taskEntity.Comments.Add(commentEntity);

        return TaskComment.ConvertFromData(commentEntity);
    }

    public async Task<Result<Page<TaskComment>, TaskItemRepositoryError>> GetCommentPageAsync(Guid taskId, IPagination pagination, CancellationToken cancellationToken = default)
    {
        var totalTaskComments = await _context.Comments.AsNoTracking()
            .Where(comment => comment.TaskId == taskId)
            .CountAsync(cancellationToken);

        var commentEntities = await _context.Comments.AsNoTracking()
            .Where(comment => comment.TaskId == taskId)
            .OrderBy(comment => comment.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new Page<TaskComment>(commentEntities.ConvertAll(TaskComment.ConvertFromData), pagination, totalTaskComments);
    }
}
