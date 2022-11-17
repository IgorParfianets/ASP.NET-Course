using System.Security.Claims;
using System.Text;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace AspNetArticle.Business.Services
{
    public class SendMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SendMessageService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task SendArticlesToUsers()
        {
            var users = await _unitOfWork.Users
                .Get()
                .Where(user => user.Spam)
                .Select(user => user.Email).ToListAsync();

            if (users.Any())
            {
                var articles = _unitOfWork.Articles
                    .Get()
                    .Where(art => art.PublicationDate > DateTime.Today)
                    .Select(art => art.Title)
                    .Aggregate((i, j) => i + "/n" + j);

                StringBuilder sb = new StringBuilder();
                sb.Append("Новости дня\n");
                sb.Append(articles);
                sb.Append("\nПереходите по ссылке на наш новостной портал\n");

                //var test = await _unitOfWork.Articles
                //    .Get()
                //    .Where(art => art.PublicationDate > DateTime.Today)
                //    .Select(art => new {Key = art.Title, Value = art.SourceUrl})
                //    .Aggregate((i, j) => i + "/n" + j);


            }
        }

    }
}
