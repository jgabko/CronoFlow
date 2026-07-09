using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Services;

namespace TimeFlow.ViewModels;

public partial class ReportsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<EmployeePerformance> _employeeStats = [];

    [ObservableProperty]
    private ObservableCollection<TaskPerformance> _taskStats = [];

    [ObservableProperty]
    private string _teamTotalTime = "00:00";

    [ObservableProperty]
    private string _teamTotalEntries = "0";

    [ObservableProperty]
    private string _teamActiveTasks = "0";

    [ObservableProperty]
    private string _teamEmployees = "0";

    public bool IsManager => ServiceLocator.Session.IsManager;
    public bool ShowEmployeeSection => IsManager;
    public bool ShowTaskSection => true;
    public bool ShowTeamSection => IsManager;

    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var user = ServiceLocator.Session.CurrentUser!;

        if (IsManager)
        {
            EmployeeStats = new ObservableCollection<EmployeePerformance>(
                await ServiceLocator.Reports.GetEmployeePerformanceAsync());
            TaskStats = new ObservableCollection<TaskPerformance>(
                await ServiceLocator.Reports.GetTaskPerformanceAsync());

            var team = await ServiceLocator.Reports.GetTeamPerformanceAsync();
            TeamTotalTime = TimeFormatter.FormatDuration(team.TotalSeconds);
            TeamTotalEntries = team.TotalEntries.ToString();
            TeamActiveTasks = team.ActiveTasks.ToString();
            TeamEmployees = team.Employees.ToString();
        }
        else
        {
            EmployeeStats = new ObservableCollection<EmployeePerformance>(
                await ServiceLocator.Reports.GetEmployeePerformanceForUserAsync(user.Id));

            var allTasks = await ServiceLocator.Reports.GetTaskPerformanceAsync();
            var userTasks = await ServiceLocator.Tasks.GetTasksForUserAsync(user.Id, false);
            var userTaskIds = userTasks.Select(t => t.Id).ToHashSet();
            TaskStats = new ObservableCollection<TaskPerformance>(
                allTasks.Where(t => userTaskIds.Contains(t.TaskId)));
        }
    }
}
