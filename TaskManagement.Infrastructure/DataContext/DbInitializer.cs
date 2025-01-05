using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities.Auth;

namespace TaskManagement.Infrastructure.DataContext
{
    public static class DbInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            try
            {
                if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

                // Resolve the required services
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

                const string defaultUserName = "superadmin";
                const string defaultEmail = "superadmin@example.com";
                const string defaultPassword = "Admin!123";
                const string defaultRole = "Administrator";

                // Ensure the role exists
                if (!await roleManager.RoleExistsAsync(defaultRole))
                {
                    var role = new AppRole { Name = defaultRole };
                    var roleResult = await roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{defaultRole}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }

                // Check if the default user exists
                var existingUser = await userManager.FindByNameAsync(defaultUserName);
                if (existingUser == null)
                {
                    var defaultUser = new AppUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = defaultUserName,
                        Email = defaultEmail,
                        EmailConfirmed = true // Consider setting this based on your requirements
                    };

                    // Create the default user
                    var userResult = await userManager.CreateAsync(defaultUser, defaultPassword);
                    if (userResult.Succeeded)
                    {
                        // Assign the default user to the Administrator role
                        var roleAssignResult = await userManager.AddToRoleAsync(defaultUser, defaultRole);
                        if (!roleAssignResult.Succeeded)
                        {
                            throw new Exception($"Failed to assign role '{defaultRole}' to the default user: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to create the default user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                Console.WriteLine($"An error occurred during seeding: {ex.Message}");
                throw;
            }
        }
    }
}
