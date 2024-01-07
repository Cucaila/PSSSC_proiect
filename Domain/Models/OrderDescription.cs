using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Domain.Models
{
    public record OrderDescription
    {
        public const string Pattern = "^[a-zA-Z]+$";
        private static readonly Regex validCode = new(Pattern);
        public string Description { get; }
        public OrderDescription(string description)
        {
            if (IsValid(description))
            {
                Description = description;
            }
            else
            {
                throw new InvalidatedCustomerOrder("");
            }

        }

        private static bool IsValid(string stringValue) => validCode.IsMatch(stringValue);

        public override string ToString()
        {
            return $" description : {Description}";
        }

        public static bool TryParse(string stringValue, out OrderDescription orderDescription)
        {
            bool isValid = false;
            orderDescription = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                orderDescription = new(stringValue);
            }

            return isValid;
        }

        public static Option<OrderDescription> TryParseOrderDescription(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<OrderDescription>(new(stringValue));
            }
            else
            {
                return None;
            }
        }

    }
}
