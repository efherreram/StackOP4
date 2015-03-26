using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StackOverflow.Domain.CustomDataNotations;

namespace StackOverflow.Web.Models
{
    public class AddNewAnswerModel
    {
        [StringLength(maximumLength: 1500, ErrorMessage = "La Descripcion debe contener minimo 50 caracteres", MinimumLength = 50)]
        [WordCount(ErrorMessage = "La Descripcion debe tener Por Lo Menos 5 palabras")]
        public string Description { get; set; }
    }
}