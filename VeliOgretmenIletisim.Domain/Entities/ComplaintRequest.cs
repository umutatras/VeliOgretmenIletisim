using VeliOgretmenIletisim.Domain.Common;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Domain.Entities;

public class ComplaintRequest : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public bool IsRead { get; set; }
    public string? AdminResponse { get; set; }
    public ComplaintStatus Status { get; set; }

    public Guid ParentId { get; set; }
    public Parent Parent { get; set; } = null!;

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
