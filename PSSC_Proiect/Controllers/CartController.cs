using Domain;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSSC_Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Domain.Models.PlacingOrderEvent;

namespace PSSC_Proiect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            this._logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrades([FromServices] IOrderLineRepository orderLineRepository) =>
            await orderLineRepository.TryGetExistingOrders().Match(
               Succ: GetAllOrdersHandleSuccess,
               Fail: GetAllOrdersHandleError
            );
        private ObjectResult GetAllOrdersHandleError(Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }
        private OkObjectResult GetAllOrdersHandleSuccess(List<CalculateCustomerOrder> orders) =>
        Ok(orders.Select(order => new
        {
            OrderRegistrationCode = order.OrderRegistrationCode.Value,
            order.OrderDescription,
            order.OrderAmount,
            order.OrderAddress,
            order.OrderPrice,
            order.FinalPrice

        }));

        [HttpPost]
        public async Task<IActionResult> PlaceOrders([FromServices] PlacingOrderWorkflow placingOrderWorkflow, [FromBody] InputOrder[] orders)
        {
            var unvalidatedOrders = orders.Select(MapInputOrderToUnvalidatedOrder)
                                          .ToList()
                                          .AsReadOnly();
            PlacingOrdersCommand command = new(unvalidatedOrders);
            var result = await placingOrderWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenPlacingOrderFailedEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenPlacingOrderSuccedeedEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedCustomerOrder MapInputOrderToUnvalidatedOrder(InputOrder order) => new UnvalidatedCustomerOrder(
            OrderRegistrationCode: order.RegistrationCode,
            OrderDescription: order.Description,
            OrderAmount: order.Amount,
            OrderAddress: order.Address,
            OrderPrice: order.Price
            );
    }
}
