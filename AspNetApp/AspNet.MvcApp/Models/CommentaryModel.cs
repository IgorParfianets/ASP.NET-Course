using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetArticle.MvcApp.Models
{
    public class CommentaryModel
    {
        public Guid Id { get; set; }

        [MaxLength(500, ErrorMessage = "Слишком длинный комментарий, не более 500 символов")]
        [MinLength(3, ErrorMessage = "Слишком короткий комментарий, не менее 3 символов")]
        [Required]
        public string Description { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public bool IsEdited { get; set; }
    }
}
