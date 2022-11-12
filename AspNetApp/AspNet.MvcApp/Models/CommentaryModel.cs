using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetArticle.MvcApp.Models
{
    public class CommentaryModel
    {
        
        [MaxLength(500, ErrorMessage = "Слишком большой комментарий")]
        [MinLength(3, ErrorMessage = "Слишком короткий комментарий")]
        [Required]
        public string Description { get; set; }
        public Guid ArticleId { get; set; } //Возможо не нужно
        //public Guid UserId { get; set; }  //Возможо не нужно
    }
}
