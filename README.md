# CronoFlow

Aplicativo desktop de **cronoanálise corporativa** para rastreamento de tempo e análise de desempenho de equipes. Interface inspirada no Clockify.

## Stack

- **.NET 10** + **Avalonia UI** (cross-platform)
- **SQLite** + Entity Framework Core
- **MVVM** (CommunityToolkit.Mvvm)

## Funcionalidades

- Login com controle de acesso (Gerente / Funcionário)
- Cronômetro com Iniciar, Pausar e Parar
- Mini-player **Always on Top**
- Recuperação de tempo após crash (timestamps persistidos)
- Dashboards de desempenho individual, por tarefa e da equipe

## Executar

```bash
cd src/CronoFlow
dotnet run
```

## Contas de demonstração

| Papel        | Usuário  | Senha       |
|-------------|----------|-------------|
| Gerente     | gerente  | gerente123  |
| Funcionário | joao     | joao123     |
| Funcionário | maria    | maria123    |

## Banco de dados

SQLite em `~/.local/share/CronoFlow/cronoflow.db`
