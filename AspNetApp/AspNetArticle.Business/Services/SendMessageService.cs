using System.Text;
using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Business.Models;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace AspNetArticle.Business.Services
{
    public class SendMessageService : ISendMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly MailSettings _mailSettings;

        public SendMessageService(IUnitOfWork unitOfWork,
            IOptions<MailSettings> mailSettings,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mailSettings = mailSettings.Value;
            _mediator = mediator;
        }

        public async Task GetArticlesAndUsersForMessage()
        {
            var usersEmails = await _mediator.Send(new GetUserEmailsForSendSpamQuery());
            //var usersEmails = await _unitOfWork.Users
            //    .Get()
            //    .Where(user => user.Spam)
            //    .Select(user => user.Email)
            //    .ToListAsync();

            if (usersEmails != null && usersEmails.Any())
            {
                var articles = (await _mediator.Send(new GetAllArticlesQuery()))
                        .Where(art => art.PublicationDate > DateTime.Today)
                        .OrderByDescending(art => art.Rate)
                        .Take(5)
                        .Select(art => $"{art.Title}\n{art.SourceUrl}\n")
                        .ToArray()
                    .Aggregate((i, j) => i + Environment.NewLine + j);
                //var articles = (await _unitOfWork.Articles
                //        .Get()
                //        .Where(art => art.PublicationDate > DateTime.Today)
                //        .OrderByDescending(art => art.Rate)
                //        .Take(5)
                //        .Select(art => $"{art.Title}\n{art.SourceUrl}\n")
                //        .ToArrayAsync())
                //    .Aggregate((i, j) => i + Environment.NewLine + j);

                if (string.IsNullOrEmpty(articles))
                    return;

                var sb = new StringBuilder();
                sb.Append("Топ 5 хороших новостей за сегодня\n\n");
                sb.Append(articles);
                sb.Append("\n\nПереходите по ссылкам на новостной портал\n");

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

                using (var smtp = new SmtpClient())
                {
                    smtp.Timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                    await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, true);
                    await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                    await smtp.SendAsync(message);

                    await smtp.DisconnectAsync(true);
                }    
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(SendEmailAsync)} method failed with model {mailRequest}");
                throw;
            }
            
        }
    }
}
