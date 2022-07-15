# GodelTech.Database.EntityFrameworkCore

Library to work with database, apply migrations and data using Entity Framework Core

## Overview

Using this library you can create console app with database migrations and data.

`DatabaseService.cs`
```csharp
public class DatabaseService : DatabaseServiceBase
{
    public DatabaseService(
        BaseContext baseContext,
        WeatherContext weatherContext,
        IHostEnvironment hostEnvironment,
        ILogger<DatabaseService> logger)
        : base(logger, baseContext, weatherContext)
    {
        RegisterDataService(
            new DataService<WeatherEntity, Guid>(
                new ConfigurationBuilder(),
                hostEnvironment,
                @"Data\Weather",
                weatherContext,
                false,
                x => x.Id,
                logger
            )
        );
    }
}
```

`Worker.cs`
```csharp
public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private static readonly Action<ILogger, Exception> LogExecuteAsyncApplyDatabaseMigrationsInformation =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(ExecuteAsync)),
            "Apply database migrations"
        );

    private static readonly Action<ILogger, Exception> LogExecuteAsyncApplyDatabaseDataInformation =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(0, nameof(ExecuteAsync)),
            "Apply database data"
        );

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseService>();

        LogExecuteAsyncApplyDatabaseMigrationsInformation(_logger, null);
        await databaseService.ApplyMigrationsAsync();

        LogExecuteAsyncApplyDatabaseDataInformation(_logger, null);
        await databaseService.ApplyDataAsync();
    }
}
```
