using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BanhMyIT.Models
{
    public enum PayMethod
    {
        Cash, CreditCard, DebitCard, MobilePayment
    }

    // New: Order status managed by enum
    public enum OrderStatus
    {
        Pending = 0,        // User created, awaiting staff/admin confirmation
        Confirmed = 1,      // Confirmed by staff/admin
        Shipped = 2,        // Marked as shipped by staff/admin
        Completed = 3,      // User confirmed received
        CancelRequested = 4,// User requested cancel (before shipped)
        Cancelled = 5       // Cancelled by admin/staff (or approved cancel)
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

        // New: workflow & soft delete
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool IsDeleted { get; set; } = false;
        [StringLength(500)] public string? CancelReason { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        [ValidateNever] public User User { get; set; } = null!; // EF populated
        [ValidateNever] public ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    }
}
