namespace CronoFlow.Models;

/// <summary>
/// Immutable audit record of a single timer action (start/pause/resume/stop),
/// separate from ActiveTimer (current state) and TimeEntry (finished segment).
/// </summary>
public class TimerActionLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;
    public TimerActionType ActionType { get; set; }
    public DateTime TimestampUtc { get; set; }
}