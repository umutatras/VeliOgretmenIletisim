using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Interfaces.Security;

public interface IJwtProvider
{
    string GenerateToken(AppUser user);
}
