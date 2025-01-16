// System
using System.Net;
using System.Text;

// Third-Party Libraries
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;

// Audit log
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;

// Domain/Core layer
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Domain.Common.HATEOAS;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Common.AuditLog;

// Application layer
using TaskManagement.Application.Services;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Application.Utilities.AuditLog;
using TaskManagement.Application.Utilities.HATEOAS;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.Validation.AppUser;
using TaskManagement.Application.Validation.EntityPrototype;
using TaskManagement.Application.Validation.Workspace;
using TaskManagement.Application.Validation.Project;
using TaskManagement.Application.Validation.Task;
using TaskManagement.Application.Validation.Issue;

// Presenter layer
using TaskManagement.Presenter.RouteTransformer;
using TaskManagement.Presenter.Filters;
using TaskManagement.Presenter.Middlewares;

// Infrastructure layer
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Infrastructure.AppSettings;
using TaskManagement.Infrastructure.ExceptionHandler;

// Authentication and Authorization
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity for user authentication and role management
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT Bearer token authentication handler for secure API endpoints
using Microsoft.IdentityModel.Tokens; // Token-based authentication and validation utilities
using System.Security.Claims; // Types for working with user claims, commonly used in JWTs

// EntityFrameworkCore
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

// Other
using Microsoft.AspNetCore.Mvc.ApplicationModels; // Provides tools to apply conventions and customize routing or action selection in controllers.
using Microsoft.OpenApi.Models; // Used for configuring Swagger documentation for APIs.
using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.DTOs.Issue;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Domain.Common.ReturnType;

