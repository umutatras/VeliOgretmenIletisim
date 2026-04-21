using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Availability : BaseEntity, IAuditEntity, ISoftDelete
{
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; } = 15;
    public bool IsGroup { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
