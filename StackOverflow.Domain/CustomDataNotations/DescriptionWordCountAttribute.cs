using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class DescriptionWordCountAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value.ToString().Split().Length >= 5;
        }
    }
}
