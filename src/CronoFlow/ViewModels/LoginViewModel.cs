using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Models;

namespace TimeFlow.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public event Action<User>? LoginSucceeded;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Informe usuário e senha.";
            return;
        }

        IsLoading = true;
        try
        {
            var user = await ServiceLocator.Auth.LoginAsync(Username.Trim(), Password);
            if (user is null)
            {
                ErrorMessage = "Credenciais inválidas.";
                return;
            }

            ServiceLocator.Session.SetUser(user);
            await ServiceLocator.Timer.RecoverAsync(user.Id);
            LoginSucceeded?.Invoke(user);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
