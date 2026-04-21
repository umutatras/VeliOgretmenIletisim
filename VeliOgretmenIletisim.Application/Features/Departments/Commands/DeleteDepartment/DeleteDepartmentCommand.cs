using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid Id) : IRequest<Result>;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteDepartmentCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _uow.GetRepository<Department>();
        var department = await repository.GetByIdAsync(request.Id);

        if (department == null)
            return Result.Failure("Department not found.");

        repository.Delete(department); // Will be handled as soft-delete by DbContext if implemented
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Department deleted successfully.");
    }
}
