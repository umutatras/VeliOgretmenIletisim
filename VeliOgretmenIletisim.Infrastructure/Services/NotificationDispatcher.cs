using Hangfire;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;

namespace VeliOgretmenIletisim.Infrastructure.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public NotificationDispatcher(IBackgroundJobClient backgroundJobClient, IEmailService emailService, ISmsService smsService)
    {
        _backgroundJobClient = backgroundJobClient;
        _emailService = emailService;
        _smsService = smsService;
    }

    public void ScheduleEmail(string to, string subject, string body)
    {
        _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(to, subject, body));
    }

    public void ScheduleSms(string phoneNumber, string message)
    {
        _backgroundJobClient.Enqueue(() => _smsService.SendSmsAsync(phoneNumber, message));
    }
}
