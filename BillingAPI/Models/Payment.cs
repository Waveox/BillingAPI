using BillingAPI.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BillingAPI.Models
{
    public class Payment : IPayment
    {
        [Key]
        [Required]
        [Display(Name = "orderNumber")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "userId")]
        public string UserId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "paymentAmount")]
        public decimal? PaymentAmount { get; set; }

        [Required]
        [Display(Name = "gateway")]
        public Gateway Gateway { get; set; }

        [Required]
        [Display(Name = "description")]
        public string Description { get; set; }
    }
}