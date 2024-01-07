using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record ValidatedCustomerOrder(OrderRegistrationCode OrderRegistrationCode, OrderDescription OrderDescription, OrderAmount OrderAmount, OrderAddress OrderAddress, OrderPrice OrderPrice)
    {
        public string toStringOrder()
        {
            return $"Registration Code: {OrderRegistrationCode.Value} \nDescription of Order: {OrderDescription.Description} \nAmount: {OrderAmount.Amount}\nAddress: {OrderAddress.Address} \nPrice per item: {OrderPrice.Price}";
        }
    }
}
