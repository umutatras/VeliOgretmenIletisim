using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                Role = UserRole.Admin 
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123*");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Departments
        if (!context.Departments.Any())
        {
            string[] deptNames = { "Matematik", "Fen Bilimleri", "Türkçe", "Sosyal Bilgiler", "İngilizce", "Görsel Sanatlar", "Müzik", "Beden Eğitimi", "Bilişim Teknolojileri", "Rehberlik" };
            foreach (var name in deptNames)
            {
                context.Departments.Add(new Department { Name = name });
            }
            await context.SaveChangesAsync();
        }

        var departments = await context.Departments.ToListAsync();

        // 4. Sample Teachers
        if (!context.Teachers.Any())
        {
            for (int i = 1; i <= 15; i++)
            {
                var email = $"teacher{i}@school.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = GetRandomName(),
                        LastName = GetRandomSurname(),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsApproved = true,
                        Role = UserRole.Teacher
                    };

                    var result = await userManager.CreateAsync(user, "Teacher123*");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Teacher");
                        var dept = departments[i % departments.Count];
                        var teacherProfile = new Teacher
                        {
                            AppUserId = user.Id,
                            Branch = dept.Name,
                            DepartmentId = dept.Id
                        };
                        context.Teachers.Add(teacherProfile);
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        // 5. Sample Parents
        if (!context.Parents.Any())
        {
            for (int i = 1; i <= 40; i++)
            {
                var email = $"parent{i}@example.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = GetRandomName(),
                        LastName = GetRandomSurname(),
                        EmailConfirmed = true,
                        IsActive = true,
                        IsApproved = true,
                        Role = UserRole.Parent,
                        PhoneNumber = "55500000" + i.ToString("D2")
                    };

                    var result = await userManager.CreateAsync(user, "Parent123*");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Parent");
                        var parentProfile = new Parent
                        {
                            AppUserId = user.Id,
                            Occupation = "Meslek " + i
                        };
                        context.Parents.Add(parentProfile);
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        // 6. Sample Students
        if (!context.Students.Any())
        {
            var teacherIds = await context.Teachers.Select(t => t.Id).ToListAsync();
            var parentIds = await context.Parents.Select(p => p.Id).ToListAsync();

            if (teacherIds.Any() && parentIds.Any())
            {
                Random rnd = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    var student = new Student
                    {
                        FirstName = GetRandomName(),
                        LastName = GetRandomSurname(),
                        StudentNumber = (1000 + i).ToString(),
                        ParentId = parentIds[rnd.Next(parentIds.Count)],
                        TeacherId = teacherIds[rnd.Next(teacherIds.Count)]
                    };
                    context.Students.Add(student);
                }
                await context.SaveChangesAsync();
            }
        }
    }

    private static string GetRandomName()
    {
        string[] names = { "Ali", "Ayşe", "Mehmet", "Fatma", "Can", "Zeynep", "Mustafa", "Elif", "Burak", "Selin", "Emre", "Derya", "Arda", "Melis", "Kerem", "Bahar", "Okan", "Gamze" };
        return names[new Random().Next(names.Length)];
    }

    private static string GetRandomSurname()
    {
        string[] surnames = { "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Öztürk", "Aydın", "Özkan", "Aslan", "Kılıç", "Arslan", "Bulut", "Güneş", "Yavuz", "Koç" };
        return surnames[new Random().Next(surnames.Length)];
    }
}
