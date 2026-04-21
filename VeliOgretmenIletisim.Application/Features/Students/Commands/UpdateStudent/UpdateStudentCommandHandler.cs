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
        var student = await _uow.GetRepository<Student>()
            .Where(s => s.Id == request.Id)
            .Include(s => s.StudentTeachers)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;

        if (role == "Teacher")
        {
            var teacher = await _uow.GetRepository<Teacher>()
                .Where(t => t.AppUserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher == null || !student.StudentTeachers.Any(st => st.TeacherId == teacher.Id))
                return Result.Failure("Bu öğrenciyi güncelleme yetkiniz bulunmamaktadır.", 403);
        }
        else if (role == "Parent")
        {
            var parent = await _uow.GetRepository<Parent>()
                .Where(p => p.AppUserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (parent == null || student.ParentId != parent.Id)
                return Result.Failure("Bu öğrenciyi güncelleme yetkiniz bulunmamaktadır.", 403);
        }

        if (student.StudentNumber != request.StudentNumber)
        {
            var numberExists = await _uow.GetRepository<Student>()
                .GetAll()
                .AnyAsync(s => s.StudentNumber == request.StudentNumber, cancellationToken);

            if (numberExists)
                return Result.Failure($"'{request.StudentNumber}' numaralı öğrenci zaten kayıtlı.");
        }

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.StudentNumber = request.StudentNumber;
        student.PhoneNumber = request.PhoneNumber;

        if (role == "Admin")
        {
            student.ParentId = request.ParentId;
        }

        // Sadece Admin öğretmen atamasını değiştirebilir veya ekleyebilir
        if (role == "Admin" && request.TeacherIds != null)
        {
            // 1. ADIM: Mevcut tüm bağlantıları veritabanından açıkça bul ve SİL
            var existingLinks = await _uow.GetRepository<StudentTeacher>()
                .Where(st => st.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            foreach (var link in existingLinks)
            {
                _uow.GetRepository<StudentTeacher>().Delete(link);
            }

            // Önce silme işlemini kaydet (Bu, takip çakışmalarını önler)
            await _uow.SaveChangesAsync(cancellationToken);

            // Yerel koleksiyonu boşalt
            student.StudentTeachers.Clear();

            // 2. ADIM: Yeni listeyi Distinct olarak ekle
            var newIds = request.TeacherIds.Distinct().ToList();
            int count = 0;
            foreach (var tId in newIds)
            {
                var newLink = new StudentTeacher
                {
                    StudentId = student.Id,
                    TeacherId = tId,
                    IsPrimary = (count == 0)
                };
                await _uow.GetRepository<StudentTeacher>().AddAsync(newLink);
                count++;
            }
        }

        // Öğrenci temel bilgilerini güncelle ve son durumu kaydet
        _uow.GetRepository<Student>().Update(student);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Öğrenci bilgileri başarıyla güncellendi.");
    }
}
