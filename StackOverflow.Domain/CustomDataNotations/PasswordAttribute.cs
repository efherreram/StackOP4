using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.CustomDataNotations
{
    public class PasswordAttribute:ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            var strValue = value.ToString();
            var strValueInLower = strValue.ToLower();

            if (strValue.Equals(strValueInLower))
            {
                return false;
            }

            return true;

        }


    }
}
