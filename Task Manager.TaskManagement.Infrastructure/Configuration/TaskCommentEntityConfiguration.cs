using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Configuration;

public class TaskCommentEntityConfiguration : IEntityTypeConfiguration<TaskCommentEntity>
{
    public void Configure(EntityTypeBuilder<TaskCommentEntity> builder)
    {
        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Message)
            .IsRequired()
            .HasMaxLength(1024);

        builder.HasOne(comment => comment.Author)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.AuthorId);

        builder.HasOne(comment => comment.Task)
            .WithMany(task => task.Comments)
            .HasForeignKey(comment => comment.TaskId);
    }
}
