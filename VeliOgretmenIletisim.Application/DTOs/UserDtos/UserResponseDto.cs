namespace VeliOgretmenIletisim.Application.DTOs.UserDtos;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
