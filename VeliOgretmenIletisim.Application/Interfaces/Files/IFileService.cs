using Microsoft.AspNetCore.Http;

namespace VeliOgretmenIletisim.Application.Interfaces.Files;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    void DeleteFile(string filePath);
}
