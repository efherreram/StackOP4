using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class PasswordHasRequiredLengthAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string strValue = value.ToString();
            if (strValue.Length >= 8 && strValue.Length <= 16) return true;

            return false;
        }
    }
}
