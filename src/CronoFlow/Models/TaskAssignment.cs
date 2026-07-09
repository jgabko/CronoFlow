namespace TimeFlow.Models;

public class TaskAssignment
{
    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
