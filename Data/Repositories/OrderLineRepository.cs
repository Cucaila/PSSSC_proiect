using Data.Models;
using Domain.Models;
using Domain.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static Domain.Models.OrdersCart;

namespace Data.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly OrdersContext ordersContext;

        public OrderLineRepository(OrdersContext ordersContext)
        {
            this.ordersContext = ordersContext;
        }


        public TryAsync<List<CalculateCustomerOrder>> TryGetExistingOrders() => async () => (await (
                         from o in ordersContext.OrdersLine
                         join p in ordersContext.Products on o.ProductId equals p.ProductId
                         join oh in ordersContext.OrdersHeader on o.OrderId equals oh.OrderId
                         select new { o.OrderLineId, p.RegistrationCode, p.Description, o.Amount, oh.Address, o.Price, oh.Total })
                         .AsNoTracking()
                         .ToListAsync())
                         .Select(result => new CalculateCustomerOrder(
                                                   OrderRegistrationCode: new(result.RegistrationCode),
                                                   OrderDescription: new(result.Description),
                                                   OrderAmount: new(result.Amount ?? 0f),
                                                   OrderAddress: new(result.Address),
                                                   OrderPrice: new(result.Price ?? 0f),
                                                   FinalPrice: new(result.Total ?? 0f))
                         {
                             OrderLineId = result.OrderLineId
                         })
                         .ToList();

        public TryAsync<Unit> TrySaveOrders(PlacedOrder orders) => async () =>
        {
            var products = (await ordersContext.Products.ToListAsync()).ToLookup(product => product.RegistrationCode);
            var ordersHeader = (await ordersContext.OrdersHeader.ToListAsync()).ToLookup(order => order.OrderId);
            var newOrders = orders.CalculateCustomerOrders
                                    .Where(g => g.IsUpdated && g.OrderLineId == 0)
                                    .Select(g => new OrderLineDbo()
                                    {
                                        ProductId = products[g.OrderRegistrationCode.Value].Single().ProductId,
                                        OrderId = ordersHeader[g.OrderHeaderId].Single().OrderId,
                                        Amount = g.OrderAmount.Amount,
                                        Price = g.OrderPrice.Price,


                                    }); ;
            var updatedOrders = orders.CalculateCustomerOrders.Where(g => g.IsUpdated && g.OrderHeaderId > 0)
                                    .Select(g => new OrderLineDbo()
                                    {
                                        OrderId = g.OrderHeaderId,
                                        ProductId = products[g.OrderRegistrationCode.Value].Single().ProductId,
                                        Amount = g.OrderAmount.Amount,
                                        Price = g.OrderPrice.Price,

                                    });

            ordersContext.AddRange(newOrders);
            foreach (var entity in updatedOrders)
            {
                ordersContext.Entry(entity).State = EntityState.Modified;
            }

            await ordersContext.SaveChangesAsync();

            return unit;
        };
    }
}
