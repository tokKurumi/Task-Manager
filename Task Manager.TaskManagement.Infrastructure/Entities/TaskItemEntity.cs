using Task_Manager.TaskManagement.Core.Entities;
using TaskStatus = Task_Manager.TaskManagement.Core.Entities.TaskStatus;

namespace Task_Manager.TaskManagement.Infrastructure.Entities;

public class TaskItemEntity : ITaskItemData
{
    public Guid Id { get; set; }
    public UserEntity? User { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ApproximateCompletedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public ICollection<TaskCommentEntity> Comments { get; set; } = [];

    private ITaskItemStatusData? _statusAdapter;
    ITaskItemStatusData ITaskItemData.Status => _statusAdapter ??= new TaskItemStatusAdapter(this);

    protected TaskItemEntity() { } // for EFCore

    public TaskItemEntity(TaskItem domainTaskItem)
    {
        Id = domainTaskItem.Id;
        UserId = domainTaskItem.UserId;
        Title = domainTaskItem.Title;
        Description = domainTaskItem.Description;
        Notes = domainTaskItem.Notes;
        Status = domainTaskItem.Status.Status;
        CreatedAt = domainTaskItem.Status.CreatedAt;
        ApproximateCompletedAt = domainTaskItem.Status.ApproximateCompletedAt;
        CompletedAt = domainTaskItem.Status.CompletedAt;
    }

    private sealed class TaskItemStatusAdapter(TaskItemEntity entity) : ITaskItemStatusData
    {
        private readonly TaskItemEntity _entity = entity;

        public TaskStatus Status => _entity.Status;
        public DateTimeOffset CreatedAt => _entity.CreatedAt;
        public DateTimeOffset? ApproximateCompletedAt => _entity.ApproximateCompletedAt;
        public DateTimeOffset? CompletedAt => _entity.CompletedAt;
    }
}
