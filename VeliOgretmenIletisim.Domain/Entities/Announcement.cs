using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Announcement : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TargetAudience { get; set; }
    public string? AttachmentPath { get; set; }

    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
