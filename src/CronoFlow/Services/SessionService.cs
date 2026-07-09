using TimeFlow.Models;

namespace TimeFlow.Services;

public interface ISessionService
{
    User? CurrentUser { get; }
    bool IsManager { get; }
    void SetUser(User user);
    void Clear();
}

public class SessionService : ISessionService
{
    public User? CurrentUser { get; private set; }
    public bool IsManager => CurrentUser?.Role == UserRole.Manager;

    public void SetUser(User user) => CurrentUser = user;
    public void Clear() => CurrentUser = null;
}
