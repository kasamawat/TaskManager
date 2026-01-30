using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Infrastructure.Auth;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Application.Validator.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Diagnostics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// allow
var allowedOrigins = new[] { "http://localhost:5173", "http://127.0.0.1:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)   // ระบุ origin ที่อนุญาต (ห้ามใช้ "*" ถ้า AllowCredentials)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();          // ถ้าใช้ cookie หรือส่ง credentials
    });
});

// ================== Serilog bootstrap ==================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithThreadId()
    .WriteTo.Console(new RenderedCompactJsonFormatter()) // structured JSON
    .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/log-.json", rollingInterval: RollingInterval.Day)
    // Seq (optional) - set via appsettings
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url", "http://localhost:5341"))
    .CreateLogger();

builder.Host.UseSerilog();

// ================== Health checks (SQL example) ==================
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddSqlServer(conn, name: "sqlserver");

// ================== OpenTelemetry Tracing (Jaeger) ==================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("TaskManager.Api"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            .AddSource("TaskManager")   // optional หากคุณใช้ ActivitySource เอง
            .AddJaegerExporter(jaegerOptions =>
            {
                jaegerOptions.AgentHost = builder.Configuration.GetValue<string>("Jaeger:Host", "localhost");
                jaegerOptions.AgentPort = builder.Configuration.GetValue<int>("Jaeger:Port", 6831);
            });
    });


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// Connection string Db Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Auth Config
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// DI for Infratructure
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectReportRepository, ProjectReportRepository>();
builder.Services.AddScoped<IUserProjectOverviewRepository, UserProjectOverviewRepository>();

// DI for Application Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectReportService, ProjectReportService>();
builder.Services.AddScoped<IUserProjectOverviewService, UserProjectOverviewService>();

// add memory cache
builder.Services.AddMemoryCache();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // add Security Schema of JWT Bearer
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "ใส่ JWT Token ในรูปแบบ: Bearer {token}",
    });
    // Required every endpoint support this Security
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            new string[] {}
        }
    });
});

// Build App
var app = builder.Build();

// ================== Use Serilog request logging (automatic request start/stop logs) ==================
app.UseSerilogRequestLogging(options =>
{
    // enrich from http context if needed
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var cid))
            diagnosticContext.Set("CorrelationId", cid.ToString());
    };
});

// ================== Global exception handler that logs exceptions ==================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exFeature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = exFeature?.Error;
        Log.Error(ex, "Unhandled exception occurred while processing request");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred."
        });
    });
});

// ================== Correlation ID middleware (adds X-Correlation-ID header) ==================
app.Use(async (context, next) =>
{
    const string headerKey = "X-Correlation-ID";
    if (!context.Request.Headers.TryGetValue(headerKey, out var correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
        context.Request.Headers[headerKey] = correlationId;
    }

    context.Response.OnStarting(() =>
    {
        if (!context.Response.Headers.ContainsKey(headerKey))
            context.Response.Headers[headerKey] = correlationId.ToString();
        return Task.CompletedTask;
    });

    using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId.ToString()))
    {
        await next();
    }
});

// Routing & Metrics
app.UseRouting();

// Prometheus: record HTTP metrics (prometheus-net)
app.UseHttpMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ใช้ CORS ก่อน auth / endpoints
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // Health check endpoint
    endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (ctx, report) =>
        {
            var json = new
            {
                status = report.Status.ToString(),
                details = report.Entries.ToDictionary(k => k.Key, v => new { v.Value.Status, v.Value.Description })
            };
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(json);
        }
    });

    // Prometheus scrape endpoint (default /metrics)
    endpoints.MapMetrics();
});

app.MapControllers();

app.Run();
