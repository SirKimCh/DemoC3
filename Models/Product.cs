using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // added

namespace BanhMyIT.Models
{
    public class Product
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Select a category")] // changed from Required to Range
        public int CategoryID { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [Range(0, int.MaxValue, ErrorMessage = "Price must be >= 0")] 
        public int Price { get; set; }
        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [ValidateNever] 
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        [ValidateNever] 
        public Category Category { get; set; } = null!; 
    }
}
