using System;

namespace BillingAPI.Exceptions
{
    public class GatewayFailureException : Exception
    {
        public GatewayFailureException()
        {
        }

        public GatewayFailureException(string message)
            : base(message)
        {
        }
    }
}
