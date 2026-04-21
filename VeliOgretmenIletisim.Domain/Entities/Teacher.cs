using VeliOgretmenIletisim.Domain.Common;

namespace VeliOgretmenIletisim.Domain.Entities;

public class Teacher : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Branch { get; set; } = string.Empty;
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
