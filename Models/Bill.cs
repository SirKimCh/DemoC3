using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

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
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public PayMethod? PayMethod { get; set; }
        public int SumPrice { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
