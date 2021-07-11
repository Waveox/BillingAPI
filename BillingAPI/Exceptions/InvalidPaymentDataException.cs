using System;

namespace BillingAPI.Exceptions
{
    public class InvalidPaymentDataException : Exception
    {
        public InvalidPaymentDataException()
        {
        }

        public InvalidPaymentDataException(string message)
            : base(message)
        {
        }
    }
}
