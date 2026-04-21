using VeliOgretmenIletisim.Application.DTOs.UserDtos;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Mappings;

public static class UserMappingExtensions
{
    public static UserResponseDto MapToResponse(this AppUser user)
    {
        if (user == null) return null;

        return new UserResponseDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
