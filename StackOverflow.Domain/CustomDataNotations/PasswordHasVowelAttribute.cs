using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class PasswordHasVowelAndNumberAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string strValue = value.ToString().ToLower();
            if ((strValue.Contains("a") || strValue.Contains("e") || strValue.Contains("i")
                || strValue.Contains("o") || strValue.Contains("u")) 
                && strValue.Any(char.IsDigit)) return true;

            return false;
        }
    }
}
