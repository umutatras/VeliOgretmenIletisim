using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Student : BaseEntity, IAuditEntity, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;

    public Guid ParentId { get; set; }
    public Parent Parent { get; set; } = null!;

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
