using System;
using System.Globalization;
using System.Windows.Controls;

namespace AccountBookWin.Resource
{
    public class ConsumeMemoValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                string memo = value.ToString();
                if (memo.Trim().Equals(string.Empty))
                {
                    return new ValidationResult(false, "Memo can't be empty!");
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
