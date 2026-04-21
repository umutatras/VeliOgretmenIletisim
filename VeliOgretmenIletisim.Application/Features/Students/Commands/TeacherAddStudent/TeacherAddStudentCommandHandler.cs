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
        var currentUserId = _currentUserService.UserId;
        
        var teacher = await _uow.GetRepository<Teacher>()
            .GetAll()
            .Include(t => t.AppUser)
            .FirstOrDefaultAsync(t => t.AppUserId == currentUserId, cancellationToken);

        if (teacher == null)
            return Result<Guid>.Failure("Teacher profile not found for current user.");

        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.Id == request.ParentId, cancellationToken);

        if (parent == null)
            return Result<Guid>.Failure("Parent not found.");

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StudentNumber = request.StudentNumber,
            ParentId = request.ParentId,
            TeacherId = teacher.Id
        };

        await _uow.GetRepository<Student>().AddAsync(student);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        if (parent.AppUser != null)
        {
            await _notificationService.SendToUserAsync(parent.AppUserId, 
                $"{student.FirstName} {student.LastName} has been linked to your account by teacher {teacher.AppUser?.FirstName} {teacher.AppUser?.LastName}.", "StudentAdded");
        }

        return Result<Guid>.Success(student.Id, "Student registered successfully by teacher.");
    }
}
