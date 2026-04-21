using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Complaints.Commands.CreateComplaint;

public class CreateComplaintCommand : IRequest<Result>
{
    public string Subject { get; set; }
    public string Description { get; set; }
}
