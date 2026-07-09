using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Models;
using TimeFlow.Services;

namespace TimeFlow.ViewModels;

public partial class TimerViewModel : ViewModelBase
{
    private readonly System.Timers.Timer _uiTimer;

    [ObservableProperty]
    private ObservableCollection<WorkTask> _availableTasks = [];

    [ObservableProperty]
    private WorkTask? _selectedTask;

    [ObservableProperty]
    private string _elapsedDisplay = "00:00";

    [ObservableProperty]
    private string _activeTaskTitle = "Nenhuma tarefa ativa";

    [ObservableProperty]
    private bool _hasActiveTimer;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public TimerViewModel()
    {
        _uiTimer = new System.Timers.Timer(1000);
        _uiTimer.Elapsed += (_, _) => UpdateDisplay();
        _uiTimer.AutoReset = true;

        ServiceLocator.Timer.TimerChanged += OnTimerChanged;
    }

    public async Task InitializeAsync()
    {
        var user = ServiceLocator.Session.CurrentUser!;
        var tasks = await ServiceLocator.Tasks.GetTasksForUserAsync(user.Id, ServiceLocator.Session.IsManager);
        AvailableTasks = new ObservableCollection<WorkTask>(tasks.Where(t => t.Status == WorkTaskStatus.Active));
        RefreshFromService();
    }

    private void OnTimerChanged() => RefreshFromService();

    private void RefreshFromService()
    {
        var snapshot = ServiceLocator.Timer.Current;
        HasActiveTimer = snapshot is not null;
        IsRunning = snapshot?.State == TimerState.Running;
        IsPaused = snapshot?.State == TimerState.Paused;

        if (snapshot is not null)
        {
            ActiveTaskTitle = snapshot.TaskTitle;
            ElapsedDisplay = TimeFormatter.FormatDuration(snapshot.ElapsedSeconds);
            SelectedTask = AvailableTasks.FirstOrDefault(t => t.Id == snapshot.TaskId);

            if (IsRunning && !_uiTimer.Enabled)
                _uiTimer.Start();
            else if (!IsRunning)
                _uiTimer.Stop();
        }
        else
        {
            ActiveTaskTitle = "Nenhuma tarefa ativa";
            ElapsedDisplay = "00:00";
            _uiTimer.Stop();
        }
    }

    private void UpdateDisplay()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var snapshot = ServiceLocator.Timer.Current;
            if (snapshot is not null && snapshot.State == TimerState.Running)
                ElapsedDisplay = TimeFormatter.FormatDuration(snapshot.ElapsedSeconds);
        });
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        StatusMessage = string.Empty;
        if (SelectedTask is null)
        {
            StatusMessage = "Selecione uma tarefa.";
            return;
        }

        try
        {
            var userId = ServiceLocator.Session.CurrentUser!.Id;
            await ServiceLocator.Timer.StartAsync(userId, SelectedTask.Id);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task PauseAsync()
    {
        StatusMessage = string.Empty;
        await ServiceLocator.Timer.PauseAsync(ServiceLocator.Session.CurrentUser!.Id);
    }

    [RelayCommand]
    private async Task ResumeAsync()
    {
        StatusMessage = string.Empty;
        await ServiceLocator.Timer.ResumeAsync(ServiceLocator.Session.CurrentUser!.Id);
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        StatusMessage = string.Empty;
        var entry = await ServiceLocator.Timer.StopAsync(ServiceLocator.Session.CurrentUser!.Id);
        if (entry is not null)
            StatusMessage = $"Tempo registrado: {TimeFormatter.FormatDuration(entry.DurationSeconds)}";
    }
}
