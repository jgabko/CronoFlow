using System.ComponentModel.DataAnnotations.Schema;

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

    /// <summary>
    /// Soma dos TimeEntries já finalizados. Requer que TimeEntries tenha sido
    /// carregado via Include — caso contrário fica 0 (não dispara lazy load).
    /// </summary>
    [NotMapped]
    public long TotalTrackedSeconds => TimeEntries.Sum(e => e.DurationSeconds);
}