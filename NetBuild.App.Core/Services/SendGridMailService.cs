using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetBuild.App.Core.Configuration;
using NetBuild.App.Core.Services.Interface;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace NetBuild.App.Core.Services
{
    public class SendGridMailService : IMailService
    {
        private readonly SendGridConfiguration _config;
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger _logger;

        public SendGridMailService(IOptions<SendGridConfiguration> options, ISendGridClient sendGridClient, ILogger<SendGridMailService> logger)
        {
            _config = options.Value;
            _sendGridClient = sendGridClient;
            _logger = logger;
        }

        public async Task<bool> SendMailAsync(string to, string subject, string body, bool htmlContent = false)
        {
            try
            {
                var mail = new SendGridMessage
                {
                    From = new EmailAddress(_config.From),
                    Subject = subject,
                };

                if (htmlContent)
                {
                    mail.HtmlContent = body;
                }
                else
                {
                    mail.PlainTextContent = body;
                }

                mail.AddTo(new EmailAddress(to));

                return (await _sendGridClient.SendEmailAsync(mail)).StatusCode == HttpStatusCode.Accepted;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return false;
            }
        }

        public async Task<bool> SendMailAsync(string to, string subject, object[] subjectArgs, string body, object[] bodyArgs, bool isHtmlContent = false)
        {
            try
            {
                return await SendMailAsync(to, subjectArgs == null ? subject : string.Format(subject, subjectArgs), bodyArgs == null ? body : string.Format(body, bodyArgs), isHtmlContent);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return false;
            }
        }
    }
}
