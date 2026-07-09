using Microsoft.EntityFrameworkCore;
using TimeFlow.Data;
using TimeFlow.Models;

namespace TimeFlow.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string username, string password);
    Task<IReadOnlyList<User>> GetAllEmployeesAsync();
}

public class AuthService(TimeFlowDbContext db) : IAuthService
{
    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        return user;
    }

    public async Task<IReadOnlyList<User>> GetAllEmployeesAsync()
        => await db.Users.OrderBy(u => u.DisplayName).ToListAsync();
}
