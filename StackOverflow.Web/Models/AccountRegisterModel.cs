using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Routing.Constraints;
using StackOverflow.Domain.CustomDataNotations;

namespace StackOverflow.Web.Models
{
    public class AccountRegisterModel
    {
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength:50, MinimumLength = 2)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordHasCapital(ErrorMessage = "Must Contain At Least 1 Capital Letter")]
        [PasswordHasRequiredLength(ErrorMessage = "Must Be Between 8-16 characters")]
        [PasswordHasVowelAndNumber(ErrorMessage = "Must Contain at least 1 Vowel and at least 1 number")]
        [PasswordHasNoRepeatingValues]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Password Can Only Accept Alphanumeric characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Confirmation of Password Differs")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string ErrorMessage { get; set; }
    }
}