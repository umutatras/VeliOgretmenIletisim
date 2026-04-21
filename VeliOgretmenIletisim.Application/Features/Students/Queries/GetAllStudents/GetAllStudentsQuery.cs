using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Queries.GetAllStudents;

public record GetAllStudentsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<StudentDto>>>;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
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
            .ThenInclude(p => p.AppUser);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                StudentNumber = s.StudentNumber,
                ParentName = $"{s.Parent.AppUser.FirstName} {s.Parent.AppUser.LastName}"
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<StudentDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<StudentDto>>.Success(pagedResult);
    }
}
