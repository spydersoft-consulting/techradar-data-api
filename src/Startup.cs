﻿// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="Startup.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Spydersoft.TechRadar.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Spydersoft.TechRadar.Api.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Spydersoft.TechRadar.Api.Configuration;
using Spydersoft.TechRadar.Api.Options;

namespace Spydersoft.TechRadar.Api
{
    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var telemetryOptions = new OpenTelemetryOptions();
            Configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(telemetryOptions);

            var identityOption = new IdentityOptions();
            Configuration.GetSection(IdentityOptions.SectionName).Bind(identityOption);

            services.AddOpenTelemetry().ConfigureOpenTelemetry(telemetryOptions);                        

            services.AddMvcCore().AddApiExplorer();
           
            services.AddDbContext<TechRadarContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("TechRadarDatabase")));

            services.AddScoped<IRadarService, RadarService>();
            services.AddScoped<IRadarDataItemService, RadarDataItemService>();
            services.AddScoped<ITagService, TagService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });


            if (identityOption.Authority != null)
            {
                services
                    .AddAuthentication(o =>
                    {
                        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(o =>
                    {
                        o.IncludeErrorDetails = true;

                        o.Authority = identityOption.Authority;
                        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidAudiences = new List<string>
                            {
                                identityOption.ApplicationName ?? "TechRadar"
                            },
                            ValidIssuers = new List<string>
                            {
                                identityOption.Authority
                            }
                        };
                    });
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tech Radar API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment()) {
                app.UseCors("AllowAll");
            }
            
            app.UseAuthentication()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseSpaStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger()
                .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tech Radar API");
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
                else
                {
                    spa.Options.SourcePath = "ClientApp";
                }

            });

            IdentityModelEventSource.ShowPII = env.IsDevelopment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
        }
    }
}