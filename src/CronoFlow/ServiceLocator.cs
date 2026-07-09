using TimeFlow.Data;
using TimeFlow.Services;

namespace TimeFlow;

public static class ServiceLocator
{
    private static TimeFlowDbContext? _db;
    private static ISessionService? _session;
    private static IAuthService? _auth;
    private static ITimerService? _timer;
    private static ITaskService? _tasks;
    private static IReportService? _reports;

    public static void Initialize()
    {
        _db = new TimeFlowDbContext();
        DatabaseInitializer.Initialize(_db);
        _session = new SessionService();
        _auth = new AuthService(_db);
        _timer = new TimerService(_db);
        _tasks = new TaskService(_db);
        _reports = new ReportService(_db);
    }

    public static ISessionService Session => _session!;
    public static IAuthService Auth => _auth!;
    public static ITimerService Timer => _timer!;
    public static ITaskService Tasks => _tasks!;
    public static IReportService Reports => _reports!;
    public static TimeFlowDbContext Db => _db!;
}
