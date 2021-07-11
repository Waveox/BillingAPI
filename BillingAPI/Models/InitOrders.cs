namespace BillingAPI.Models
{
    public static class OrdersData
    {
        public static void Init(OrdersContext context)
        {
            context.Orders.AddRange(
                new Order()
                {
                    OrderNumber = "ON00001",
                    UserId = "1",
                    Product = "Product1",
                    FullPrice = 250m,
                    PaidAmount = 0m
                },
                new Order()
                {
                    OrderNumber = "ON00002",
                    UserId = "2",
                    Product = "Product2",
                    FullPrice = 1500m,
                    PaidAmount = 0m
                }, 
                new Order()
                {
                    OrderNumber = "ON00003",
                    UserId = "3",
                    Product = "Product3",
                    FullPrice = 0.7m,
                    PaidAmount = 0m
                }, 
                new Order()
                {
                    OrderNumber = "ON00004",
                    UserId = "2",
                    Product = "Product4",
                    FullPrice = 500m,
                    PaidAmount = 250m
                }
            );

            context.SaveChanges();
        }
    }
}
