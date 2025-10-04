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
        // Full address concatenated (Street + District + Province) - preserved for backward compatibility
        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;
        // New structured fields
        [Required, StringLength(200)]
        public string StreetAddress { get; set; } = string.Empty;
        [Required]
        [Range(1,int.MaxValue)]
        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int DistrictId { get; set; }
        public District? District { get; set; }
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
