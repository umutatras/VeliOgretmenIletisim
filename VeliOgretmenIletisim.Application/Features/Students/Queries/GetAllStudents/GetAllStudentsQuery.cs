using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Queries.GetAllStudents;

public record GetAllStudentsQuery(int PageNumber = 1, int PageSize = 50, string? SearchTerm = null) : IRequest<Result<PagedResult<StudentDto>>>;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public Guid ParentId { get; set; }
    public List<string> TeacherNames { get; set; } = new();
    public List<Guid> TeacherIds { get; set; } = new();
}

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, Result<PagedResult<StudentDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllStudentsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PagedResult<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.GetRepository<Student>()
            .GetAll()
            .AsNoTracking()
            .Include(s => s.Parent)
                .ThenInclude(p => p.AppUser)
            .Include(s => s.StudentTeachers)
                .ThenInclude(st => st.Teacher)
                    .ThenInclude(t => t.AppUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(s => 
                s.FirstName.ToLower().Contains(searchTerm) || 
                s.LastName.ToLower().Contains(searchTerm) || 
                s.StudentNumber.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                StudentNumber = s.StudentNumber,
                PhoneNumber = s.PhoneNumber,
                ParentName = s.Parent != null && s.Parent.AppUser != null 
                    ? s.Parent.AppUser.FirstName + " " + s.Parent.AppUser.LastName 
                    : "Bilinmiyor",
                ParentId = s.ParentId,
                TeacherNames = s.StudentTeachers
                    .Select(st => st.Teacher != null && st.Teacher.AppUser != null 
                        ? st.Teacher.AppUser.FirstName + " " + st.Teacher.AppUser.LastName 
                        : "Bilinmiyor")
                    .ToList(),
                TeacherIds = s.StudentTeachers.Select(st => st.TeacherId).ToList()
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<StudentDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<StudentDto>>.Success(pagedResult);
    }
}
