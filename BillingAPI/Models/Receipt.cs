using BillingAPI.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BillingAPI.Models
{
    public class Receipt : IReceipt
    {
        [Key]
        [Required]
        [Display(Name = "orderNumber")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "gateway")]
        public Gateway Gateway { get; set; }

        [Required]
        [Display(Name = "paidAmount")]
        public decimal? PaidAmount { get; set; }

        [Required]
        [Display(Name = "paidAmount")]
        public decimal? UnpaidAmount { get; set; }
    }
}