using AsoNetArticle.Data.CQS.Commands;
using AspNetArticle.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Handers.CommandHanders
{
    public class UpdateArticleTextDevIoCommandHandler : IRequestHandler<UpdateArticleTextDevIoCommand, Unit>
    {
        private readonly AggregatorContext _context;

        public UpdateArticleTextDevIoCommandHandler(AggregatorContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateArticleTextDevIoCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id.Equals(request.ArticleId), cancellationToken);

            if (article != null)
            {
                article.Text = request.Text;
            }

            _context.SaveChanges();
            return Unit.Value;
        }
    }
}
