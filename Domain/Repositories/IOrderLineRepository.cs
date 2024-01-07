using Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.OrdersCart;

namespace Domain.Repositories
{
    public interface IOrderLineRepository
    {
        TryAsync<List<CalculateCustomerOrder>> TryGetExistingOrders();

        TryAsync<Unit> TrySaveOrders(PlacedOrder order);
    }
}
