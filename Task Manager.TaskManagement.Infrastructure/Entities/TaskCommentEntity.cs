using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Entities;

public class TaskCommentEntity : ITaskCommentData, IInfrastructureEntity<TaskComment, TaskCommentEntity>
{
    public Guid Id { get; set; }
    public UserEntity? Author { get; set; }
    public Guid AuthorId { get; set; }
    public TaskItemEntity? Task { get; set; }
    public Guid TaskId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }

    protected TaskCommentEntity() { } // for EFCore

    public TaskCommentEntity(TaskComment domainTaskComment)
    {
        Id = domainTaskComment.Id;
        AuthorId = domainTaskComment.AuthorId;
        Message = domainTaskComment.Message;
        Timestamp = domainTaskComment.Timestamp;
    }

    public static TaskCommentEntity Create(TaskComment input) => new(input);
}
