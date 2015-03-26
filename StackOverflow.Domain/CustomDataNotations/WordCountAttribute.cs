using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class WordCountAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string val = value.ToString();
            if (val.Split().Length < 3)
                return false;

            return true;
        }
    }
}
