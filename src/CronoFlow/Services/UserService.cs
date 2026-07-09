using Microsoft.EntityFrameworkCore;
using CronoFlow.Data;
using CronoFlow.Models;

namespace CronoFlow.Services;

public record UserListItem(
    int Id,
    string Username,
    string DisplayName,
    UserRole Role,
    bool IsActive,
    int AssignedTasks,
    long TotalSeconds);

public interface IUserService
{
    Task<IReadOnlyList<UserListItem>> GetAllUsersAsync();
    Task<IReadOnlyList<User>> GetActiveEmployeesAsync();
    Task<User> CreateUserAsync(string username, string displayName, string password, UserRole role);
    Task UpdateUserAsync(int id, string username, string displayName, UserRole role);
    Task ResetPasswordAsync(int id, string newPassword);
    Task SetActiveAsync(int id, bool isActive);
}

public class UserService(CronoFlowDbContext db) : IUserService
{
    public async Task<IReadOnlyList<UserListItem>> GetAllUsersAsync()
    {
        var users = await db.Users.OrderBy(u => u.DisplayName).ToListAsync();
        var taskCounts = await db.TaskAssignments
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var timeTotals = await db.TimeEntries
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(e => e.DurationSeconds) })
            .ToDictionaryAsync(x => x.UserId, x => x.Total);

        return users.Select(u => new UserListItem(
            u.Id,
            u.Username,
            u.DisplayName,
            u.Role,
            u.IsActive,
            taskCounts.GetValueOrDefault(u.Id),
            timeTotals.GetValueOrDefault(u.Id))).ToList();
    }

    public async Task<IReadOnlyList<User>> GetActiveEmployeesAsync()
        => await db.Users
            .Where(u => u.IsActive && u.Role == UserRole.Employee)
            .OrderBy(u => u.DisplayName)
            .ToListAsync();

    public async Task<User> CreateUserAsync(string username, string displayName, string password, UserRole role)
    {
        username = username.Trim().ToLowerInvariant();
        displayName = displayName.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(displayName))
            throw new InvalidOperationException("Usuário e nome são obrigatórios.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new InvalidOperationException("A senha deve ter no mínimo 6 caracteres.");

        if (await db.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("Este nome de usuário já existe.");

        var user = new User
        {
            Username = username,
            DisplayName = displayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            IsActive = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(int id, string username, string displayName, UserRole role)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        username = username.Trim().ToLowerInvariant();
        displayName = displayName.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(displayName))
            throw new InvalidOperationException("Usuário e nome são obrigatórios.");

        if (await db.Users.AnyAsync(u => u.Username == username && u.Id != id))
            throw new InvalidOperationException("Este nome de usuário já existe.");

        user.Username = username;
        user.DisplayName = displayName;
        user.Role = role;
        await db.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(int id, string newPassword)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new InvalidOperationException("A senha deve ter no mínimo 6 caracteres.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        var currentUserId = ServiceLocator.Session.CurrentUser?.Id;
        if (currentUserId == id && !isActive)
            throw new InvalidOperationException("Você não pode desativar sua própria conta.");

        var user = await db.Users.FindAsync(id)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (user.Role == UserRole.Manager && !isActive)
        {
            var activeManagers = await db.Users.CountAsync(u =>
                u.Role == UserRole.Manager && u.IsActive && u.Id != id);
            if (activeManagers == 0)
                throw new InvalidOperationException("Deve existir pelo menos um gerente ativo.");
        }

        user.IsActive = isActive;
        await db.SaveChangesAsync();
    }
}
