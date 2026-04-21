using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public UpdateStudentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _uow.GetRepository<Student>().GetByIdAsync(request.Id);

        if (student == null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;

        if (role == "Teacher")
        {
            var teacher = await _uow.GetRepository<Teacher>()
                .Where(t => t.AppUserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher == null || student.TeacherId != teacher.Id)
                return Result.Failure("Bu öğrenciyi güncelleme yetkiniz bulunmamaktadır.", 403);
            
            // Öğretmen kendi ID'sini değiştiremez, admin yapabilir
            // Ama öğretmen kendi eklediği öğrencinin bilgilerini güncelleyebilir.
        }

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.StudentNumber = request.StudentNumber;
        student.ParentId = request.ParentId;

        // Sadece Admin öğretmen atamasını değiştirebilir
        if (role == "Admin")
        {
            student.TeacherId = request.TeacherId;
        }

        _uow.GetRepository<Student>().Update(student);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Öğrenci bilgileri başarıyla güncellendi.");
    }
}
