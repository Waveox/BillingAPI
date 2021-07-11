using BillingAPI.Models;

namespace BillingAPI.Interfaces
{
    public interface IReceipt
    {
        string OrderNumber { get; set; }
        Gateway Gateway { get; set; }
        decimal? PaidAmount { get; set; }
        decimal? UnpaidAmount { get; set; }
    }
}
