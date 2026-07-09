namespace TimeFlow.Models;

/// <summary>
/// Persisted timer state for crash recovery. One active timer per user at a time.
/// </summary>
public class ActiveTimer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime LastResumedAt { get; set; }
    public long AccumulatedSeconds { get; set; }
    public TimerState State { get; set; }
}