try
{
    #region App settings & Serilog

    var configuration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .Build();

    EmailConfig? emailConfig = configuration.GetRequiredSection("EmailConfig").Get<EmailConfig>();

    var emailInfo = new EmailSinkOptions
    {
        Subject = new MessageTemplateTextFormatter(emailConfig.EmailSubject, null),
        Port = emailConfig.Port,
        From = emailConfig.FromEmail,
        To = new List<string>() { emailConfig.ToEmail },
        Host = emailConfig.MailServer,
        //EnableSsl = appSettings.EnableSsl,
        Credentials = new NetworkCredential(emailConfig.FromEmail, emailConfig.EmailPassword)
    };

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        //.WriteTo.Email(emailInfo)                           
        .CreateLogger();

    #endregion

    var builder = WebApplication.CreateBuilder(args);

    #region Register CORS services

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("https://localhost:3000", "http://localhost:3000")   // Add allowed origins
                  .AllowAnyHeader()                                                 // Allow any headers (e.g., Authorization)
                  .AllowAnyMethod() // Allow any HTTP methods (GET, POST, etc.)
                  .AllowCredentials(); 
        });

        // Optional: Allow all origins (not recommended in production)
        options.AddPolicy("AllowAllOrigins", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    #endregion

    #region Controllers & Endpoints

    //builder.Services.AddControllers();
    builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SpinCaseTransformer()));
        options.Filters.Add<InputValidationAttribute>();
    }).AddNewtonsoftJson();
    //.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(UserController).Assembly));

    builder.Services.AddEndpointsApiExplorer();

    #endregion

    #region Swagger UI

    /* An error occurs if XML documentation generation is not enabled.
         Ensure that XML documentation generation is enabled for your project.
            1. Right-click on your project in Visual Studio.
            2. Select Properties.
            3. Go to the Build tab.
            4. Check the documentation file checkbox.
            5. Verify that the path specified matches what you expect (bin\Debug\net8.0\BookInformationService.xml)
    */

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1,v2", // API versions
            Title = "TaskManagement.CleanArchitecture",
            Description = "Task Management System Using ASP.NET Core Web API, Identity and PostgreSQL with Clean Architecture"
        });

        // Add JWT Bearer Authorization in Swagger UI
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer abc123def456\""
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
            });
    });

    #endregion

    #region Add API Versioning

    // Package: Asp.Versioning.Mvc & Asp.Versioning.Mvc.ApiExplorer
    builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true; // Include the version information in the response headers
        options.AssumeDefaultVersionWhenUnspecified = true; // Default to the specified version when not supplied
        options.DefaultApiVersion = new ApiVersion(1, 0);
        //options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Use URL segments for versioning

        options.ApiVersionReader = ApiVersionReader.Combine(
                new MediaTypeApiVersionReader("ver")
            );

        //options.ApiVersionReader = ApiVersionReader.Combine(
        //    new QueryStringApiVersionReader("api-version"),   // GET /api/v1/User?api-version=2.0
        //    new HeaderApiVersionReader("x-version"),          // GET /api/v1/User
        //                                                      // x - version: 2.0
        //    new MediaTypeApiVersionReader("ver")              // GET /api/v1/User
        //                                                      // Accept: application / json; ver = 2.0
        //    );
    })
        .AddMvc()                       // API version-aware extensions for MVC Core with controllers (not full MVC)
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";         // Format the version as 'v1', 'v2', etc.
            options.SubstituteApiVersionInUrl = true;   // Substitute the version in the URL
        }); // Add API Versioned Explorer to support versioned API documentation in Swagger

    #endregion

    #region Add Entity Framework Core with SQLite

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            opt => opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AppDbContext")) // Set name of EF History on DB
                                                                                                   // .UseSnakeCaseNamingConvention() // For PostgreSQL database table naming
        .EnableSensitiveDataLogging() // Optional: for detailed. logs not in production
        .EnableDetailedErrors() // not in production 
        .LogTo(Console.WriteLine, LogLevel.Information));


    /* lazy loading, eager loading and Explicit Loading
    // Production: Use DbContextPool with Npgsql
    // <PackageReference Include="Microsoft.EntityFrameworkCore.Proxies" Version="8.0.0" />
    builder.Services.AddDbContextPool<AppDbContext>(
        options => options
            .UseLazyLoadingProxies() // Lazy loading for [virtual] navigation properties
            .UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.CommandTimeout(30)
            )
            .EnableSensitiveDataLogging() // Disable in production
            .UseSnakeCaseNamingConvention(),
        poolSize: 64
    );
    */

    #endregion

    #region Add Caching

    builder.Services.AddMemoryCache(options =>
    {
        options.SizeLimit = 1024; // Limit cache size (optional, helps prevent memory overuse)
        options.ExpirationScanFrequency = TimeSpan.FromMinutes(1); // Period to scan for expired items
    });

    //builder.Services.AddHybridCache(); // I have to use later after releasing fnal version of .Net 9

    #endregion

    #region Authentication - JWT and Authorization - Role, Claim or Policy

    // Access JwtSettings from appsettings.json
    JwtSettings jwtSettings = configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>();
    // Jwt Settings register service
    builder.Services.AddSingleton<JwtSettings>(jwtSettings);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    //.AddCookie(IdentityConstants.ApplicationScheme)
    .AddJwtBearer("Identity.Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "Issuer",
            ValidAudience = "Audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2d3f4g5h6j7k8l9m0n1p2q3r4s5t6u7v8w9x0y1z2a3b4c5d6e7f8g9h0i1j2k")),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                var user = await userManager.FindByNameAsync(context.Principal.Identity.Name);

                if (user != null)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));
                    var identity = context.Principal.Identity as ClaimsIdentity;
                    identity.AddClaims(claims);
                }
            }
        };
    });

    // Add Identity
    builder.Services.AddIdentity<AppUser, AppRole>(options =>
    {
        // Configure identity options if needed
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<AppDbContext>() // Tells Identity to use AppDbContext for storage
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Package: Microsoft.AspNetCore.Identity.UI

    #endregion

    #region Exception handler

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Messages = e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                })
                .ToArray();

            var response = ApiResponse.Failure(
                errors: errors.SelectMany(e => e.Messages.Select(m => new Error(400, e.Field, m))).ToArray(),
                message: "Validation Failed"
            );

            return new BadRequestObjectResult(response);
        };
    });

    #endregion

    #region Register services for Dependency Injection

    // Middleware
    builder.Services.AddTransient<LoggingMiddleware>();

    // Activity log
    builder.Services.AddSingleton<IActivityLog, ActivityLog>();

    // HATEOAS
    builder.Services.AddScoped<IEntityLinkGenerator, EntityLinkGenerator>();

    // Business & Data Access Layer
    builder.Services.AddScoped<IAppUserService, AppUserService>();
    builder.Services.AddScoped<IAppRoleService, AppRoleService>();

    //builder.Services.AddScoped<AppUserService>();
    //builder.Services.AddScoped<AppRoleService>();


    builder.Services.AddScoped<IEntityPrototypeRepository, EntityPrototypeRepository>();
    builder.Services.AddScoped<IEntityPrototypeService, EntityPrototypeService>();

    builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
    builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();

    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<IProjectService, ProjectService>();

    builder.Services.AddScoped<ITaskRepository, TaskRepository>();
    builder.Services.AddScoped<ITaskService, TaskService>();

    builder.Services.AddScoped<IIssueRepository, IssueRepository>();
    builder.Services.AddScoped<IIssueService, IssueService>();


    // AutoMapper Configuration
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    // HATEOAS 
    builder.Services.AddHttpContextAccessor();

    // Add FluentValidation services
    builder.Services
        .AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();
    builder.Services.AddScoped<IValidator<AppUserUpdateDto>, AppUserUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<EntityPrototypeUpdateDto>, EntityPrototypeUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<EntityPrototypeCreateDto>, EntityPrototypeCreateDtoValidator>();
    builder.Services.AddScoped<IValidator<WorkspaceUpdateDto>, WorkspaceUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<WorkspaceCreateDto>, WorkspaceCreateDtoValidator>();
    builder.Services.AddScoped<IValidator<ProjectUpdateDto>, ProjectUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<ProjectCreateDto>, ProjectCreateDtoValidator>();
    builder.Services.AddScoped<IValidator<TaskUpdateDto>, TaskUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<TaskCreateDto>, TaskCreateDtoValidator>();
    builder.Services.AddScoped<IValidator<IssueUpdateDto>, IssueUpdateDtoValidator>();
    builder.Services.AddScoped<IValidator<IssueCreateDto>, IssueCreateDtoValidator>();

    #endregion

    var app = builder.Build();

    #region Build services

    app.UseCors("AllowSpecificOrigins"); // Use the named policy
    //app.UseCors("AllowAllOrigins"); // Use the named policy

    app.UseStatusCodePages();

    if (app.Environment.IsDevelopment())
    {
        // Apply migrations and seed data
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            app.UseSwagger();
            app.UseSwaggerUI();

            // Ensures that the database for the context exists. If it does not exist, it creates the database and all 
            // its schema (tables, indexes, etc.). Unlike Migrate(), this method does not use migrations and is mainly 
            // suited for simpler setups or scenarios where migrations are not required. It’s typically used in development 
            // or testing environments but is not ideal for production if migrations are part of the workflow.
            dbContext.Database.EnsureCreated();
            //dbContext.Database.EnsureDeleted();

            DbInitializer.Seed(scope.ServiceProvider);

            // Checks if there are any pending migrations that have not yet been applied to the database.
            // If there are pending migrations, it applies them by calling the Migrate() method. This ensures
            // that the database schema is up to date with the current model as defined in the application.
            try
            {
                dbContext.Database.Migrate();
            }
            catch { }
        }
    }

    //app.Urls.Add("http://192.168.56.1:3101"); // Windows IP (ipconfig) http://192.168.56.1:3101/swagger/index.html (Development Environment)
    //app.Urls.Add("http://localhost:3101"); // 127.0.0.1 

    // Configure the HTTP request pipeline.
    // Command for Windows using PowerShell
    // $env:ASPNETCORE_ENVIRONMENT="Development"
    // dotnet BookInformationService.dll
    // Command for Ubuntu using Terminal
    // export ASPNETCORE_ENVIRONMENT=Development
    // dotnet BookInformationService.dll

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(); //http://localhost:9001/swagger/index.html
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<LoggingMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseExceptionHandler();

    app.MapIdentityApi<AppUser>();
        //.RequireCors("AllowSpecificOrigins");
    app.MapControllers();

    app.Run();

    #endregion

    Log.Information("TaskManagement.Template Program is stopped.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "TaskManagement.Template Program failed to run correctly.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }