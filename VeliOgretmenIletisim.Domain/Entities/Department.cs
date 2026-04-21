using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Department : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
