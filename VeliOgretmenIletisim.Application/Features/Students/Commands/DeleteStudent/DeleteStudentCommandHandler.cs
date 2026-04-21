using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.DeleteStudent;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public DeleteStudentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _uow.GetRepository<Student>()
            .GetAll()
            .Include(s => s.StudentTeachers)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var userId = _currentUserService.UserId;
        // Yetki Kontrolü: Admin her şeyi silebilir, öğretmen sadece kendine bağlı öğrenciyi silebilir
        var isAdmin = _currentUserService.Role == "Admin";
        
        if (!isAdmin)
        {
            var teacher = await _uow.GetRepository<Teacher>()
                .Where(t => t.AppUserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher == null || !student.StudentTeachers.Any(st => st.TeacherId == teacher.Id))
                return Result.Failure("Bu öğrenciyi silme yetkiniz bulunmamaktadır.", 403);
        }

        // Öğrenciyi sil
        _uow.GetRepository<Student>().Delete(student);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Öğrenci başarıyla silindi.");
    }
}
