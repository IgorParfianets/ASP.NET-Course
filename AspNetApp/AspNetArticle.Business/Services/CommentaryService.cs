using AsoNetArticle.Data.CQS.Commands;
using AsoNetArticle.Data.CQS.Handers;
using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services
{
    public class CommentaryService : ICommentaryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CommentaryService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<CommentDto> GetCommentByIdAsync(Guid id)
        {
            return _mapper.Map<CommentDto>(await _mediator.Send(new GetCommentByIdQuery() { Id = id}));
        }

        public async Task CreateCommentAsync(CommentDto dto)
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity == null) 
                throw new ArgumentException(nameof(entity));

            await _mediator.Send(new AddCommentCommand() {Comment = entity});
        }

        public async Task UpdateCommentAsync(CommentDto dto) 
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity == null)
                throw new ArgumentException(nameof(entity));     
            
            var result = await _mediator.Send(new UpdateCommentCommand() { Comment = entity });
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id)
        {
            var userAllComments = await _mediator.Send(new GetAllCommentsByUserIdQuery() {UserId = id});

            if (userAllComments != null)
                return userAllComments
                    .Select(com => _mapper.Map<CommentDto>(com))
                    .ToArray();

            return Array.Empty<CommentDto>();
        }

        public async Task<IEnumerable<CommentDto>> GelAllCommentsAsync() 
        {
            var comments = await _mediator.Send(new GetAllCommentsQuery());

            if (comments != null)
                return comments
                    .Select(com => _mapper.Map<CommentDto>(com))
                    .ToArray();

            return Array.Empty<CommentDto>();
        }

        public async Task<IEnumerable<CommentaryWithUserDto>> GetAllCommentsWithUsersByArticleIdAsync(Guid id) 
        {
            var comments = await _mediator.Send(new GetAllCommentsWithUsersByArticleIdQuery() { ArticleId = id});

            if (comments != null)
                return comments
                    .Select(com => _mapper.Map<CommentaryWithUserDto>(com))
                    .ToArray();

            return Array.Empty<CommentaryWithUserDto>();
        }

        public async Task DeleteCommentById(Guid id)
        {
            await _mediator.Send(new DeleteCommentByIdCommand() { Id = id });
        }

        //Api Method not implemented
        public async Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleId(Guid articleId, Guid userId) // not implement for CQS
        {
            var entities = _unitOfWork.Comments.Get();

            if (!Guid.Empty.Equals(articleId))
            {
                entities = entities.Where(ent => ent.ArticleId.Equals(articleId));
            }

            if (!Guid.Empty.Equals(userId))
            {
                entities = entities.Where(ent => ent.UserId.Equals(userId));
            }

            var result = (await entities.ToListAsync())
                .Select(ent => _mapper.Map<CommentDto>(ent))
                .ToList();

            return result;
        }
    }
}
