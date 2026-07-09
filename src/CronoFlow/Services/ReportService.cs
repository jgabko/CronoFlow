using Microsoft.EntityFrameworkCore;
using CronoFlow.Data;

namespace CronoFlow.Services;

public record EmployeePerformance(
    int UserId,
    string DisplayName,
    long TotalSeconds,
    int EntryCount,
    int TasksWorked);

public record TaskPerformance(
    int TaskId,
    string Title,
    long TotalSeconds,
    int Contributors,
    int EntryCount,
    string Status);

public record TeamPerformance(
    long TotalSeconds,
    int TotalEntries,
    int ActiveTasks,
    int Employees);

public interface IReportService
{
    Task<IReadOnlyList<EmployeePerformance>> GetEmployeePerformanceAsync();
    Task<IReadOnlyList<TaskPerformance>> GetTaskPerformanceAsync();
    Task<TeamPerformance> GetTeamPerformanceAsync();
    Task<IReadOnlyList<EmployeePerformance>> GetEmployeePerformanceForUserAsync(int userId);
}

public class ReportService(CronoFlowDbContext db) : IReportService
{
    public async Task<IReadOnlyList<EmployeePerformance>> GetEmployeePerformanceAsync()
    {
        var entries = await db.TimeEntries
            .Include(e => e.User)
            .ToListAsync();

        return entries
            .GroupBy(e => new { e.UserId, e.User.DisplayName })
            .Select(g => new EmployeePerformance(
                g.Key.UserId,
                g.Key.DisplayName,
                g.Sum(e => e.DurationSeconds),
                g.Count(),
                g.Select(e => e.TaskId).Distinct().Count()))
            .OrderByDescending(p => p.TotalSeconds)
            .ToList();
    }

    public async Task<IReadOnlyList<TaskPerformance>> GetTaskPerformanceAsync()
    {
        var entries = await db.TimeEntries
            .Include(e => e.Task)
            .ToListAsync();

        return entries
            .GroupBy(e => new { e.TaskId, e.Task.Title, e.Task.Status })
            .Select(g => new TaskPerformance(
                g.Key.TaskId,
                g.Key.Title,
                g.Sum(e => e.DurationSeconds),
                g.Select(e => e.UserId).Distinct().Count(),
                g.Count(),
                g.Key.Status.ToString()))
            .OrderByDescending(p => p.TotalSeconds)
            .ToList();
    }

    public async Task<TeamPerformance> GetTeamPerformanceAsync()
    {
        var totalSeconds = await db.TimeEntries.SumAsync(e => (long?)e.DurationSeconds) ?? 0;
        var totalEntries = await db.TimeEntries.CountAsync();
        var activeTasks = await db.Tasks.CountAsync(t => t.Status == Models.WorkTaskStatus.Active);
        var employees = await db.Users.CountAsync(u => u.Role == Models.UserRole.Employee);

        return new TeamPerformance(totalSeconds, totalEntries, activeTasks, employees);
    }

    public async Task<IReadOnlyList<EmployeePerformance>> GetEmployeePerformanceForUserAsync(int userId)
    {
        var entries = await db.TimeEntries
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .ToListAsync();

        if (entries.Count == 0)
            return [];

        var user = entries[0].User;
        return
        [
            new EmployeePerformance(
                userId,
                user.DisplayName,
                entries.Sum(e => e.DurationSeconds),
                entries.Count,
                entries.Select(e => e.TaskId).Distinct().Count())
        ];
    }
}
