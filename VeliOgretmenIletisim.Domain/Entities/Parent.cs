using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Parent : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Occupation { get; set; } = string.Empty;
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public ICollection<Student> Children { get; set; } = new List<Student>();

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
