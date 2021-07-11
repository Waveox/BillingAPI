using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Models
{
    public class ProductPaging
    {
        [FromQuery(Name = "MaxResults")]
        public int MaxResults { get; set; } = 50;

        [FromQuery(Name = "StartAt")]
        public int StartAt { get; set; }
    }
}
