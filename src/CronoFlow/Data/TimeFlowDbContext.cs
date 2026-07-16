using Microsoft.EntityFrameworkCore;
using CronoFlow.Models;

namespace CronoFlow.Data;

public class CronoFlowDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<WorkTask> Tasks => Set<WorkTask>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
    public DbSet<ActiveTimer> ActiveTimers => Set<ActiveTimer>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    public DbSet<TimerActionLog> TimerActionLogs => Set<TimerActionLog>();

    private readonly string _dbPath;

    public CronoFlowDbContext()
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CronoFlow");
        Directory.CreateDirectory(appData);
        _dbPath = Path.Combine(appData, "cronoflow.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.UserId });
            entity.HasOne(e => e.Task).WithMany(t => t.Assignments).HasForeignKey(e => e.TaskId);
            entity.HasOne(e => e.User).WithMany(u => u.Assignments).HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<ActiveTimer>(entity =>
        {
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.ActiveTimers).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Task).WithMany(t => t.ActiveTimers).HasForeignKey(e => e.TaskId);
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasOne(e => e.User).WithMany(u => u.TimeEntries).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Task).WithMany(t => t.TimeEntries).HasForeignKey(e => e.TaskId);
        });

        modelBuilder.Entity<TimerActionLog>(entity =>
        {
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Task).WithMany().HasForeignKey(e => e.TaskId);
            entity.HasIndex(e => e.TimestampUtc);
        });

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
    }
}
