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
        [PasswordAttribute]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [PasswordAttribute]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public Guid OwnerId { get; set; }
    }
}