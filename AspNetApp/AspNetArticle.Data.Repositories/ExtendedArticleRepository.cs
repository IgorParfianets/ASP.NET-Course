using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Data.Repositories
{
    public class ExtendedArticleRepository : Repository<Article>, IExtendedArticleRepository
    {
        public ExtendedArticleRepository(AggregatorContext context) : base(context)
        {

        }

        public async Task UpdateArticleTextAsync(Guid id, string text) 
        {
            var article = await DbSet.FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (article != null)
            {
                article.Text = text;
            }
        }
    }
}
