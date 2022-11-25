using System.Text;
using AspNetArticle.Business.Models;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace AspNetArticle.Business.Services
{
    public class SendMessageService : ISendMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MailSettings _mailSettings;

        public SendMessageService(IUnitOfWork unitOfWork,
            IOptions<MailSettings> mailSettings)
        {
            _unitOfWork = unitOfWork;
            _mailSettings = mailSettings.Value;
        }

        public async Task GetArticlesAndUsersForMessage()
        {
            var usersEmails = await _unitOfWork.Users
                .Get()
                .Where(user => user.Spam)
                .Select(user => user.Email)
                .ToListAsync();

            if (usersEmails.Any())
            {
                var articles = (await _unitOfWork.Articles
                        .Get()
                        .Where(art => art.PublicationDate > DateTime.Today)
                        .Select(art => $"{art.Title}\n{art.SourceUrl}\n")
                        .ToListAsync())
                    .Aggregate((i, j) => i + Environment.NewLine + j);

                var sb = new StringBuilder();
                sb.Append("Свежие новости\n\n");
                sb.Append(articles);
                sb.Append("\n\nПереходите по ссылкам на наш новостной портал\n");

                foreach (var email in usersEmails)
                {
                    var request = new MailRequest()
                    {
                        ToEmail = email,
                        Body = sb.ToString(),
                        Subject = "Новости"
                    };
                    await SendEmailAsync(request);
                }
            }
        }

        private async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                message.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                message.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();


                builder.TextBody = mailRequest.Body;
                message.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SendEmailAsync)} method failed with model {mailRequest}");
                throw;
            }
            
        }
    }
}
