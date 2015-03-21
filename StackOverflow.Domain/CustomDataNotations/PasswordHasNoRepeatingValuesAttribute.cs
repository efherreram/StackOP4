using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ValueProviders.Providers;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class PasswordHasNoRepeatingValuesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string strValue = value.ToString().ToLower();

            for (int i = 0; i < strValue.Length - 1; i++)
            {
                if (strValue.ElementAt(i) == strValue.ElementAt(i + 1))
                {
                    ErrorMessage = "Password can't contain repeating characters. Ej. aa, dd, tt...";
                    return false;
                }
            }

            return true;
        }
    }
}
