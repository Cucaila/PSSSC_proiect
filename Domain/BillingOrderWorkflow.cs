using Domain.Models;
using Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.BillingCart;
using static Domain.Models.BillingOrderEvent;
using static Domain.Models.OrdersCart;
using static LanguageExt.Prelude;

namespace Domain
{
    public class BillingOrderWorkflow
    {
        private readonly ILogger<BillingOrderWorkflow> logger;
        private readonly IOrderHeaderRepository orderHeaderRepository;
        private readonly IOrderLineRepository orderLineRepository;
        private readonly IProductRepository productRepository;

        public BillingOrderWorkflow(IOrderHeaderRepository orderHeaderRepository, IProductRepository productRepository, IOrderLineRepository orderLineRepository, ILogger<BillingOrderWorkflow> logger)
        {
            this.orderHeaderRepository = orderHeaderRepository;
            this.productRepository = productRepository;
            this.orderLineRepository = orderLineRepository;
            this.logger = logger;
        }


        public async Task<IBillingOrderEvent> ExecuteAsync(BillingOrderCommand command)
        {
            // Crează un coș cu comenzile nevalidate pentru facturare
            UnbilledOrdersCart unbilledOrders = new UnbilledOrdersCart(command.InputOrders);

            // Validează comenzile
            var validatedResult = await ValidateOrdersForBilling(unbilledOrders);
            if (validatedResult.IsLeft)
            {
                return new BillingOrderFailedEvent(((FailedBillingCart)validatedResult.LeftAsEnumerable().First()).Reason);

            }

            // Facturează comenzile validate
            var billingResult = await BillOrdersAsync(validatedResult.RightAsEnumerable().First());
            if (billingResult.IsLeft)
            {
                return new BillingOrderFailedEvent(((FailedBillingCart)billingResult.LeftAsEnumerable().First()).Reason);
            }

            // Salvează comenzile facturate
            var saveResult = await SaveBilledOrdersAsync(billingResult.RightAsEnumerable().First());
            if (saveResult.IsLeft)
            {
                return new BillingOrderFailedEvent(((FailedBillingCart)saveResult.LeftAsEnumerable().First()).Reason);
            }

            // Dacă toate etapele au fost de succes, returnează un eveniment de succes
            var billedOrdersCart = saveResult.RightAsEnumerable().First();
            return new BillingOrderSucceededEvent(
                    billedOrdersCart.BilledOrderLines,
                    billedOrdersCart.InvoiceNumber,
                    billedOrdersCart.AmountBilled,
                    DateTime.Now
            );
        }


        //private async Task<Either<FailedBillingCart, BilledOrdersCart>> SaveBilledOrdersAsync(BilledOrdersCart

        // Metodele folosite în procesul de facturare
        private async Task<Either<IBillingCart, ValidatedOrdersCart>> ValidateOrdersForBilling(UnbilledOrdersCart cart)
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
                return Left(new FailedBillingCart("Validation failed") as IBillingCart);
            }
        }


            private static async Task<Either<IBillingCart, BilledOrdersCart>> BillOrdersAsync(ValidatedOrdersCart validatedOrders)
            {
                try
                {
                    var billedOrders = new List<BilledOrderLine>();
                    decimal totalAmountBilled = 0;

                    foreach (var validatedOrder in validatedOrders.OrdersList)
                    {

                        var productId = validatedOrder.OrderRegistrationCode; // ID-ul produsului
                        var productName = validatedOrder.OrderDescription; // Numele produsului
                        var quantity = validatedOrder.OrderAmount; // Cantitatea produsului în comandă
                        var unitPrice = validatedOrder.OrderPrice; // Prețul unitar al produsului
                        var adress = validatedOrder.OrderAddress;

                        var lineTotal = (decimal)(quantity.Amount * unitPrice.Price);
                        var billedLine = new BilledOrderLine(productId, productName, quantity, adress, unitPrice);
                        billedOrders.Add(billedLine);
                        totalAmountBilled += lineTotal; // Adunăm totalul liniei la suma totală facturată
                    }

                    // Creăm un coș cu comenzile facturate
                    var billingDate = DateTime.UtcNow; // Data și ora actuală UTC
                    var invoiceNumber = "Invoice" + billingDate.Ticks.ToString(); // Un număr unic de factură

                    var billedOrdersCart = new BilledOrdersCart(billedOrders, invoiceNumber, totalAmountBilled, billingDate);
                    return Right(billedOrdersCart);
                }
                catch (Exception ex)
                {
                    // În caz de eroare, returnăm un coș de facturare eșuat
                    return Left(new FailedBillingCart("Billing failed: " + ex.Message) as IBillingCart);


                }


            }
        private async Task<Either<IBillingCart, BilledOrdersCart>> SaveBilledOrdersAsync(BilledOrdersCart billedOrdersCart)
        {
            try
            {
                // Salvarea comenzilor facturate ar putea implica actualizarea unui header de comandă
                // și/sau inserarea liniilor de comandă facturate într-o tabelă corespunzătoare.
                //await orderHeaderRepository.SaveBilledOrderHeaderAsync(billedOrdersCart);
                //await orderLineRepository.SaveBilledOrderLinesAsync(billedOrdersCart.BilledOrderLines);

                // După ce datele au fost salvate cu succes, returnăm coșul de comenzi facturate.
                return Right(billedOrdersCart);
            }
            catch (Exception ex)
            {
                // Dacă întâmpinăm o eroare în timpul salvării, returnăm o stângă (Left) cu eroarea.
                return Left(new FailedBillingCart($"Failed to save billed orders: {ex.Message}") as IBillingCart);
            }
        }

    }
}
