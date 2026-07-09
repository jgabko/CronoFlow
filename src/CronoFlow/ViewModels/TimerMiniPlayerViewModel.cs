using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Models;
using TimeFlow.Services;
using TimeFlow.Views;

namespace TimeFlow.ViewModels;

public partial class TimerMiniPlayerViewModel : ViewModelBase
{
    private readonly System.Timers.Timer _uiTimer;

    [ObservableProperty]
    private string _taskTitle = "—";

    [ObservableProperty]
    private string _elapsedDisplay = "00:00";

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private bool _alwaysOnTop = true;

    public TimerMiniPlayerViewModel()
    {
        _uiTimer = new System.Timers.Timer(1000);
        _uiTimer.Elapsed += (_, _) => UpdateDisplay();
        _uiTimer.AutoReset = true;
        ServiceLocator.Timer.TimerChanged += Refresh;
    }

    public void Refresh()
    {
        var snapshot = ServiceLocator.Timer.Current;
        IsVisible = snapshot is not null;

        if (snapshot is null)
        {
            _uiTimer.Stop();
            return;
        }

        TaskTitle = snapshot.TaskTitle;
        ElapsedDisplay = TimeFormatter.FormatDuration(snapshot.ElapsedSeconds);
        IsRunning = snapshot.State == TimerState.Running;

        if (IsRunning && !_uiTimer.Enabled)
            _uiTimer.Start();
        else if (!IsRunning)
            _uiTimer.Stop();
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
    private async Task PauseOrResumeAsync()
    {
        var userId = ServiceLocator.Session.CurrentUser!.Id;
        var snapshot = ServiceLocator.Timer.Current;
        if (snapshot is null) return;

        if (snapshot.State == TimerState.Running)
            await ServiceLocator.Timer.PauseAsync(userId);
        else
            await ServiceLocator.Timer.ResumeAsync(userId);
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        await ServiceLocator.Timer.StopAsync(ServiceLocator.Session.CurrentUser!.Id);
    }

    partial void OnAlwaysOnTopChanged(bool value)
    {
        if (MiniPlayerWindow.Instance is not null)
            MiniPlayerWindow.Instance.Topmost = value;
    }
}
