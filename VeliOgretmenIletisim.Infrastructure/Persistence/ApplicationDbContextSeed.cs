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
                        ParentId = parentIds[rnd.Next(parentIds.Count)]
                    };
                    
                    var randomTeacherId = teacherIds[rnd.Next(teacherIds.Count)];
                    student.StudentTeachers.Add(new StudentTeacher { TeacherId = randomTeacherId, IsPrimary = true });
                    
                    // Rastgele ikinci bir öğretmen daha ekleyelim her ikinci öğrenciye
                    if (i % 2 == 0)
                    {
                        var secondTeacherId = teacherIds[rnd.Next(teacherIds.Count)];
                        if (secondTeacherId != randomTeacherId)
                        {
                            student.StudentTeachers.Add(new StudentTeacher { TeacherId = secondTeacherId, IsPrimary = false });
                        }
                    }

                    context.Students.Add(student);
                }
                await context.SaveChangesAsync();
            }
        }

        // 7. Special Test Scenario: Fatma Kılınç
        var fatmaEmail = "fatma@gmail.com";
        var fatmaUser = await userManager.FindByEmailAsync(fatmaEmail);
        
        if (fatmaUser == null)
        {
            fatmaUser = new AppUser
            {
                UserName = fatmaEmail,
                Email = fatmaEmail,
                FirstName = "Fatma",
                LastName = "Kılınç",
                EmailConfirmed = true,
                IsActive = true,
                IsApproved = true,
                Role = UserRole.Parent,
                PhoneNumber = "0532 999 88 77"
            };

            await userManager.CreateAsync(fatmaUser, "Fatma123*");
            await userManager.AddToRoleAsync(fatmaUser, "Parent");
        }

        var fatmaProfile = await context.Parents.FirstOrDefaultAsync(p => p.AppUserId == fatmaUser.Id);
        if (fatmaProfile == null)
        {
            fatmaProfile = new Parent { AppUserId = fatmaUser.Id, Occupation = "Mühendis" };
            context.Parents.Add(fatmaProfile);
            await context.SaveChangesAsync();
        }

        // Add Students for Fatma if she has none
        var teacherList = await context.Teachers.Take(5).ToListAsync();
        if (!context.Students.Any(s => s.ParentId == fatmaProfile.Id))
        {
            if (teacherList.Count >= 3)
            {
                var student1 = new Student { FirstName = "Ahmet", LastName = "Kılınç", StudentNumber = "777", ParentId = fatmaProfile.Id, PhoneNumber = "0533 111 22 33" };
                student1.StudentTeachers.Add(new StudentTeacher { TeacherId = teacherList[0].Id, IsPrimary = true });
                student1.StudentTeachers.Add(new StudentTeacher { TeacherId = teacherList[1].Id, IsPrimary = false });

                var student2 = new Student { FirstName = "Zeynep", LastName = "Kılınç", StudentNumber = "888", ParentId = fatmaProfile.Id, PhoneNumber = "0533 444 55 66" };
                student2.StudentTeachers.Add(new StudentTeacher { TeacherId = teacherList[2].Id, IsPrimary = true });

                context.Students.AddRange(student1, student2);
                await context.SaveChangesAsync();
            }
            else if (teacherList.Count > 0)
            {
                var student1 = new Student { FirstName = "Ahmet", LastName = "Kılınç", StudentNumber = "777", ParentId = fatmaProfile.Id, PhoneNumber = "0533 111 22 33" };
                student1.StudentTeachers.Add(new StudentTeacher { TeacherId = teacherList[0].Id, IsPrimary = true });

                context.Students.Add(student1);
                await context.SaveChangesAsync();
            }
        }

        // Create Müzikli Randevular (Availabilities) for these teachers ALWAYS if they don't have future ones
        if (teacherList.Any())
        {
            foreach (var teacher in teacherList)
            {
                if (!context.Availabilities.Any(a => a.TeacherId == teacher.Id && a.StartTime > DateTime.Now))
                {
                    context.Availabilities.Add(new Availability
                    {
                        TeacherId = teacher.Id,
                        StartTime = DateTime.Now.AddDays(1).Date.AddHours(9), // Yarın 09:00
                        EndTime = DateTime.Now.AddDays(1).Date.AddHours(10),
                        MaxCapacity = 5,
                        IsGroup = true
                    });
                    
                    context.Availabilities.Add(new Availability
                    {
                        TeacherId = teacher.Id,
                        StartTime = DateTime.Now.AddDays(2).Date.AddHours(13), // 2 gün sonra 13:00
                        EndTime = DateTime.Now.AddDays(2).Date.AddHours(14),
                        MaxCapacity = 1,
                        IsGroup = false
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // 8. Special Test Scenario: Ahmet Teacher (Ahmet Hoca)
        var ahmetTeacherEmail = "ahmet@school.com";
        var ahmetTeacherUser = await userManager.FindByEmailAsync(ahmetTeacherEmail);
        if (ahmetTeacherUser == null)
        {
            ahmetTeacherUser = new AppUser
            {
                UserName = ahmetTeacherEmail,
                Email = ahmetTeacherEmail,
                FirstName = "Ahmet",
                LastName = "Hoca",
                EmailConfirmed = true,
                IsActive = true,
                IsApproved = true,
                Role = UserRole.Teacher
            };
            await userManager.CreateAsync(ahmetTeacherUser, "Teacher123*");
            await userManager.AddToRoleAsync(ahmetTeacherUser, "Teacher");
            
            var dept = await context.Departments.FirstOrDefaultAsync();
            var teacherProfile = new Teacher
            {
                AppUserId = ahmetTeacherUser.Id,
                Branch = "Matematik",
                DepartmentId = dept?.Id ?? Guid.Empty
            };
            context.Teachers.Add(teacherProfile);
            await context.SaveChangesAsync();

            // Link this specific teacher to Fatma's student Ahmet
            var fatmaStudent = await context.Students.FirstOrDefaultAsync(s => s.FirstName == "Ahmet" && s.LastName == "Kılınç");
            if (fatmaStudent != null)
            {
                if (!context.StudentTeachers.Any(st => st.StudentId == fatmaStudent.Id && st.TeacherId == teacherProfile.Id))
                {
                    context.StudentTeachers.Add(new StudentTeacher { StudentId = fatmaStudent.Id, TeacherId = teacherProfile.Id, IsPrimary = true });
                    await context.SaveChangesAsync();
                }
            }
            // Ensure Ahmet Hoca has more availabilities for testing
            if (context.Availabilities.Count(a => a.TeacherId == teacherProfile.Id && a.StartTime > DateTime.Now) < 5)
            {
                for (int h = 9; h <= 17; h++)
                {
                    if (h == 12) continue; // Lunch break
                    context.Availabilities.Add(new Availability
                    {
                        TeacherId = teacherProfile.Id,
                        StartTime = DateTime.Now.AddDays(2).Date.AddHours(h),
                        EndTime = DateTime.Now.AddDays(2).Date.AddHours(h).AddMinutes(40),
                        MaxCapacity = 1,
                        IsGroup = false
                    });
                }
                await context.SaveChangesAsync();
            }

            // Add a test appointment for Ahmet Hoca to see
            var ahmetAvailability = await context.Availabilities.FirstOrDefaultAsync(a => a.TeacherId == teacherProfile.Id && a.StartTime > DateTime.Now);
            if (ahmetAvailability != null && !context.Appointments.Any(ap => ap.ParentId == fatmaProfile.Id && ap.AvailabilityId == ahmetAvailability.Id))
            {
                context.Appointments.Add(new Appointment
                {
                    ParentId = fatmaProfile.Id,
                    TeacherId = teacherProfile.Id,
                    AvailabilityId = ahmetAvailability.Id,
                    StudentId = fatmaStudent?.Id,
                    Status = AppointmentStatus.Pending,
                    Note = "Öğrencinin genel durumu hakkında görüşmek istiyorum."
                });
                await context.SaveChangesAsync();
            }
        }
    }

    private static string GetRandomName()
    {
        string[] names = { "Ali", "Ayşe", "Mehmet", "Fatma", "Can", "Zeynep", "Mustafa", "Elif", "Burak", "Selin", "Emre", "Derya", "Arda", "Melis", "Kerem", "Bahar", "Okan", "Gamze" };
        return names[new Random().Next(names.Length)];
    }

    private static string GetRandomName2()
    {
        string[] names = { "Fatma", "Ayşe", "Emine", "Hatice", "Zeynep", "Meryem", "Elif", "Zehra" };
        return names[new Random().Next(names.Length)];
    }

    private static string GetRandomSurname()
    {
        string[] surnames = { "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Öztürk", "Aydın", "Özkan", "Aslan", "Kılıç", "Arslan", "Bulut", "Güneş", "Yavuz", "Koç" };
        return surnames[new Random().Next(surnames.Length)];
    }
}
