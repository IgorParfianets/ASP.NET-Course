﻿using AspNetArticle.Core;
using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateUserCommand : IRequest<int>
    {
        public Guid UserId { get; set; }
        public List<PatchModel> PatchData { get; set; }
    }
}
