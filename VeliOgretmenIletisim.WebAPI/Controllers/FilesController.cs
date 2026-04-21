using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Files;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class FilesController : BaseApiController
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "general")
    {
        try
        {
            var path = await _fileService.UploadFileAsync(file, folder);
            return Ok(Result<string>.Success(path, "File uploaded successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(ex.Message));
        }
    }
}
