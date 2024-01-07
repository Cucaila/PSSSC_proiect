using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrderPrice
    {
        public float Price { get; }
        public OrderPrice(float price)
        {
            if (IsValid(price))
            {
                Price = price;
            }
            else
            {
                throw new InvalidatedCustomerOrder("");
            }
        }

        public static OrderPrice operator *(OrderPrice a, OrderAmount b) => new OrderPrice((a.Price * b.Amount));
        private static bool IsValid(float decValue) => decValue > 0;

        public override string ToString()
        {
            return $"Price: {Price}";
        }

        public static bool TryParse(string value, out OrderPrice orderPrice)
        {
            bool isValid = false;
            orderPrice = null;
            if (float.TryParse(value, out float floatValue))
            {
                if (IsValid(floatValue))
                {
                    isValid = true;
                    orderPrice = new(floatValue);
                }
            }

            return isValid;
        }

        public static Option<OrderPrice> TryParsePrice(float price)
        {
            if (IsValid(price))
            {
                return Some<OrderPrice>(new(price));
            }
            else
            {
                return None;
            }
        }
    }
}
