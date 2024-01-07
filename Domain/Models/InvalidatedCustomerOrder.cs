using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Serializable]
    public class InvalidatedCustomerOrder : Exception
    {
        public InvalidatedCustomerOrder()
        {
        }

        public InvalidatedCustomerOrder(string? message) : base(message)
        {
        }

        public InvalidatedCustomerOrder(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public InvalidatedCustomerOrder(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
