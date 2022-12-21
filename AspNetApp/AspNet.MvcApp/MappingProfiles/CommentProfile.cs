
using AspNetArticle.Core;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using System.Drawing.Text;

namespace AspNetArticle.MvcApp.MappingProfiles;


public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dto => dto.Id,
                opt
                    => opt.MapFrom(comment => comment.Id))
            .ForMember(dto => dto.Description,
                opt
                    => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublicationDate,
                opt
                    => opt.MapFrom(comment => comment.PublicationDate))
            .ForMember(dto => dto.UserId,
                opt
                    => opt.MapFrom(comment => comment.UserId))
            .ForMember(dto => dto.IsEdited,
                opt
                    => opt.MapFrom(comment => comment.IsEdited))
            .ForMember(dto => dto.ArticleName,
                opt
                    => opt.MapFrom(comment => comment.Article.Title))
            .ForMember(dto => dto.Username,
                opt
                    => opt.MapFrom(comment => comment.User.UserName))
            .ForMember(dto => dto.Email,
            opt
                => opt.MapFrom(comment => comment.User.Email));

        CreateMap<CommentDto, Comment>()
            .ForMember(comment => comment.Id, 
                opt 
                    => opt.MapFrom(dto => dto.Id))
            .ForMember(comment => comment.Description, 
                opt
                    => opt.MapFrom(dto => dto.Description))
            .ForMember(comment => comment.PublicationDate, 
                opt 
                    => opt.MapFrom(dto => dto.PublicationDate))
            .ForMember(comment => comment.IsEdited,
            opt
                => opt.MapFrom(dto => dto.IsEdited));

        CreateMap<CreateCommentModel, CommentDto>()
            .ForMember(dto => dto.Id,
                opt
                    => opt.MapFrom(comment => Guid.NewGuid()))
            .ForMember(dto => dto.Description,
                opt
                    => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublicationDate,
                opt
                    => opt.MapFrom(comment => DateTime.Now));

        CreateMap<CommentDto, CreateCommentModel>()
            .ForMember(dto => dto.ArticleId,
                opt
                    => opt.MapFrom(comment => comment.ArticleId))
            .ForMember(dto => dto.Description,
                opt
                    => opt.MapFrom(comment => comment.Description));

        CreateMap<Comment, CommentaryWithUserDto>().ConvertUsing(new FromCommentToCommentaryWithUserConverter());

    }

    private class FromCommentToCommentaryWithUserConverter : ITypeConverter<Comment, CommentaryWithUserDto>
    {
        public CommentaryWithUserDto Convert(Comment source, CommentaryWithUserDto destination, ResolutionContext context)
        {
            CommentaryWithUserDto commentWithUserModel = new CommentaryWithUserDto
            {
                CommentId = source.Id,
                CommentDescription = source.Description,
                PublishedDate = source.PublicationDate,
                UserId = source.UserId,
                Username = source.User.UserName,
                Email = source.User.Email,
                IsEdited = source.IsEdited
            };

            var days = (DateTime.Now - source.User.AccountCreated).Days;

            switch (days)
            {
                case <= 7:
                    commentWithUserModel.Status = MembershipStatus.Новичек;
                    break;
                case <= 30 when days > 7:
                    commentWithUserModel.Status = MembershipStatus.Местный;
                    break;
                case <= 180 when days > 30:
                    commentWithUserModel.Status = MembershipStatus.Опытный;
                    break;
                case > 180:
                    commentWithUserModel.Status = MembershipStatus.Ветеран;
                    break;
            }

            if (source.User.Avatar != null)
            {
                var loadAvatar = System.Convert.ToBase64String(source.User.Avatar);
                commentWithUserModel.Avatar = loadAvatar;
            }
            return commentWithUserModel;
        }
    }
}
