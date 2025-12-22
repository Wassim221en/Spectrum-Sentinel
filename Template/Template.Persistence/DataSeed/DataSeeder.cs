using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Template.Domain.Entities.Security;
using Template.Domain.Enums;
using Template.Domain.Primitives.Entity.Identity;
using Template.Persistence.DbContext;

namespace Template.Persistence.DataSeed;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Employees
        await SeedEmployeesAsync(userManager, roleManager);
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        // Check if roles already exist
        if (await roleManager.Roles.AnyAsync())
        {
            Console.WriteLine("Roles already seeded.");
            return;
        }

        var roles = new List<(string Name, RoleStatus Status, List<string> Permissions)>
        {
            ("Admin", RoleStatus.Active, new List<string>
            {
                "Employees.View",
                "Employees.Create",
                "Employees.Update",
                "Employees.Delete",
                "Roles.View",
                "Roles.Create",
                "Roles.Update",
                "Roles.Delete"
            }),
            ("Manager", RoleStatus.Active, new List<string>
            {
                "Employees.View",
                "Employees.Create",
                "Employees.Update",
                "Roles.View"
            }),
            ("Employee", RoleStatus.Active, new List<string>
            {
                "Employees.View"
            }),
            ("Viewer", RoleStatus.Active, new List<string>
            {
                "Employees.View",
                "Roles.View"
            })
        };

        foreach (var (name, status, permissions) in roles)
        {
            var role = new Role(name, status);
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                // Add permissions as claims
                foreach (var permission in permissions)
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
                Console.WriteLine($"Role '{name}' created successfully with {permissions.Count} permissions.");
            }
            else
            {
                Console.WriteLine($"Failed to create role '{name}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private static async Task SeedEmployeesAsync(UserManager<Employee> userManager, RoleManager<Role> roleManager)
    {
        // Check if employees already exist
        if (await userManager.Users.AnyAsync())
        {
            Console.WriteLine("Employees already seeded.");
            return;
        }

        // Get roles
        var adminRole = await roleManager.FindByNameAsync("Admin");
        var managerRole = await roleManager.FindByNameAsync("Manager");
        var employeeRole = await roleManager.FindByNameAsync("Employee");
        var viewerRole = await roleManager.FindByNameAsync("Viewer");

        var employees = new List<(string FirstName, string LastName, string UserName, string Email, string PhoneNumber, string Password, EmployeeStatus Status, Role? Role)>
        {
            ("Admin", "User", "admin", "admin@example.com", "+1234567890", "Admin@123", EmployeeStatus.Active, adminRole),
            ("John", "Manager", "john.manager", "john.manager@example.com", "+1234567891", "Manager@123", EmployeeStatus.Active, managerRole),
            ("Jane", "Smith", "jane.smith", "jane.smith@example.com", "+1234567892", "Employee@123", EmployeeStatus.Active, employeeRole),
            ("Bob", "Johnson", "bob.johnson", "bob.johnson@example.com", "+1234567893", "Employee@123", EmployeeStatus.Active, employeeRole),
            ("Alice", "Williams", "alice.williams", "alice.williams@example.com", "+1234567894", "Viewer@123", EmployeeStatus.Active, viewerRole),
            ("Charlie", "Brown", "charlie.brown", "charlie.brown@example.com", "+1234567895", "Employee@123", EmployeeStatus.Inactive, employeeRole),
            ("David", "Davis", "david.davis", "david.davis@example.com", "+1234567896", "Manager@123", EmployeeStatus.Active, managerRole),
            ("Emma", "Wilson", "emma.wilson", "emma.wilson@example.com", "+1234567897", "Employee@123", EmployeeStatus.Active, employeeRole),
            ("Frank", "Moore", "frank.moore", "frank.moore@example.com", "+1234567898", "Viewer@123", EmployeeStatus.Active, viewerRole),
            ("Grace", "Taylor", "grace.taylor", "grace.taylor@example.com", "+1234567899", "Employee@123", EmployeeStatus.Active, employeeRole)
        };

        foreach (var (firstName, lastName, userName, email, phoneNumber, password, status, role) in employees)
        {
            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Status = status,
                DateCreated = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(employee, password);

            if (result.Succeeded)
            {
                if (role != null)
                {
                    await userManager.AddToRoleAsync(employee, role.Name!);
                    Console.WriteLine($"Employee '{userName}' created successfully with role '{role.Name}'.");
                }
                else
                {
                    Console.WriteLine($"Employee '{userName}' created successfully without role.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to create employee '{userName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}

