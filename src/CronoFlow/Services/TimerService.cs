using Microsoft.EntityFrameworkCore;
using CronoFlow.Data;
using CronoFlow.Models;

namespace CronoFlow.Services;

public record TimerSnapshot(
    int TaskId,
    string TaskTitle,
    TimerState State,
    long ElapsedSeconds,
    DateTime StartedAt,
    long AccumulatedSeconds,
    DateTime LastResumedAt)
{
    /// <summary>
    /// Tempo decorrido calculado no momento da leitura (não um valor congelado).
    /// Deve ser usado pela UI a cada "tick" do timer visual, em vez de ElapsedSeconds,
    /// que só reflete o instante em que o snapshot foi criado.
    /// </summary>
    public long LiveElapsedSeconds =>
        State == TimerState.Running
            ? AccumulatedSeconds + (long)(DateTime.UtcNow - LastResumedAt).TotalSeconds
            : ElapsedSeconds;
}

public interface ITimerService
{
    event Action? TimerChanged;
    TimerSnapshot? Current { get; }
    Task<TimerSnapshot?> RecoverAsync(int userId);
    Task<TimerSnapshot> StartAsync(int userId, int taskId);
    Task<TimerSnapshot?> PauseAsync(int userId);
    Task<TimerSnapshot?> ResumeAsync(int userId);
    Task<TimeEntry?> StopAsync(int userId);
    long CalculateElapsed(ActiveTimer timer);
}

public class TimerService(CronoFlowDbContext db) : ITimerService
{
    public event Action? TimerChanged;

    public TimerSnapshot? Current { get; private set; }

    public async Task<TimerSnapshot?> RecoverAsync(int userId)
    {
        var timer = await db.ActiveTimers
            .Include(t => t.Task)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (timer is null)
        {
            Current = null;
            return null;
        }

        Current = ToSnapshot(timer);
        TimerChanged?.Invoke();
        return Current;
    }

    public async Task<TimerSnapshot> StartAsync(int userId, int taskId)
    {
        var existing = await db.ActiveTimers.FirstOrDefaultAsync(t => t.UserId == userId);
        if (existing is not null)
            throw new InvalidOperationException("Já existe um cronômetro ativo. Pare ou pause antes de iniciar outro.");

        var assigned = await db.TaskAssignments.AnyAsync(a => a.UserId == userId && a.TaskId == taskId);
        if (!assigned)
            throw new InvalidOperationException("Tarefa não atribuída a este usuário.");

        var now = DateTime.UtcNow;
        var timer = new ActiveTimer
        {
            UserId = userId,
            TaskId = taskId,
            StartedAt = now,
            LastResumedAt = now,
            AccumulatedSeconds = 0,
            State = TimerState.Running
        };

        db.ActiveTimers.Add(timer);
        await db.SaveChangesAsync();

        await db.Entry(timer).Reference(t => t.Task).LoadAsync();
        Current = ToSnapshot(timer);
        TimerChanged?.Invoke();
        return Current;
    }

    public async Task<TimerSnapshot?> PauseAsync(int userId)
    {
        var timer = await db.ActiveTimers
            .Include(t => t.Task)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (timer is null || timer.State == TimerState.Paused)
            return Current;

        timer.AccumulatedSeconds = CalculateElapsed(timer);
        timer.State = TimerState.Paused;
        await db.SaveChangesAsync();

        Current = ToSnapshot(timer);
        TimerChanged?.Invoke();
        return Current;
    }

    public async Task<TimerSnapshot?> ResumeAsync(int userId)
    {
        var timer = await db.ActiveTimers
            .Include(t => t.Task)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (timer is null || timer.State == TimerState.Running)
            return Current;

        timer.LastResumedAt = DateTime.UtcNow;
        timer.State = TimerState.Running;
        await db.SaveChangesAsync();

        Current = ToSnapshot(timer);
        TimerChanged?.Invoke();
        return Current;
    }

    public async Task<TimeEntry?> StopAsync(int userId)
    {
        var timer = await db.ActiveTimers
            .Include(t => t.Task)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (timer is null)
            return null;

        var duration = CalculateElapsed(timer);
        var entry = new TimeEntry
        {
            UserId = userId,
            TaskId = timer.TaskId,
            StartedAt = timer.StartedAt,
            EndedAt = DateTime.UtcNow,
            DurationSeconds = duration
        };

        db.TimeEntries.Add(entry);
        db.ActiveTimers.Remove(timer);
        await db.SaveChangesAsync();

        Current = null;
        TimerChanged?.Invoke();
        return entry;
    }

    public long CalculateElapsed(ActiveTimer timer)
    {
        if (timer.State == TimerState.Paused)
            return timer.AccumulatedSeconds;

        var runningSeconds = (long)(DateTime.UtcNow - timer.LastResumedAt).TotalSeconds;
        return timer.AccumulatedSeconds + runningSeconds;
    }

    private TimerSnapshot ToSnapshot(ActiveTimer timer) =>
        new(timer.TaskId, timer.Task.Title, timer.State, CalculateElapsed(timer), timer.StartedAt,
            timer.AccumulatedSeconds, timer.LastResumedAt);
}
