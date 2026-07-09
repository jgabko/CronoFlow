using Avalonia.Controls;
using TimeFlow.ViewModels;

namespace TimeFlow.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        var vm = new LoginViewModel();
        vm.LoginSucceeded += user => App.OnLoginSuccess(user);
        DataContext = vm;
    }
}
