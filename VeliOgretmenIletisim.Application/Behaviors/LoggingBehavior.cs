using MediatR;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.Json;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _uow;

    public LoggingBehavior(IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService, IUnitOfWork uow)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
        _uow = uow;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var timer = new Stopwatch();
        timer.Start();

        var context = _httpContextAccessor.HttpContext;
        string requestBody = string.Empty;
        try { requestBody = JsonSerializer.Serialize(request); } catch { requestBody = "[Serialization Error]"; }

        TResponse response;
        bool isSuccess = true;
        string? errorMessage = null;

        try
        {
            response = await next();
        }
        catch (Exception ex)
        {
            isSuccess = false;
            errorMessage = ex.Message;
            throw;
        }
        finally
        {
            timer.Stop();

            var auditLog = new AuditLog
            {
                UserId = _currentUserService.UserId?.ToString(),
                ActionName = requestName,
                HttpMethod = context?.Request.Method,
                RequestPath = context?.Request.Path,
                RequestBody = requestBody,
                ClientIp = context?.Connection.RemoteIpAddress?.ToString(),
                ExecutionTimeMs = timer.ElapsedMilliseconds,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                StatusCode = context?.Response.StatusCode ?? 0,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId?.ToString() ?? "System"
            };

            // Basic cURL generation for logging
            auditLog.Curl = $"curl -X {auditLog.HttpMethod} \"{auditLog.RequestPath}\" -d '{auditLog.RequestBody}'";

            await _uow.GetRepository<AuditLog>().AddAsync(auditLog);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
