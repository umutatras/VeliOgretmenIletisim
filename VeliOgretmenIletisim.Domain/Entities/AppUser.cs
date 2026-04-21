using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Domain.Common;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Domain.Entities;

public class AppUser : IdentityUser<Guid>, IAuditEntity, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }
    public string? ProfilePicturePath { get; set; }

    public Teacher? TeacherProfile { get; set; }
    public Parent? ParentProfile { get; set; }

    // IAuditEntity
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
}
