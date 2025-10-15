namespace ECommerce.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string userId, string subject, string htmlBody);
    Task SendEmailToAddressAsync(string toEmail, string subject, string htmlBody);
}
