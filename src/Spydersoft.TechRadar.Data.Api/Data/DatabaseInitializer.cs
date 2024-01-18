using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spydersoft.TechRadar.Data.Api.Data
{
    /// <summary>
    /// Class DatabaseInitializer.
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// The application
        /// </summary>
        private readonly IApplicationBuilder _app;
        /// <summary>
        /// The log
        /// </summary>
        private ILogger? _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInitializer"/> class.
        /// </summary>
        /// <param name="app">The application.</param>
        public DatabaseInitializer(IApplicationBuilder app)
        {
            _app = app;
        }

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Unable to get IServiceScopeFactory</exception>
        public void InitializeDatabase()
        {
            var scopeFactory = _app.ApplicationServices.GetService<IServiceScopeFactory>();

            if (scopeFactory == null)
            {
                throw new NullReferenceException("Unable to get IServiceScopeFactory");
            }
            using IServiceScope serviceScope = scopeFactory.CreateScope();
            _log = serviceScope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();

            PerformDatabaseMigrations(serviceScope);
            SeedDatabase(serviceScope);
        }

        #region Database Migration Methods

        /// <summary>
        /// Performs the database migrations.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        private void PerformDatabaseMigrations(IServiceScope serviceScope)
        {
            Task techRadarTask =
                DoMigrationIfNeeded(serviceScope.ServiceProvider.GetRequiredService<TechRadarContext>(), "Tech Radar Database");

            Task.WaitAll(techRadarTask);
        }

        /// <summary>
        /// Does the migration if needed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="databaseName">Name of the database.</param>
        private async Task DoMigrationIfNeeded(DbContext context, string databaseName)
        {
            _log?.LogDebug("Checking {database} for pending migrations.", databaseName);
            try
            {
                var hasMigrations = (await context.Database.GetPendingMigrationsAsync()).Any();
                if (hasMigrations)
                {
                    _log?.LogInformation("Migrating {database}.", databaseName);
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, "Error migrating {database}.", databaseName);
                throw;
            }

        }

        #endregion Database Migration Methods

        #region Seeding Functions

        /// <summary>
        /// Seeds the database.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        private void SeedDatabase(IServiceScope serviceScope)
        {

        }
        #endregion Seeding Functions
    }
}
