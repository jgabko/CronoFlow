using Microsoft.EntityFrameworkCore;
using TimeFlow.Data;
using TimeFlow.Models;

namespace TimeFlow.Services;

public interface ITaskService
{
    Task<IReadOnlyList<WorkTask>> GetTasksForUserAsync(int userId, bool isManager);
    Task<IReadOnlyList<WorkTask>> GetAllTasksAsync();
    Task<WorkTask> CreateTaskAsync(string title, string? description, IEnumerable<int> assigneeIds);
    Task AssignUsersAsync(int taskId, IEnumerable<int> userIds);
    Task CompleteTaskAsync(int taskId);
}

public class TaskService(TimeFlowDbContext db) : ITaskService
{
    public async Task<IReadOnlyList<WorkTask>> GetTasksForUserAsync(int userId, bool isManager)
    {
        if (isManager)
            return await GetAllTasksAsync();

        return await db.Tasks
            .Include(t => t.Assignments).ThenInclude(a => a.User)
            .Where(t => t.Assignments.Any(a => a.UserId == userId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<WorkTask>> GetAllTasksAsync()
        => await db.Tasks
            .Include(t => t.Assignments).ThenInclude(a => a.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task<WorkTask> CreateTaskAsync(string title, string? description, IEnumerable<int> assigneeIds)
    {
        var task = new WorkTask
        {
            Title = title,
            Description = description,
            Status = WorkTaskStatus.Active
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        foreach (var userId in assigneeIds.Distinct())
        {
            db.TaskAssignments.Add(new TaskAssignment { TaskId = task.Id, UserId = userId });
        }

        await db.SaveChangesAsync();
        return task;
    }

    public async Task AssignUsersAsync(int taskId, IEnumerable<int> userIds)
    {
        var existing = await db.TaskAssignments
            .Where(a => a.TaskId == taskId)
            .Select(a => a.UserId)
            .ToListAsync();

        foreach (var userId in userIds.Distinct().Except(existing))
        {
            db.TaskAssignments.Add(new TaskAssignment { TaskId = taskId, UserId = userId });
        }

        await db.SaveChangesAsync();
    }

    public async Task CompleteTaskAsync(int taskId)
    {
        var task = await db.Tasks.FindAsync(taskId);
        if (task is not null)
        {
            task.Status = WorkTaskStatus.Completed;
            await db.SaveChangesAsync();
        }
    }
}
