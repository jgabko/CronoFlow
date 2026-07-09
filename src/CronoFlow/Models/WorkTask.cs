namespace CronoFlow.Models;

public class WorkTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskAssignment> Assignments { get; set; } = [];
    public ICollection<ActiveTimer> ActiveTimers { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}
