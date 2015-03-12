using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Routing.Constraints;
using StackOverflow.Domain.CustomDataNotations;

namespace StackOverflow.Web.Models
{
    public class AccountRegisterModel
    {
        [Required]
        [StringLength(15, ErrorMessage = "String Too Long")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordAttribute(ErrorMessage = "At Least One Capital")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Confirmation of Password Differs")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}