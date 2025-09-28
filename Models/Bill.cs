using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BanhMyIT.Models
{
    public enum PayMethod
    {
        Cash, CreditCard, DebitCard, MobilePayment
    }

    public class Bill
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillID { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public int UserID { get; set; }
        public PayMethod? PayMethod { get; set; }
        [Range(0,int.MaxValue, ErrorMessage = "SumPrice must be >= 0")] 
        public int SumPrice { get; set; }

        [ValidateNever] public User User { get; set; } = null!; // EF populated
        [ValidateNever] public Product Product { get; set; } = null!; // EF populated
    }
}
