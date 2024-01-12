// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="Program.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Microsoft.Extensions.Logging.Configuration;
using Serilog;
using Spydersoft.TechRadar.Api;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

try {
    Log.Information("TechRadar starting.");
    var config = builder.Configuration;

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
    });

    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    var app = builder.Build();
    startup.Configure(app, app.Environment);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SpyderSoft.TechRadar.Api failed to start.");
}
finally
{
    Log.Information("ASpyderSoft.TechRadar.Api shut down complete");
    Log.CloseAndFlush();
}
