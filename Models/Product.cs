using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanhMyIT.Models
{
    public class Product
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [Range(0, int.MaxValue, ErrorMessage = "Price must be >= 0")] 
        public int Price { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public Category Category { get; set; } = null!; 
    }
}
