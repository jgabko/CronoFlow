using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeFlow.Models;
using TimeFlow.Services;

namespace TimeFlow.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<UserListItem> _users = [];

    [ObservableProperty]
    private UserListItem? _selectedUser;

    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private string _formUsername = string.Empty;

    [ObservableProperty]
    private string _formDisplayName = string.Empty;

    [ObservableProperty]
    private string _formPassword = string.Empty;

    [ObservableProperty]
    private UserRole _formRole = UserRole.Employee;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isError;

    public UserRole[] AvailableRoles { get; } = [UserRole.Employee, UserRole.Manager];

    public async Task InitializeAsync() => await RefreshAsync();

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var users = await ServiceLocator.Users.GetAllUsersAsync();
        Users = new ObservableCollection<UserListItem>(users);
    }

    [RelayCommand]
    private void NewUser()
    {
        ClearStatus();
        SelectedUser = null;
        IsEditing = false;
        FormTitle = "Novo usuário";
        FormUsername = string.Empty;
        FormDisplayName = string.Empty;
        FormPassword = string.Empty;
        FormRole = UserRole.Employee;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void EditUser(UserListItem? user)
    {
        if (user is null) return;

        ClearStatus();
        SelectedUser = user;
        IsEditing = true;
        FormTitle = "Editar usuário";
        FormUsername = user.Username;
        FormDisplayName = user.DisplayName;
        FormPassword = string.Empty;
        FormRole = user.Role;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void CancelForm()
    {
        IsFormVisible = false;
        SelectedUser = null;
        ClearStatus();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ClearStatus();
        try
        {
            if (IsEditing && SelectedUser is not null)
            {
                await ServiceLocator.Users.UpdateUserAsync(
                    SelectedUser.Id, FormUsername, FormDisplayName, FormRole);

                if (!string.IsNullOrWhiteSpace(FormPassword))
                    await ServiceLocator.Users.ResetPasswordAsync(SelectedUser.Id, FormPassword);

                SetStatus("Usuário atualizado com sucesso.");
            }
            else
            {
                await ServiceLocator.Users.CreateUserAsync(
                    FormUsername, FormDisplayName, FormPassword, FormRole);
                SetStatus("Usuário criado com sucesso.");
            }

            IsFormVisible = false;
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, isError: true);
        }
    }

    [RelayCommand]
    private async Task ToggleActiveAsync(UserListItem? user)
    {
        if (user is null) return;

        ClearStatus();
        try
        {
            await ServiceLocator.Users.SetActiveAsync(user.Id, !user.IsActive);
            SetStatus(user.IsActive ? "Usuário desativado." : "Usuário reativado.");
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, isError: true);
        }
    }

    private void SetStatus(string message, bool isError = false)
    {
        StatusMessage = message;
        IsError = isError;
    }

    private void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsError = false;
    }
}
