namespace VeliOgretmenIletisim.Application.Interfaces.Notifications;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}

public interface IWhatsAppService
{
    Task SendWhatsAppLinkAsync(string phoneNumber, string message);
}
