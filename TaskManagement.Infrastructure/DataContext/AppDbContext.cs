using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.DTOs.Issue;
using TaskManagement.Domain.Entities.Base;

/* Ensure Migration and Database Initialization:

Open Package Manager Console:
Navigate to Tools > NuGet Package Manager > Package Manager Console in Visual Studio.

Need to be installed the packages:
    TaskManagement.Infrastructure:
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10">
		  <PrivateAssets>all</PrivateAssets>
		  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
		<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.10">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
        <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.10" />
    TaskManagement.Presenter:
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10">
		  <PrivateAssets>all</PrivateAssets>
		  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
Program.cs:
    TaskManagement.Presenter:
        // EF
        builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(defaultConnectionString));
Troubleshooting:
    Check for Pending Migrations
        dotnet ef migrations list --project TaskManagement.Infrastructure --startup-project TaskManagement.Presenter
    Enable Detailed Logging:
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging() // Optional: for detailed logs
                .LogTo(Console.WriteLine, LogLevel.Information)); 


Execute the following commands:
    [For same project holds program.cs & appsettings.json]
    Add-Migration InitialCreate
    Update-Database

    [If not working the above commands, then use below commands]
    dotnet ef migrations add InitialCreate --project TaskManagement.Infrastructure --startup-project TaskManagement.Infrastructure
    Update-Database

    [For different projects] (Execute from Solution folder, not from Project folder)
    dotnet ef migrations add InitialCreate --project TaskManagement.Infrastructure --startup-project TaskManagement.Presenter
    dotnet ef database update --project TaskManagement.Infrastructure --startup-project TaskManagement.Presenter

*/

namespace TaskManagement.Infrastructure.DataContext
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public DbSet<EntityPrototype> EntityPrototype { get; set; }
        public DbSet<Workspace> Workspace { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Domain.Entities.Task> Task { get; set; }
        public DbSet<Issue> Issue { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region NotMapped Attributes, such as ULid, Value Object

            #region AppUser

            #region ID Conversion - Guid

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Id)
                .HasConversion(
                    ulid => ulid.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<AppUser>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        ulid => ulid.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region ID Custom Data Type (Value Object) Conversion - Email, Phone etc.

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Email)
            //    .HasConversion(
            //        email => (string)(email ?? string.Empty),       // Converts Email to string for the database
            //        value => (Email)value);                         // Converts string back to Email when reading from the database

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Phone)
            //    .HasConversion(
            //        phone => (string)(phone ?? string.Empty),       // Converts Email to string for the database
            //        value => (Phone)value);                         // Converts string back to Email when reading from the database

            #endregion

            #endregion

            #region Auth

            modelBuilder.Entity<IdentityRoleClaim<Guid>>()
                .Property(u => u.RoleId)
                .HasConversion(
                    ulid => ulid.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            #endregion

            #region EntityPrototype

            #region ID Conversion - Guid

            modelBuilder.Entity<EntityPrototype>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<EntityPrototype>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        id => id.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region Unique property index

            modelBuilder.Entity<EntityPrototype>()
            .HasIndex(e => e.Name)
            .IsUnique();

            #endregion

            #endregion

            #region Workspace

            #region ID Conversion - Guid

            modelBuilder.Entity<Workspace>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<Workspace>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        id => id.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region Unique property index

            modelBuilder.Entity<Workspace>()
            .HasIndex(e => e.Name)
            .IsUnique();

            #endregion

            #endregion

            #region Project

            #region ID Conversion - Guid

            modelBuilder.Entity<Project>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<Project>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        id => id.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region Unique property index

            modelBuilder.Entity<Project>()
            .HasIndex(e => e.Name)
            .IsUnique();

            #endregion

            #endregion

            #region Task

            #region ID Conversion - Guid

            modelBuilder.Entity<Domain.Entities.Task>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<Domain.Entities.Task>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        id => id.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region Unique property index

            modelBuilder.Entity<Domain.Entities.Task>()
            .HasIndex(e => e.Name)
            .IsUnique();

            #endregion

            #endregion

            #region Issue

            #region ID Conversion - Guid

            modelBuilder.Entity<Issue>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),        // Converts Guid to string for the database
                    value => Guid.Parse(value));    // Converts string back to Guid when reading from the database

            modelBuilder.Entity<Issue>()
                    .Property(u => u.CreatedBy)
                    .HasConversion(
                        id => id.ToString(),
                        value => Guid.Parse(value));

            #endregion

            #region Unique property index

            modelBuilder.Entity<Issue>()
            .HasIndex(e => e.Name)
            .IsUnique();

            #endregion

            #endregion

            #endregion

            #region Ignore DTO & Value Objects models to create the table in database

            #region AppUser

            modelBuilder.Ignore<AppUserReadDto>();
            modelBuilder.Ignore<AppUserCreateDto>();
            modelBuilder.Ignore<AppUserUpdateDto>();

            #endregion

            #region EntityPrototype

            modelBuilder.Ignore<EntityPrototypeReadDto>();
            modelBuilder.Ignore<EntityPrototypeCreateDto>();
            modelBuilder.Ignore<EntityPrototypeUpdateDto>();

            #endregion

            #region Workspace

            modelBuilder.Ignore<WorkspaceReadDto>();
            modelBuilder.Ignore<WorkspaceCreateDto>();
            modelBuilder.Ignore<WorkspaceUpdateDto>();

            #endregion

            #region Project

            modelBuilder.Ignore<ProjectReadDto>();
            modelBuilder.Ignore<ProjectCreateDto>();
            modelBuilder.Ignore<ProjectUpdateDto>();

            #endregion

            #region Task

            modelBuilder.Ignore<TaskReadDto>();
            modelBuilder.Ignore<TaskCreateDto>();
            modelBuilder.Ignore<TaskUpdateDto>();

            #endregion

            #region Issue

            modelBuilder.Ignore<IssueReadDto>();
            modelBuilder.Ignore<IssueCreateDto>();
            modelBuilder.Ignore<IssueUpdateDto>();

            #endregion

            #endregion

            #region Configure entities without a key

            //modelBuilder.Entity<Address>()
            //    .HasNoKey() // Specifies that the Address entity does not have a key
            //    .ToTable("Address"); // Maps the Address entity to the "Address" table

            #endregion
        }

        public Expression<Func<T, object>> GetSortExpression<T>(string sortColumn)
        {
            var param = Expression.Parameter(typeof(T), "entity");

            // Find the property by name on the specified type T
            var property = Expression.PropertyOrField(param, sortColumn);
            var convertedProperty = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(convertedProperty, param);
        }
    }
}

