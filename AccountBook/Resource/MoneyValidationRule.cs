using System;
using System.Globalization;
using System.Windows.Controls;

namespace AccountBookWin.Resource
{
    public class MoneyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                decimal money = decimal.Parse(value.ToString());
                if (money <= 0)
                {
                    return new ValidationResult(false, "Consume money must be larger than 0 ");
                }
            }
            catch(Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }

            return new ValidationResult(true, null);
        }
    }
}
