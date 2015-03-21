using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StackOverflow.Domain.CustomDataNotations;

namespace StackOverflow.Web.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [PasswordHasCapital(ErrorMessage = "Must Contain At Least 1 Capital Letter")]
        [PasswordHasRequiredLength(ErrorMessage = "Must Be Between 8-16 characters")]
        [PasswordHasVowelAndNumber(ErrorMessage = "Must Contain at least 1 Vowel and at least 1 number")]
        [PasswordHasNoRepeatingValues]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Password Can Only Accept Alphanumeric characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public Guid OwnerId { get; set; }
    }
}