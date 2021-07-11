using BillingAPI.Models;
using System.Linq;

namespace BillingAPI.Interfaces
{
    public interface IBillingService
    {
        public IQueryable<Order> ListAllOrders();
        public IQueryable<Order> ListUserOrders(string user);
        public Order GetOrder(string orderNr);
        public Receipt MakePayment(Payment payment);
    }
}
