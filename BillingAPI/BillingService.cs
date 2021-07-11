using BillingAPI.Exceptions;
using BillingAPI.Interfaces;
using BillingAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace BillingAPI
{
    public class BillingService : IBillingService
    {
        private readonly OrdersContext context;
        private readonly IConfiguration Configuration;
        private static Random rnd = new Random();

        public BillingService(OrdersContext context, IConfiguration configuration)
        {
            this.context = context;
            Configuration = configuration;
        }

        public IQueryable<Order> ListAllOrders() => context.Orders;

        public IQueryable<Order> ListUserOrders(string user) => string.IsNullOrEmpty(user) ? null : context.Orders.Where(o => o.UserId.Equals(user, StringComparison.InvariantCultureIgnoreCase));

        public Order GetOrder(string orderNr) => string.IsNullOrEmpty(orderNr) ? null : context.Orders.FirstOrDefault(o => o.OrderNumber.Equals(orderNr, StringComparison.InvariantCultureIgnoreCase));

        private int GatewayFailureRnd => rnd.Next(0, 100);

        private GatewayResponse ValidateGateway(string rq) => rq == "validRQ" ? GatewayResponse.Success : GatewayResponse.Failure;

        public Receipt MakePayment(Payment payment)
        {
            if (payment == null ||
                payment.PaymentAmount == null ||
                string.IsNullOrEmpty(payment.OrderNumber) ||
                string.IsNullOrEmpty(payment.UserId) ||
                string.IsNullOrEmpty(payment.Gateway.ToString()))
            {
                throw new InvalidPaymentDataException();
            }

            var order = context.Orders.FirstOrDefault(o => o.OrderNumber.Equals(payment.OrderNumber, StringComparison.InvariantCultureIgnoreCase)
                                                            && o.UserId.Equals(payment.UserId, StringComparison.InvariantCultureIgnoreCase));

            if (order == null)
                throw new OrderNotFoundException();

            int fChance = -1;
            int.TryParse(Configuration["GatewayFailureChance"], out fChance);

            if (ValidateGateway(GatewayFailureRnd > fChance ? "validRQ" : "invalidRQ") != GatewayResponse.Success)
                throw new GatewayFailureException();

            UpdateOrder(order, payment.PaymentAmount);
                        
            return new Receipt()
            {
                OrderNumber = payment.OrderNumber,
                Gateway = payment.Gateway,
                PaidAmount = payment.PaymentAmount,
                UnpaidAmount = order.FullPrice - order.PaidAmount < 0 ? 0 : order.FullPrice - order.PaidAmount
            };
        }

        private void UpdateOrder(Order order, decimal? payment)
        {
            if (order != null && payment != null)
            {
                order.PaidAmount += payment;
            }
            context.SaveChanges();
        }

    }
}