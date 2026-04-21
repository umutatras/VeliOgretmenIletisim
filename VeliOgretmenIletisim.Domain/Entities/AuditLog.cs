using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class AuditLog : BaseEntity, IAuditEntity
{
    public string? UserId { get; set; }
    public string? ActionName { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? Curl { get; set; }
    public string? ClientIp { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public long ExecutionTimeMs { get; set; }

    // IAuditEntity implementation
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
}
