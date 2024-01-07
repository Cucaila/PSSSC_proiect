using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Domain.Models
{
    public class OrderAddress
    {
        public const string Pattern = "^[a-zA-Z]+$";
        private static readonly Regex validCode = new(Pattern);
        public string Address { get; }
        public OrderAddress(string address)
        {
            if (IsValid(address))
            {
                Address = address;
            }
            else
            {
                throw new InvalidatedCustomerOrder("");
            }

        }

        private static bool IsValid(string stringValue) => validCode.IsMatch(stringValue);

        public override string ToString()
        {
            return $" Adress : {Address}";
        }

        public static bool TryParse(string stringValue, out OrderAddress orderAddress)
        {
            bool isValid = false;
            orderAddress = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                orderAddress = new(stringValue);
            }

            return isValid;
        }
        public static Option<OrderAddress> TryParseOrderAddress(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<OrderAddress>(new(stringValue));
            }
            else
            {
                return None;
            }
        }
    }
}
