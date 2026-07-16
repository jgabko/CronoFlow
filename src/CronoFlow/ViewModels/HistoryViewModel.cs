using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CronoFlow.Models;
using CronoFlow.Services;

namespace CronoFlow.ViewModels;

public partial class HistoryViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<TimerActionLogEntry> _entries = [];

    [ObservableProperty]
    private ObservableCollection<User> _employees = [];

    [ObservableProperty]
    private User? _selectedEmployee;

    public bool IsManager => ServiceLocator.Session.IsManager;

    public async Task InitializeAsync()
    {
        if (IsManager)
        {
            var all = await ServiceLocator.Auth.GetAllEmployeesAsync();
            Employees = new ObservableCollection<User>(all.Where(u => u.Role == UserRole.Employee));
        }

        await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var user = ServiceLocator.Session.CurrentUser!;
        var logs = await ServiceLocator.ActionLog.GetLogsAsync(
            user.Id, IsManager, filterUserId: SelectedEmployee?.Id);

        Entries = new ObservableCollection<TimerActionLogEntry>(logs);
    }

    [RelayCommand]
    private async Task ClearFilterAsync()
    {
        SelectedEmployee = null;
        await RefreshAsync();
    }
}