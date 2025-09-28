using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BanhMyIT.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string FirstMidName { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;
        [Range(0,int.MaxValue)]
        public int City { get; set; }
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
