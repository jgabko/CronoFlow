using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CronoFlow.Models;

namespace CronoFlow.ViewModels;

public partial class TasksViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<WorkTask> _tasks = [];

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private int? _pendingDeleteTaskId;

    [ObservableProperty]
    private string _newTaskDescription = string.Empty;

    [ObservableProperty]
    private ObservableCollection<User> _employees = [];

    [ObservableProperty]
    private User? _selectedAssignee;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public bool IsManager => ServiceLocator.Session.IsManager;

    public async Task InitializeAsync()
    {
        await RefreshAsync();

        if (IsManager)
        {
            var all = await ServiceLocator.Auth.GetAllEmployeesAsync();
            Employees = new ObservableCollection<User>(all.Where(u => u.Role == UserRole.Employee));
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var user = ServiceLocator.Session.CurrentUser!;
        var tasks = await ServiceLocator.Tasks.GetTasksForUserAsync(user.Id, IsManager);
        Tasks = new ObservableCollection<WorkTask>(tasks);
        PendingDeleteTaskId = null;
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(WorkTask task)
    {
        if (!IsManager)
            return;

        if (PendingDeleteTaskId != task.Id)
        {
            // First click just arms the confirmation; the UI swaps the button label.
            PendingDeleteTaskId = task.Id;
            return;
        }

        StatusMessage = string.Empty;
        try
        {
            await ServiceLocator.Tasks.DeleteTaskAsync(task.Id);
            StatusMessage = "Tarefa excluída.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            PendingDeleteTaskId = null;
            await RefreshAsync();
        }
    }

[RelayCommand]
private void CancelDeleteTask()
{
    PendingDeleteTaskId = null;
}
    [RelayCommand]
    private async Task CreateTaskAsync()
    {
        if (!IsManager)
            return;

        StatusMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            StatusMessage = "Informe o título da tarefa.";
            return;
        }

        var assignees = SelectedAssignee is not null
            ? new[] { SelectedAssignee.Id }
            : Array.Empty<int>();

        await ServiceLocator.Tasks.CreateTaskAsync(NewTaskTitle.Trim(), NewTaskDescription.Trim(), assignees);
        NewTaskTitle = string.Empty;
        NewTaskDescription = string.Empty;
        SelectedAssignee = null;
        StatusMessage = "Tarefa criada.";
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task CompleteTaskAsync(WorkTask task)
    {
        if (!IsManager)
            return;

        await ServiceLocator.Tasks.CompleteTaskAsync(task.Id);
        await RefreshAsync();
    }
}
