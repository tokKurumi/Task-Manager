using Task_Manager.TaskManagement.Core.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Entities;

public class UserEntity : IUserData
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<TaskItemEntity> Tasks { get; set; } = [];
    public ICollection<TaskCommentEntity> Comments { get; set; } = [];

    protected UserEntity() { } // For EF Core

    public UserEntity(User domainUser)
    {
        Id = domainUser.Id;
        DisplayName = domainUser.DisplayName;
    }
}
