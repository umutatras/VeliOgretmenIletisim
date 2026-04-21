using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;
using VeliOgretmenIletisim.Infrastructure.Persistence.Context;

namespace VeliOgretmenIletisim.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // 1. Roles Seed
        string[] roles = { "Admin", "Teacher", "Parent" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // 2. Admin User Seed
        var adminEmail = "admin@school.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true,
                IsApproved = true,
                Role = UserRole.Admin // Keep enum for business logic if needed
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123*");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Departments & Sample Teacher Seed
        if (!context.Departments.Any())
        {
            var mathDept = new Department { Name = "Matematik" };
            var scienceDept = new Department { Name = "Fen Bilimleri" };
            context.Departments.AddRange(mathDept, scienceDept);
            await context.SaveChangesAsync();

            // Sample Teacher
            var teacherEmail = "teacher@school.com";
            if (await userManager.FindByEmailAsync(teacherEmail) == null)
            {
                var teacherUser = new AppUser
                {
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    FirstName = "Ahmet",
                    LastName = "Öğretmen",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsApproved = true,
                    Role = UserRole.Teacher
                };

                var result = await userManager.CreateAsync(teacherUser, "Teacher123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacherUser, "Teacher");
                    
                    var teacherProfile = new Teacher
                    {
                        AppUserId = teacherUser.Id,
                        Branch = "Matematik",
                        DepartmentId = mathDept.Id
                    };
                    context.Teachers.Add(teacherProfile);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
