using VeliOgretmenIletisim.Application.Interfaces.Notifications;

namespace VeliOgretmenIletisim.Infrastructure.Services;

public class MockEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL MOCK] To: {to}, Subject: {subject}");
        return Task.CompletedTask;
    }
}

public class MockSmsService : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string message)
    {
        Console.WriteLine($"[SMS MOCK] To: {phoneNumber}, Message: {message}");
        return Task.CompletedTask;
    }
}
