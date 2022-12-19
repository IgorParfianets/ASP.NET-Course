using AspNetArticle.Database.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class DeleteFavouriteArticleCommand : IRequest
    {
        public FavouriteArticle FavouriteArticle { get; set; }
    }
}
