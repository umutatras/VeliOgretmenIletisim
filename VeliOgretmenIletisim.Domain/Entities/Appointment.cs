using VeliOgretmenIletisim.Domain.Common;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Appointment : BaseEntity, IAuditEntity, ISoftDelete
{
    public Guid AvailabilityId { get; set; }
    public Availability Availability { get; set; } = null!;

    public Guid ParentId { get; set; }
    public Parent Parent { get; set; } = null!;

    public AppointmentStatus Status { get; set; }
    public string? TeacherNote { get; set; }

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
