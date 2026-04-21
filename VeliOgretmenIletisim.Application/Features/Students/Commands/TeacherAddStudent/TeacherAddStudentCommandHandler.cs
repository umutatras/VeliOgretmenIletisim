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
        Guid? teacherId = null;

        if (request.TeacherId.HasValue && request.TeacherId != Guid.Empty)
        {
            teacherId = request.TeacherId.Value;
        }
        else if (_currentUserService.Role != "Admin")
        {
            var currentUserId = _currentUserService.UserId;
            var teacher = await _uow.GetRepository<Teacher>()
                .GetAll()
                .FirstOrDefaultAsync(t => t.AppUserId == currentUserId, cancellationToken);

            if (teacher == null)
                return Result<Guid>.Failure("Giriş yapan kullanıcının öğretmen profili bulunamadı.");
            
            teacherId = teacher.Id;
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
            ParentId = request.ParentId,
            TeacherId = teacherId
        };

        await _uow.GetRepository<Student>().AddAsync(student);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        if (parent.AppUser != null)
        {
            var teacher = await _uow.GetRepository<Teacher>().GetAll()
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);

            await _notificationService.SendToUserAsync(parent.AppUserId, 
                $"{student.FirstName} {student.LastName} isimli öğrenci {teacher?.AppUser?.FirstName} {teacher?.AppUser?.LastName} öğretmeni ile ilişkilendirildi.", "StudentAdded");
        }

        return Result<Guid>.Success(student.Id, "Öğrenci başarıyla kaydedildi.");
    }
}
