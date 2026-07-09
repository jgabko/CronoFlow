using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TimeFlow.Models;
using TimeFlow.ViewModels;
using TimeFlow.Views;

namespace TimeFlow;

public partial class App : Application
{
    private MainWindow? _mainWindow;
    private MiniPlayerWindow? _miniPlayer;
    private LoginWindow? _loginWindow;
    private MainViewModel? _mainViewModel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ServiceLocator.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            ShowLogin();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void ShowLogin()
    {
        if (Current is not App app)
            return;

        app._mainWindow?.Close();
        app._miniPlayer?.Close();
        app._mainWindow = null;
        app._miniPlayer = null;
        app._mainViewModel = null;

        app._loginWindow = new LoginWindow();
        app._loginWindow.Closed += (_, _) =>
        {
            if (app._mainWindow is null &&
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            {
                d.Shutdown();
            }
        };
        app._loginWindow.Show();
    }

    public static async void OnLoginSuccess(User user)
    {
        if (Current is not App app)
            return;

        app._loginWindow?.Close();
        app._loginWindow = null;

        app._mainViewModel = new MainViewModel();
        await app._mainViewModel.InitializeAsync(user);

        app._mainWindow = new MainWindow { DataContext = app._mainViewModel };
        app._mainWindow.Closed += (_, _) =>
        {
            app._miniPlayer?.Close();
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
                d.Shutdown();
        };
        app._mainWindow.Show();

        app._miniPlayer = new MiniPlayerWindow { DataContext = app._mainViewModel.MiniPlayer };
        ServiceLocator.Timer.TimerChanged += app._mainViewModel.MiniPlayer.Refresh;
        app._mainViewModel.MiniPlayer.Refresh();

        if (app._mainViewModel.MiniPlayer.IsVisible)
            app._miniPlayer.Show();
        else
            app._miniPlayer.Hide();

        app._mainViewModel.MiniPlayer.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TimerMiniPlayerViewModel.IsVisible) && app._miniPlayer is not null)
            {
                if (app._mainViewModel.MiniPlayer.IsVisible)
                    app._miniPlayer.Show();
                else
                    app._miniPlayer.Hide();
            }
        };
    }
}
