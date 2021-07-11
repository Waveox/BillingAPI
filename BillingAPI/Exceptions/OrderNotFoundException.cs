using System;

namespace BillingAPI.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException()
        {
        }

        public OrderNotFoundException(string message)
            : base(message)
        {
        }
    }
}
