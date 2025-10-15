using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service d'envoi d'emails
/// </summary>
public class EmailService : IEmailService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;

        // Configuration SMTP
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@example.com";
        _fromName = configuration["Email:FromName"] ?? "E-Commerce";
    }

    public async Task SendEmailAsync(string userId, string subject, string htmlBody)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            throw new Exception("Utilisateur ou email introuvable");
        }

        await SendEmailToAddressAsync(user.Email, subject, htmlBody);
    }

    public async Task SendEmailToAddressAsync(string toEmail, string subject, string htmlBody)
    {
        // En développement, on peut simplement logger l'email
        if (string.IsNullOrEmpty(_smtpUsername))
        {
            Console.WriteLine($"[EMAIL] To: {toEmail}");
            Console.WriteLine($"[EMAIL] Subject: {subject}");
            Console.WriteLine($"[EMAIL] Body: {htmlBody}");
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
            throw;
        }
    }
}
