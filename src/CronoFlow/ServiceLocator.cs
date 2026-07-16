using CronoFlow.Data;
using CronoFlow.Services;

namespace CronoFlow;

public static class ServiceLocator
{
    private static CronoFlowDbContext? _db;
    private static ISessionService? _session;
    private static IAuthService? _auth;
    private static ITimerService? _timer;
    private static ITaskService? _tasks;
    private static IReportService? _reports;
    private static IActionLogService? _actionLog;

    // 1. Adicione a variável do UserService
    private static IUserService? _users;

    public static void Initialize()
    {
        _db = new CronoFlowDbContext();
        DatabaseInitializer.Initialize(_db);
        _session = new SessionService();
        _auth = new AuthService(_db);
        _timer = new TimerService(_db);
        _tasks = new TaskService(_db);
        _reports = new ReportService(_db);
        _actionLog = new ActionLogService(_db);

        // 2. Inicialize o UserService
        _users = new UserService(_db);
    }

    public static ISessionService Session => _session!;
    public static IAuthService Auth => _auth!;
    public static ITimerService Timer => _timer!;
    public static ITaskService Tasks => _tasks!;
    public static IReportService Reports => _reports!;
    public static IActionLogService ActionLog => _actionLog!;

    // 3. Exponha a propriedade Users
    public static IUserService Users => _users!;

    public static CronoFlowDbContext Db => _db!;
}