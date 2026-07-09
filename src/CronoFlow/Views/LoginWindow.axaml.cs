using Avalonia.Controls;
using CronoFlow.ViewModels;

namespace CronoFlow.Views;

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
