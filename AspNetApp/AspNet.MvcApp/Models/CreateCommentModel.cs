using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetArticle.MvcApp.Models
{
    public class CreateCommentModel
    {
        public Guid Id { get; set; }

        [MaxLength(1000, ErrorMessage = "Слишком длинный комментарий, не более 500 символов")]
        [MinLength(2, ErrorMessage = "Слишком короткий комментарий, не менее 2 символов")]
        [Required(ErrorMessage = "Пустой комментарий")]
        public string Description { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public bool IsEdited { get; set; }
    }
}
