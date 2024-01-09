using Domain.Models;
using Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.OrdersCart;
using static Domain.Models.ShippingCart;
using static Domain.Models.ShippingOrderEvent;
using static LanguageExt.Prelude;

namespace Domain
{
    public class ShippingOrderWorkflow
    {
        private readonly ILogger<ShippingOrderWorkflow> logger;
        private readonly IOrderHeaderRepository orderHeaderRepository;
        private readonly IOrderLineRepository orderLineRepository;
        private readonly IProductRepository productRepository;

        public ShippingOrderWorkflow(IOrderHeaderRepository orderHeaderRepository, IProductRepository productRepository, IOrderLineRepository orderLineRepository, ILogger<ShippingOrderWorkflow> logger)
        {
            this.orderHeaderRepository = orderHeaderRepository;
            this.productRepository = productRepository;
            this.orderLineRepository = orderLineRepository;
            this.logger = logger;
        }

        public async Task<IShippingOrderEvent> ExecuteAsync(ShippingOrderCommand command)
        {
            // Crează un coș cu comenzile nevalidate pentru facturare
            UnshipedOrdersCart unshippedOrders = new UnshipedOrdersCart(command.InputOrders);

            // Validează comenzile
            var validatedResult = await ValidateOrdersForShipping(unshippedOrders);
            if (validatedResult.IsLeft)
            {
                return new ShippingOrderFailedEvent(((FailedShippingCart)validatedResult.LeftAsEnumerable().First()).Reason);

            }

            // Facturează comenzile validate
            var billingResult = await ShipOrdersAsync(validatedResult.RightAsEnumerable().First());
            if (billingResult.IsLeft)
            {
                return new ShippingOrderFailedEvent(((FailedShippingCart)billingResult.LeftAsEnumerable().First()).Reason);
            }

            // Salvează comenzile facturate
            var saveResult = await SaveShipedOrdersAsync(billingResult.RightAsEnumerable().First());
            if (saveResult.IsLeft)
            {
                return new ShippingOrderFailedEvent(((FailedShippingCart)saveResult.LeftAsEnumerable().First()).Reason);
            }

            // Dacă toate etapele au fost de succes, returnează un eveniment de succes
            var shippedOrdersCart = saveResult.RightAsEnumerable().First();
            return new ShippingOrderSucceededEvent(
                    shippedOrdersCart.ShippedOrder,
                    shippedOrdersCart.InvoiceNumber,
                    shippedOrdersCart.AmountBilled,
                    DateTime.Now
            );
        }


        //private async Task<Either<FailedBillingCart, BilledOrdersCart>> SaveBilledOrdersAsync(BilledOrdersCart

        // Metodele folosite în procesul de facturare
        private async Task<Either<IShippingCart, ValidatedOrdersCart>> ValidateOrdersForShipping(UnshipedOrdersCart cart)
        {
            var isValid = true;

            if (isValid)
            {
                // Convertim coșul într-unul validat pentru facturare
                var validatedOrders = new ValidatedOrdersCart(cart.OrdersList);
                return Right(validatedOrders);
            }
            else
            {
                // În caz de eșec, returnăm un coș de facturare eșuat
                return Left(new FailedShippingCart("Validation failed") as IShippingCart);
            }
        }


        private static async Task<Either<IShippingCart, ShipedOrdersCart>> ShipOrdersAsync(ValidatedOrdersCart validatedOrders)
        {
            try
            {
                var shipedOrders = new List<ShippedOrder>();
                decimal totalAmountBilled = 0;

                foreach (var validatedOrder in validatedOrders.OrdersList)
                {

                    var productId = validatedOrder.OrderRegistrationCode; // ID-ul produsului
                    var productName = validatedOrder.OrderDescription; // Numele produsului
                    var quantity = validatedOrder.OrderAmount; // Cantitatea produsului în comandă
                    var unitPrice = validatedOrder.OrderPrice; // Prețul unitar al produsului
                    var adress = validatedOrder.OrderAddress;

                    var lineTotal = (decimal)(quantity.Amount * unitPrice.Price);
                    var shipedLine = new ShippedOrder(productId, productName, quantity, adress, unitPrice);
                    shipedOrders.Add(shipedLine);
                    totalAmountBilled += lineTotal; // Adunăm totalul liniei la suma totală facturată
                }

                // Creăm un coș cu comenzile facturate
                var shippingDate = DateTime.UtcNow; // Data și ora actuală UTC
                var invoiceNumber = "Invoice" + shippingDate.Ticks.ToString(); // Un număr unic de factură

                var shipedOrdersCart = new ShipedOrdersCart(shipedOrders, invoiceNumber, totalAmountBilled, shippingDate);
                return Right(shipedOrdersCart);
            }
            catch (Exception ex)
            {
                // În caz de eroare, returnăm un coș de facturare eșuat
                return Left(new FailedShippingCart("Shipping failed: " + ex.Message) as IShippingCart);


            }

        }
        private async Task<Either<IShippingCart, ShipedOrdersCart>> SaveShipedOrdersAsync(ShipedOrdersCart shippedOrdersCart)
        {
            try
            {
                // Salvarea comenzilor facturate ar putea implica actualizarea unui header de comandă
                // și/sau inserarea liniilor de comandă facturate într-o tabelă corespunzătoare.
                //await orderHeaderRepository.SaveBilledOrderHeaderAsync(billedOrdersCart);
                //await orderLineRepository.SaveBilledOrderLinesAsync(billedOrdersCart.BilledOrderLines);

                // După ce datele au fost salvate cu succes, returnăm coșul de comenzi facturate.
                return Right(shippedOrdersCart);
            }
            catch (Exception ex)
            {
                // Dacă întâmpinăm o eroare în timpul salvării, returnăm o stângă (Left) cu eroarea.
                return Left(new FailedShippingCart($"Failed to save shiped orders: {ex.Message}") as IShippingCart);
            }
        }

    }
}
