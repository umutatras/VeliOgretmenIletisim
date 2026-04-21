using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Commands.AssignDepartment;

public class AssignDepartmentCommandHandler : IRequestHandler<AssignDepartmentCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public AssignDepartmentCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(AssignDepartmentCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _uow.GetRepository<Teacher>().GetByIdAsync(request.TeacherId);
        var department = await _uow.GetRepository<Department>().GetByIdAsync(request.DepartmentId);

        if (teacher == null || department == null)
            return Result.Failure("Teacher or Department not found.");

        teacher.DepartmentId = request.DepartmentId;

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success($"Teacher assigned to {department.Name} department.");
    }
}
