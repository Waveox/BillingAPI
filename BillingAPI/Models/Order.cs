using BillingAPI.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BillingAPI.Models
{
    public class Order : IOrder
    {
        [Key]
        [Required]
        [Display(Name = "orderNumber")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "product")]
        public string Product { get; set; }

        [Required]
        [Display(Name = "userId")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "fullPrice")]
        public decimal? FullPrice { get; set; }

        [Required]
        [Display(Name = "paidAmount")]
        public decimal? PaidAmount { get; set; }
    }
}