namespace BillingAPI.Interfaces
{
    public interface IOrder
    {
        string OrderNumber { get; set; }
        string Product { get; set; }
        string UserId { get; set; }
        decimal? FullPrice { get; set; }
        decimal? PaidAmount { get; set; }
    }
}
