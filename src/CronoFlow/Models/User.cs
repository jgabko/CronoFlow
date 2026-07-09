namespace CronoFlow.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TaskAssignment> Assignments { get; set; } = [];
    public ICollection<ActiveTimer> ActiveTimers { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}
