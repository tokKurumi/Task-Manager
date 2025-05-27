using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.DisplayName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(user => user.Tasks)
            .WithOne(task => task.User)
            .HasForeignKey(task => task.UserId);

        builder.HasMany(user => user.Comments)
            .WithOne(comment => comment.Author)
            .HasForeignKey(comment => comment.AuthorId);
    }
}
