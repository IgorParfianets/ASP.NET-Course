
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;


public class CommentProfile : Profile
{
    public CommentProfile()
    {
        // For Entity -> Dto & Dto -> Entity
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
                    => opt.MapFrom(comment => comment.User))
            .ForMember(dto => dto.IsEdited,
                opt
                    => opt.MapFrom(comment => comment.IsEdited));



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

        CreateMap<Comment, CommentaryWithUserDto>()
            .ForMember(dto => dto.CommentId,
                opt
                    => opt.MapFrom(comment => comment.Id))
            .ForMember(dto => dto.CommentDescription,
                opt
                    => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublishedDate,
                opt
                    => opt.MapFrom(comment => comment.PublicationDate))
            .ForMember(dto => dto.UserId,
                opt
                    => opt.MapFrom(comment => comment.UserId))
            .ForMember(dto => dto.Username,
                opt
                    => opt.MapFrom(comment => comment.User.UserName))
            .ForMember(dto => dto.Email,
                opt
                    => opt.MapFrom(comment => comment.User.Email))
            .ForMember(dto => dto.AccountCreated,
                opt
                    => opt.MapFrom(comment => comment.User.AccountCreated))
            .ForMember(comment => comment.IsEdited,
                opt
                    => opt.MapFrom(dto => dto.IsEdited));


        CreateMap<CommentaryModel, CommentDto>()
            .ForMember(dto => dto.Id,
                opt
                    => opt.MapFrom(comment => Guid.NewGuid()))
            .ForMember(dto => dto.Description,
                opt
                    => opt.MapFrom(comment => comment.Description))
            .ForMember(dto => dto.PublicationDate,
                opt
                    => opt.MapFrom(comment => DateTime.Now));

        CreateMap<CommentDto, CommentaryModel>()
            .ForMember(dto => dto.ArticleId,
                opt
                    => opt.MapFrom(comment => comment.ArticleId))
            .ForMember(dto => dto.Description,
                opt
                    => opt.MapFrom(comment => comment.Description));









    }
}
