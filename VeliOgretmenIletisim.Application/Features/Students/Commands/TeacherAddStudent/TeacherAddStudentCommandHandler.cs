using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.TeacherAddStudent;

public class TeacherAddStudentCommandHandler : IRequestHandler<TeacherAddStudentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public TeacherAddStudentCommandHandler(IUnitOfWork uow, INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(TeacherAddStudentCommand request, CancellationToken cancellationToken)
    {
        var finalTeacherIds = new List<Guid>();

        if (request.TeacherIds != null && request.TeacherIds.Any())
        {
            finalTeacherIds.AddRange(request.TeacherIds);
        }
        else if (_currentUserService.Role != "Admin")
        {
            var currentUserId = _currentUserService.UserId;
            var teacher = await _uow.GetRepository<Teacher>()
                .GetAll()
                .FirstOrDefaultAsync(t => t.AppUserId == currentUserId, cancellationToken);

            if (teacher == null)
                return Result<Guid>.Failure("Giriş yapan kullanıcının öğretmen profili bulunamadı.");

            finalTeacherIds.Add(teacher.Id);
        }

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

        // Add Teacher assignments
        foreach (var tId in finalTeacherIds.Distinct())
        {
            student.StudentTeachers.Add(new StudentTeacher
            {
                TeacherId = tId,
                IsPrimary = tId == finalTeacherIds.First() // İlk seçilen ana öğretmen olsun
            });
        }

        await _uow.GetRepository<Student>().AddAsync(student);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        if (parent.AppUser != null && finalTeacherIds.Any())
        {
            var firstTeacherId = finalTeacherIds.First();
            var teacher = await _uow.GetRepository<Teacher>().GetAll()
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(t => t.Id == firstTeacherId, cancellationToken);

            var message = $"{student.FirstName} {student.LastName} isimli öğrenci sisteme kaydedildi.";
            if (teacher != null)
                message += $" (Danışman: {teacher.AppUser.FirstName} {teacher.AppUser.LastName})";

            await _notificationService.SendToUserAsync(parent.AppUserId, message, "StudentAdded");
        }

        return Result<Guid>.Success(student.Id, "Öğrenci başarıyla kaydedildi.");
    }
}
