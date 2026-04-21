using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.AdminAddStudent;

public class AdminAddStudentCommandHandler : IRequestHandler<AdminAddStudentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;

    public AdminAddStudentCommandHandler(IUnitOfWork uow, INotificationService notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(AdminAddStudentCommand request, CancellationToken cancellationToken)
    {
        var existingStudent = await _uow.GetRepository<Student>()
            .GetAll()
            .FirstOrDefaultAsync(s => s.StudentNumber == request.StudentNumber, cancellationToken);

        if (existingStudent != null)
            return Result<Guid>.Failure($"'{request.StudentNumber}' numaralı öğrenci zaten kayıtlı.");

        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.Id == request.ParentId, cancellationToken);

        if (parent == null)
            return Result<Guid>.Failure("Veli bulunamadı.");

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StudentNumber = request.StudentNumber,
            PhoneNumber = request.PhoneNumber,
            ParentId = request.ParentId
        };

        if (request.TeacherIds != null && request.TeacherIds.Any())
        {
            foreach (var tId in request.TeacherIds.Distinct())
            {
                student.StudentTeachers.Add(new StudentTeacher 
                { 
                    TeacherId = tId,
                    IsPrimary = tId == request.TeacherIds.First() 
                });
            }
        }

        await _uow.GetRepository<Student>().AddAsync(student);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        await _notificationService.SendToUserAsync(parent.AppUserId, 
            $"{student.FirstName} {student.LastName} isimli öğrenci hesabınızla ilişkilendirildi.", "StudentAdded");

        return Result<Guid>.Success(student.Id, "Student registered and linked successfully.");
    }
}
