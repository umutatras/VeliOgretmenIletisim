using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;

    public CreateDepartmentCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Name = request.Name
        };

        await _uow.GetRepository<Department>().AddAsync(department);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(department.Id, "Department created successfully.");
    }
}
