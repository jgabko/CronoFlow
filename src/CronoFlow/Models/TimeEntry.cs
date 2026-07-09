namespace TimeFlow.Models;

/// <summary>
/// Completed time segment saved after Stop.
/// </summary>
public class TimeEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public long DurationSeconds { get; set; }
}
