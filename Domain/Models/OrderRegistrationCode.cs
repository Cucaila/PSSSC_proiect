using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Domain.Models
{
    public record OrderRegistrationCode
    {
        public const string Pattern = "^RO[0-9]";
        private static readonly Regex validCode = new(Pattern);
        public string Value { get; }

        public OrderRegistrationCode(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidOrderRegistrationNumberException("");
            }
        }

        private static bool IsValid(string stringValue) => validCode.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static bool TryParse(string stringValue, out OrderRegistrationCode registrationNumber)
        {
            bool isValid = false;
            registrationNumber = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                registrationNumber = new(stringValue);
            }

            return isValid;
        }

        public static Option<OrderRegistrationCode> TryParseRegistrationCode(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<OrderRegistrationCode>(new(stringValue));
            }
            else
            {
                return None;
            }
        }
    }
}
