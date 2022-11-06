using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetArticle.MvcApp.Models
{
    public class UserLoginModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Input your email")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
