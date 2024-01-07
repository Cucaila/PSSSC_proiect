using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ProductDbo
    {
        public int ProductId { get; set; }
        public string RegistrationCode { get; set; }
        public string Description { get; set; }
        public float Stock { get; set; }
    }
}
