using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateArticleOnlinerCommandHandler : IRequestHandler<UpdateArticleOnlinerCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public UpdateArticleOnlinerCommandHandler(AggregatorContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(UpdateArticleOnlinerCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id.Equals(request.ArticleId), cancellationToken);

            if(article != null)
            {
                if (!string.IsNullOrEmpty(request.Text))
                    request.Text = Regex.Replace(request.Text, @"<a([^>]+)>(.+?)<\/a>", " ");

                if (!string.IsNullOrEmpty(request.ImageUrl))
                    article.ImageUrl = request.ImageUrl;

                _context.SaveChanges();
            } 
            return Unit.Value;
        }
    }
}
