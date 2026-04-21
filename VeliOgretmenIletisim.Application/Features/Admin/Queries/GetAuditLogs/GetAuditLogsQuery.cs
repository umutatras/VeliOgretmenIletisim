using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Admin.Queries.GetAuditLogs;

public record GetAuditLogsQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<AuditLogDto>>>;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string RequestBody { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
    public double ExecutionTimeMs { get; set; }
    public int StatusCode { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CurlCommand { get; set; }
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAuditLogsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.GetRepository<AuditLog>()
            .GetAll()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Action = x.ActionName ?? string.Empty,
                HttpMethod = x.HttpMethod ?? string.Empty,
                RequestPath = x.RequestPath ?? string.Empty,
                IpAddress = x.ClientIp ?? string.Empty,
                RequestBody = x.RequestBody ?? string.Empty,
                ResponseBody = x.ResponseBody ?? string.Empty,
                ExecutionTimeMs = (double)x.ExecutionTimeMs,
                StatusCode = x.StatusCode,
                CreatedDate = x.CreatedDate,
                CurlCommand = x.Curl
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<AuditLogDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
    }
}
