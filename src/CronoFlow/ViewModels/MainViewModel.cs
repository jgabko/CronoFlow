using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Models;

namespace TimeFlow.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private string _userDisplayName = string.Empty;

    [ObservableProperty]
    private string _userRoleLabel = string.Empty;

    [ObservableProperty]
    private int _selectedNavIndex;

    public TimerViewModel TimerPage { get; } = new();
    public TasksViewModel TasksPage { get; } = new();
    public ReportsViewModel ReportsPage { get; } = new();
    public TimerMiniPlayerViewModel MiniPlayer { get; } = new();

    public bool IsManager => ServiceLocator.Session.IsManager;

    public async Task InitializeAsync(User user)
    {
        UserDisplayName = user.DisplayName;
        UserRoleLabel = user.Role == UserRole.Manager ? "Gerente" : "Funcionário";

        await TimerPage.InitializeAsync();
        await TasksPage.InitializeAsync();
        await ReportsPage.InitializeAsync();
        MiniPlayer.Refresh();

        CurrentPage = TimerPage;
        SelectedNavIndex = 0;
    }

    [RelayCommand]
    private void Navigate(string page)
    {
        CurrentPage = page switch
        {
            "timer" => TimerPage,
            "tasks" => TasksPage,
            "reports" => ReportsPage,
            _ => TimerPage
        };

        SelectedNavIndex = page switch
        {
            "timer" => 0,
            "tasks" => 1,
            "reports" => 2,
            _ => 0
        };
    }

    [RelayCommand]
    private void Logout()
    {
        ServiceLocator.Session.Clear();
        App.ShowLogin();
    }
}
