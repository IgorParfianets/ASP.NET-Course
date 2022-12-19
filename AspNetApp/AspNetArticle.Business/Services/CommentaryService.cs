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
            //return _mapper.Map<CommentDto>(await _unitOfWork.Comments.GetByIdAsync(id));
            return _mapper.Map<CommentDto>(await _mediator.Send(new GetCommentByIdQuery() { Id = id}));
        }

        public async Task CreateCommentAsync(CommentDto dto)
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity == null) 
                throw new ArgumentException(nameof(entity));

            await _mediator.Send(new AddCommentCommand() {Comment = entity});
            //await _unitOfWork.Comments.AddAsync(entity);
            //var result = await _unitOfWork.Commit();
        }

        public async Task UpdateCommentAsync(CommentDto dto) // Create method create ReNew Guid for Exist Entity
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));                
            }
            await _mediator.Send(new UpdateCommentCommand() { Comment = entity });
            //_unitOfWork.Comments.Update(entity);
            //await _unitOfWork.Commit();
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id)
        {
            var userAllComments = await _mediator.Send(new GetAllCommentsByUserIdQuery() {UserId = id});

            if (userAllComments != null)
                return userAllComments.Select(com => _mapper.Map<CommentDto>(com)).ToArray();

            return Array.Empty<CommentDto>();

            //var userAllComments = await _unitOfWork.Comments
            //    .Get()
            //    .Where(com => 
            //        com.UserId.Equals(id))
            //    .Include(com => com.Article)
            //    .Select(com => 
            //        _mapper.Map<CommentDto>(com))
            //    .ToListAsync();
        }

        public async Task<IEnumerable<CommentDto>> GelAllCommentsAsync() // not implement for CQS
        {
            var comments = await _mediator.Send(new GetAllCommentsQuery());

            if (comments != null)
                return comments.Select(com => _mapper.Map<CommentDto>(com)).ToArray();

            return Array.Empty<CommentDto>();
            //return await _unitOfWork.Comments
            //    .Get()
            //    .Include(com => com.User)
            //    .Include(com => com.Article)
            //    .Select(com => _mapper.Map<CommentDto>(com))
            //    .ToListAsync();
        }

        public async Task<IEnumerable<CommentaryWithUserDto>> GetAllCommentsWithUsersByArticleIdAsync(Guid id) // For Details // not implement for CQS
        {
            var comments = await _mediator.Send(new GetAllCommentsWithUsersByArticleIdQuery() { ArticleId = id});

            if (comments != null)
                return comments.Select(com => _mapper.Map<CommentaryWithUserDto>(com)).ToArray();

            return Array.Empty<CommentaryWithUserDto>();
            //GetAllCommentsWithUsersByArticleIdQuery
            //var articleAllComments = await _unitOfWork.Comments
            //    .Get()
            //    .Where(article =>
            //        article.ArticleId.Equals(id))
            //    .Include(com => com.User)
            //    .Select(cmt =>
            //        _mapper.Map<CommentaryWithUserDto>(cmt))
            //    .ToListAsync();

            //return articleAllComments;
        }

        public async Task DeleteCommentById(Guid id)
        {
            await _mediator.Send(new DeleteCommentByIdCommand() { Id = id });
            //var entity = await _unitOfWork.Comments.GetByIdAsync(id);

            //if(entity != null)
            //    _unitOfWork.Comments.Remove(entity);

            //await _unitOfWork.Commit();
        }

        //Api Method not implemented
        public async Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleId(Guid? article, Guid? user) // not implement for CQS
        {
            var entities = _unitOfWork.Comments.Get();

            if (article != null && !Guid.Empty.Equals(article))
            {
                entities = entities.Where(ent => ent.ArticleId.Equals(article));
            }

            if (user != null && !Guid.Empty.Equals(user))
            {
                entities = entities.Where(ent => ent.UserId.Equals(user));
            }

            var result = (await entities.ToListAsync())
                .Select(ent => _mapper.Map<CommentDto>(ent))
                .ToList();

            return result;
        }
    }
}
