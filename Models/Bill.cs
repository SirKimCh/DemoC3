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
        public int UserID { get; set; }
        public PayMethod? PayMethod { get; set; }
        [Range(0,int.MaxValue, ErrorMessage = "TotalPrice must be >= 0")] 
        public int TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ValidateNever] public User User { get; set; } = null!; // EF populated
        [ValidateNever] public ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}
