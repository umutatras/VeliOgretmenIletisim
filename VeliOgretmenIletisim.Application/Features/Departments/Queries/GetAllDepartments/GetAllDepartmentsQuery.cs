using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Departments.Queries.GetAllDepartments;

public class GetAllDepartmentsQuery : IRequest<Result<List<DepartmentDto>>> { }

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<List<DepartmentDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllDepartmentsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _uow.GetRepository<Department>()
            .GetAll()
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<List<DepartmentDto>>.Success(departments);
    }
}
