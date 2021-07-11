using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BillingAPI.Models;
using BillingAPI.Interfaces;
using BillingAPI.Exceptions;

namespace BillingAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IBillingService _billingService;

        public OrdersController(OrdersContext context, ILogger<OrdersController> logger, IBillingService billingService)
        {
            _context = context;
            _logger = logger;
            _billingService = billingService;

            if (_context.Orders.Any()) return;

            OrdersData.Init(context);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<Order>> ListAllOrders([FromQuery] ProductPaging request)
        {
            var result = _billingService.ListAllOrders();

            Response.Headers["x-total-count"] = result.Count().ToString();
            return Ok(result
              .OrderBy(o => o.OrderNumber)
              .Skip(request.StartAt)
              .Take(request.MaxResults));
        }

        [HttpGet]
        [Route("ListOrders/{user}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IQueryable<Order>> ListUserOrders([FromRoute] string user, [FromQuery] ProductPaging request)
        {
            try
            {
                var orders = _billingService.ListUserOrders(user);

                if (orders == null) return NotFound();

                return Ok(orders
                  .OrderBy(o => o.OrderNumber)
                  .Skip(request.StartAt)
                  .Take(request.MaxResults));
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "ListOrders encountered an error");
                return ValidationProblem(e.Message);
            }
        }

        [HttpGet]
        [Route("GetOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Order> GetOrder([FromQuery] OrderRequest rq)
        {
            try
            {
                var order = _billingService.GetOrder(rq.OrderNumber);

                if (order == null) return NotFound();

                return Ok(order);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "GetOrder encountered an error");
                return ValidationProblem(e.Message);
            }
        }


        [HttpPost]
        [Route("Payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Receipt> MakePayment([FromBody] Payment payment)
        {
            try
            {
                var receipt = _billingService.MakePayment(payment);

                return Ok(receipt);
            }
            catch (InvalidPaymentDataException e)
            {
                _logger.LogWarning(e, "Invalid payment data error");
                return BadRequest(e.Message);
            }
            catch (OrderNotFoundException e)
            {
                _logger.LogWarning(e, "Order was not found");
                return NotFound(e.Message);
            }
            catch (GatewayFailureException e)
            {
                _logger.LogWarning(e, "Payment gateway returned an error");
                return ValidationProblem(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "MakePayment encountered an error");
                return ValidationProblem(e.Message);
            }
        }

    }
}
