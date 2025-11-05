using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Tienda_Restaurante.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace Tienda_Restaurante.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string correo, string asunto, string cuerpo)
        {
            _logger.LogInformation("Preparando envío de correo a {Correo} con asunto '{Asunto}'", correo, asunto);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    _config["InfoCorreo:CorreoRemitente"],
                    _config["InfoCorreo:ClaveCorreo"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["InfoCorreo:CorreoRemitente"]),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(correo);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado correctamente a {Correo}", correo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {Correo}", correo);
                throw;
            }
        }
    }
}
