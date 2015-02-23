using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class AddNewQuestionModel
    {

        [Required(ErrorMessage = "Field Required")]
        public string Title { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "Description not Valid", MinimumLength = 10)]
        public string Description { get; set; }
    }
}