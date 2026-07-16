using Microsoft.EntityFrameworkCore;
using CronoFlow.Data;
using CronoFlow.Models;

namespace CronoFlow.Services;

public interface ITaskService
{
    Task<IReadOnlyList<WorkTask>> GetTasksForUserAsync(int userId, bool isManager);
    Task<IReadOnlyList<WorkTask>> GetAllTasksAsync();
    Task<WorkTask> CreateTaskAsync(string title, string? description, IEnumerable<int> assigneeIds);
    Task AssignUsersAsync(int taskId, IEnumerable<int> userIds);
    Task CompleteTaskAsync(int taskId);

    Task DeleteTaskAsync(int taskId);
}

public class TaskService(CronoFlowDbContext db) : ITaskService
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

    public async Task DeleteTaskAsync(int taskId)
{
    var task = await db.Tasks.FindAsync(taskId);
    if (task is null)
        return;

    var hasRunningTimer = await db.ActiveTimers.AnyAsync(t => t.TaskId == taskId);
    if (hasRunningTimer)
        throw new InvalidOperationException("Não é possível excluir uma tarefa com cronômetro em andamento. Peça para pausar ou parar antes.");

    // Delete manually instead of relying on DB cascade: EnsureCreated + ad hoc
    // ALTER TABLE schema updates in this project make implicit FK cascade unreliable.
    var assignments = db.TaskAssignments.Where(a => a.TaskId == taskId);
    var timeEntries = db.TimeEntries.Where(e => e.TaskId == taskId);
    var actionLogs = db.TimerActionLogs.Where(l => l.TaskId == taskId);
    db.TimerActionLogs.RemoveRange(actionLogs);

    db.TaskAssignments.RemoveRange(assignments);
    db.TimeEntries.RemoveRange(timeEntries);
    db.Tasks.Remove(task);

    

    await db.SaveChangesAsync();
}
}
