using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BillingAPI.Models
{
    public class OrderRequest
    {
        [Required]
        [FromQuery(Name = "orderNumber")]
        public string OrderNumber { get; set; }
    }
}
