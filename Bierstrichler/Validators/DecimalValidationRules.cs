using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bierstrichler.Validators
{
    public class PositiveDecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string _strInt = value.ToString();
            decimal _decimal = -1;
            if (!Decimal.TryParse(_strInt, out _decimal))
                return new ValidationResult(false, "Der Wert muss eine Zahl sein!");
            if (_decimal < 0)
                return new ValidationResult(false, "Der Wert muss positiv sein!");
            return new ValidationResult(true, null);
        }
    }

    public class NegativeDecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string _strInt = value.ToString();
            decimal _decimal = -1;
            if (!Decimal.TryParse(_strInt, out _decimal))
                return new ValidationResult(false, "Der Wert muss eine Zahl sein!");
            if (_decimal > 0)
                return new ValidationResult(false, "Der Wert muss negativ sein!");
            return new ValidationResult(true, null);
        }
    }

    public class WholeNumberDecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string _strInt = value.ToString();
            decimal _decimal = -1;
            if (!Decimal.TryParse(_strInt, out _decimal))
                return new ValidationResult(false, "Der Wert muss eine Zahl sein!");
            if ((_decimal % 1) != 0)
                return new ValidationResult(false, "Der Wert muss eine ganze Zahl sein!");
            return new ValidationResult(true, null);
        }
    }
}