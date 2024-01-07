using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class OrderLineDbo
    {
        public int OrderLineId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public float? Amount { get; set; }
        public float? Price { get; set; }
    }
}
