using System.ComponentModel.DataAnnotations;

namespace BanhMyIT.ViewModels
{
    public class UserAddressViewModel
    {
        public int UserID { get; set; }
        [Required, StringLength(100)]
        [Display(Name = "First / Middle Name")]
        public string FirstMidName { get; set; } = string.Empty;
        [Required, StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(200)]
        [Display(Name = "Địa chỉ chi tiết (Số nhà / Đường)")]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [Range(1,int.MaxValue)]
        [Display(Name = "Tỉnh / Thành phố")]
        public int ProvinceId { get; set; }

        [Required]
        [Range(1,int.MaxValue)]
        [Display(Name = "Quận / Huyện")]
        public int DistrictId { get; set; }

        // For display (names loaded from DB)
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }

        public string FullAddress => string.Join(", ", new[] { StreetAddress, DistrictName, ProvinceName }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
