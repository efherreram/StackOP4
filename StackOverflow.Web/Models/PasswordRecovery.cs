using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StackOverflow.Domain.Entities;

namespace StackOverflow.Web.Models
{
    public class PasswordRecovery
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
    }
}