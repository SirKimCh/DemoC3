using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanhMyIT.Models
{
    public class District
    {
        public int DistrictId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [ForeignKey(nameof(Province))]
        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
    }
}

