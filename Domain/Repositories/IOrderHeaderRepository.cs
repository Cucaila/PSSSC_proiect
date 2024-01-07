using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IOrderHeaderRepository
    {
        TryAsync<List<int>> TryGetExistingOrders(IEnumerable<int> ordertsToCheck);

    }
}
