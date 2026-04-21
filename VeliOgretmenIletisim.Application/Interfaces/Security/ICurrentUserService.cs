namespace VeliOgretmenIletisim.Application.Interfaces.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
}
