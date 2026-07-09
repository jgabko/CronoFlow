using Microsoft.EntityFrameworkCore;
using CronoFlow.Models;

namespace CronoFlow.Data;

public static class DatabaseInitializer
{
    public static void Initialize(CronoFlowDbContext db)
    {
        db.Database.EnsureCreated();
        ApplySchemaUpdates(db);

        if (db.Users.Any())
            return;

        var manager = new User
        {
            Username = "gerente",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("gerente123"),
            DisplayName = "Ana Gerente",
            Role = UserRole.Manager
        };

        var emp1 = new User
        {
            Username = "joao",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("joao123"),
            DisplayName = "João Silva",
            Role = UserRole.Employee
        };

        var emp2 = new User
        {
            Username = "maria",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("maria123"),
            DisplayName = "Maria Santos",
            Role = UserRole.Employee
        };

        db.Users.AddRange(manager, emp1, emp2);

        var task1 = new WorkTask
        {
            Title = "Relatório Mensal",
            Description = "Consolidar dados de produtividade do mês",
            Status = WorkTaskStatus.Active
        };

        var task2 = new WorkTask
        {
            Title = "Revisão de Processos",
            Description = "Mapear fluxos operacionais do setor",
            Status = WorkTaskStatus.Active
        };

        var task3 = new WorkTask
        {
            Title = "Treinamento de Equipe",
            Description = "Capacitação em novas ferramentas",
            Status = WorkTaskStatus.Active
        };

        db.Tasks.AddRange(task1, task2, task3);
        db.SaveChanges();

        db.TaskAssignments.AddRange(
            new TaskAssignment { TaskId = task1.Id, UserId = emp1.Id },
            new TaskAssignment { TaskId = task1.Id, UserId = emp2.Id },
            new TaskAssignment { TaskId = task2.Id, UserId = emp1.Id },
            new TaskAssignment { TaskId = task3.Id, UserId = emp2.Id });

        db.SaveChanges();
    }

    private static void ApplySchemaUpdates(CronoFlowDbContext db)
    {
        try
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE Users ADD COLUMN IsActive INTEGER NOT NULL DEFAULT 1");
        }
        catch
        {
            // Column already exists.
        }
    }
}
