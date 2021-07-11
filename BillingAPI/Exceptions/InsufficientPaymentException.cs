using System;

namespace BillingAPI.Exceptions
{
    public class InsufficientPaymentException : Exception
    {
        public InsufficientPaymentException()
        {
        }

        public InsufficientPaymentException(string message)
            : base(message)
        {
        }
    }
}
