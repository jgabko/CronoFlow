using Microsoft.EntityFrameworkCore;
using CronoFlow.Data;
using CronoFlow.Models;

namespace CronoFlow.Services;

public record TimerActionLogEntry(
    int Id,
    int UserId,
    string UserDisplayName,
    int TaskId,
    string TaskTitle,
    TimerActionType ActionType,
    DateTime TimestampUtc);

public interface IActionLogService
{
    /// <summary>
    /// Funcionários só recebem os próprios eventos, independente dos filtros
    /// passados. Gerentes recebem todos, com os filtros opcionais aplicados.
    /// </summary>
    Task<IReadOnlyList<TimerActionLogEntry>> GetLogsAsync(
        int requestingUserId,
        bool isManager,
        int? filterUserId = null,
        int? filterTaskId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null);
}

public class ActionLogService(CronoFlowDbContext db) : IActionLogService
{
    public async Task<IReadOnlyList<TimerActionLogEntry>> GetLogsAsync(
        int requestingUserId,
        bool isManager,
        int? filterUserId = null,
        int? filterTaskId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
    {
        var query = db.TimerActionLogs
            .Include(l => l.User)
            .Include(l => l.Task)
            .AsQueryable();

        query = isManager
            ? (filterUserId.HasValue ? query.Where(l => l.UserId == filterUserId.Value) : query)
            : query.Where(l => l.UserId == requestingUserId);

        if (filterTaskId.HasValue)
            query = query.Where(l => l.TaskId == filterTaskId.Value);

        if (fromUtc.HasValue)
            query = query.Where(l => l.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(l => l.TimestampUtc <= toUtc.Value);

        return await query
            .OrderByDescending(l => l.TimestampUtc)
            .Select(l => new TimerActionLogEntry(
                l.Id, l.UserId, l.User.DisplayName, l.TaskId, l.Task.Title, l.ActionType, l.TimestampUtc))
            .ToListAsync();
    }
}