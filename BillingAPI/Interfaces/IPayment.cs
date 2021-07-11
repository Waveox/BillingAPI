using BillingAPI.Models;

namespace BillingAPI.Interfaces
{
    public interface IPayment
    {
        string OrderNumber { get; set; }
        string UserId { get; set; }
        decimal? PaymentAmount { get; set; }
        Gateway Gateway { get; set; }
        string Description { get; set; }
    }
}
