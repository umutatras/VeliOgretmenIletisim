using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Features.Dashboard.Queries.GetStats;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        return HandleResult(await Mediator.Send(new GetDashboardStatsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("check-fatma")]
    public async Task<IActionResult> CheckFatma()
    {
        var context = HttpContext.RequestServices.GetRequiredService<VeliOgretmenIletisim.Infrastructure.Persistence.Context.ApplicationDbContext>();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "fatma@gmail.com");
        if (user == null) return NotFound("Fatma user not found");

        var parent = await context.Parents.FirstOrDefaultAsync(p => p.AppUserId == user.Id);
        if (parent == null) return NotFound("Fatma parent profile not found");

        var students = await context.Students.Where(s => s.ParentId == parent.Id).Include(s => s.StudentTeachers).ToListAsync();

        var teacherIds = students.SelectMany(s => s.StudentTeachers).Select(st => st.TeacherId).Distinct().ToList();

        var availabilities = await context.Availabilities.Where(a => teacherIds.Contains(a.TeacherId) && a.StartTime > DateTime.Now).ToListAsync();

        return Ok(new
        {
            UserId = user.Id,
            ParentId = parent.Id,
            StudentCount = students.Count,
            ConnectedTeacherCount = teacherIds.Count,
            FutureAvailabilityCount = availabilities.Count,
            Students = students.Select(s => new { s.FirstName, s.LastName, Teachers = s.StudentTeachers.Count })
        });
    }

    [AllowAnonymous]
    [HttpGet("check-teacher")]
    public async Task<IActionResult> CheckTeacher(string email)
    {
        var context = HttpContext.RequestServices.GetRequiredService<VeliOgretmenIletisim.Infrastructure.Persistence.Context.ApplicationDbContext>();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return NotFound("User not found");

        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == user.Id);
        if (teacher == null) return NotFound("Teacher profile not found");

        var appointments = await context.Appointments.Where(a => a.Availability.TeacherId == teacher.Id).ToListAsync();

        return Ok(new
        {
            UserId = user.Id,
            TeacherId = teacher.Id,
            AppointmentCount = appointments.Count,
            Role = user.Role.ToString()
        });
    }

    [AllowAnonymous]
    [HttpGet("fix-db")]
    public async Task<IActionResult> FixDb()
    {
        var context = HttpContext.RequestServices.GetRequiredService<VeliOgretmenIletisim.Infrastructure.Persistence.Context.ApplicationDbContext>();
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Appointments') AND name = 'TeacherId')
                BEGIN
                    ALTER TABLE Appointments ADD TeacherId UNIQUEIDENTIFIER NULL;
                END
            ");

            // Also fill existing ones if possible (best effort)
            await context.Database.ExecuteSqlRawAsync(@"
                UPDATE a SET a.TeacherId = av.TeacherId 
                FROM Appointments a
                JOIN Availabilities av ON a.AvailabilityId = av.Id
                WHERE a.TeacherId IS NULL
            ");

            return Ok("Database schema checked and updated.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
