using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using StackOverflow.Domain.CustomDataNotations;

namespace StackOverflow.Web.Models
{
    public class AddNewQuestionModel
    {

        [Required(ErrorMessage = "Field Required")]
        [StringLength(50, ErrorMessage = "Max characters: 50")]
        [WordCount(ErrorMessage = "At Least 3 words")]
        public string Title { get; set; }
        [Required]
        [DescriptionWordCount(ErrorMessage = "At Least 5 Words")]
        public string Description { get; set; }
    }
}