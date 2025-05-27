using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Configuration;

public class TaskItemEntityConfiguration : IEntityTypeConfiguration<TaskItemEntity>
{
    public void Configure(EntityTypeBuilder<TaskItemEntity> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(task => task.Description)
            .HasMaxLength(2048);

        builder.Property(task => task.Notes)
            .HasMaxLength(2048);

        builder.HasOne(task => task.User)
            .WithMany(user => user.Tasks)
            .HasForeignKey(user => user.Id);

        builder.HasMany(task => task.Comments)
            .WithOne(comment => comment.Task)
            .HasForeignKey(comment => comment.TaskId);
    }
}
