using Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IProductRepository
    {
        TryAsync<List<OrderRegistrationCode>> TryGetExistingOrders(IEnumerable<string> ordersToCheck);

    }
}
