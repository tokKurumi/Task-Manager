using Microsoft.EntityFrameworkCore;
using Task_Manager.TaskManagement.Infrastructure.Configuration;
using Task_Manager.TaskManagement.Infrastructure.Entities;

namespace Task_Manager.TaskManagement.Infrastructure.Data;

public class TaskManagementDbContext(
    DbContextOptions<TaskManagementDbContext> options
) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskCommentEntityConfiguration());
    }
}
