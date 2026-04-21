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

        if (request.TeacherId.HasValue)
        {
            var teacher = await _uow.GetRepository<Teacher>().GetByIdAsync(request.TeacherId.Value);
            if (teacher == null) return Result<Guid>.Failure("Öğretmen bulunamadı.");
        }

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StudentNumber = request.StudentNumber,
            ParentId = request.ParentId,
            TeacherId = request.TeacherId
        };

        await _uow.GetRepository<Student>().AddAsync(student);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        await _notificationService.SendToUserAsync(parent.AppUserId, 
            $"{student.FirstName} {student.LastName} has been linked to your account.", "StudentAdded");

        return Result<Guid>.Success(student.Id, "Student registered and linked successfully.");
    }
}
