using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BanhMyIT.Models
{
    public class Province
    {
        public int ProvinceId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<District> Districts { get; set; } = new List<District>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
