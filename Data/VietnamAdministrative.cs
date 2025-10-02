using System.Collections.Generic;

namespace BanhMyIT.Data
{
    public static class VietnamAdministrative
    {
        public static IReadOnlyList<ProvinceInfo> Provinces { get; } = new List<ProvinceInfo>
        {
            new ProvinceInfo
            {
                Name = "Hà Nội",
                Districts = new List<string>
                {
                    "Quận Ba Đình","Quận Hoàn Kiếm","Quận Tây Hồ","Quận Long Biên","Quận Cầu Giấy","Quận Đống Đa","Quận Hai Bà Trưng","Quận Hoàng Mai","Quận Thanh Xuân","Huyện Sóc Sơn","Huyện Đông Anh","Huyện Gia Lâm","Quận Nam Từ Liêm","Huyện Thanh Trì","Quận Bắc Từ Liêm","Huyện Mê Linh","Quận Hà Đông","Thị xã Sơn Tây","Huyện Ba Vì","Huyện Phúc Thọ","Huyện Đan Phượng","Huyện Hoài Đức","Huyện Quốc Oai","Huyện Thạch Thất","Huyện Chương Mỹ","Huyện Thanh Oai","Huyện Thường Tín","Huyện Phú Xuyên","Huyện Ứng Hòa","Huyện Mỹ Đức"
                }
            },
            new ProvinceInfo
            {
                Name = "TP Hồ Chí Minh",
                Districts = new List<string>
                {
                    "Quận 1","Quận 3","Quận 4","Quận 5","Quận 6","Quận 7","Quận 8","Quận 10","Quận 11","Quận 12","Quận Bình Thạnh","Quận Gò Vấp","Quận Phú Nhuận","Quận Tân Bình","Quận Tân Phú","Quận Bình Tân","Thành phố Thủ Đức","Huyện Bình Chánh","Huyện Cần Giờ","Huyện Củ Chi","Huyện Hóc Môn","Huyện Nhà Bè"
                }
            },
            new ProvinceInfo
            {
                Name = "Đà Nẵng",
                Districts = new List<string>
                {
                    "Quận Hải Châu","Quận Thanh Khê","Quận Sơn Trà","Quận Ngũ Hành Sơn","Quận Liên Chiểu","Quận Cẩm Lệ","Huyện Hòa Vang","Huyện Hoàng Sa"
                }
            },
            new ProvinceInfo
            {
                Name = "Hải Phòng",
                Districts = new List<string>
                {
                    "Quận Hồng Bàng","Quận Ngô Quyền","Quận Lê Chân","Quận Hải An","Quận Kiến An","Quận Đồ Sơn","Quận Dương Kinh","Huyện Thuỷ Nguyên","Huyện An Dương","Huyện An Lão","Huyện Kiến Thuỵ","Huyện Tiên Lãng","Huyện Vĩnh Bảo","Huyện Cát Hải","Huyện Bạch Long Vĩ"
                }
            },
            new ProvinceInfo
            {
                Name = "Cần Thơ",
                Districts = new List<string>
                {
                    "Quận Ninh Kiều","Quận Ô Môn","Quận Bình Thuỷ","Quận Cái Răng","Quận Thốt Nốt","Huyện Vĩnh Thạnh","Huyện Cờ Đỏ","Huyện Phong Điền","Huyện Thới Lai"
                }
            }
            // TODO: Add remaining provinces for full coverage.
        };

        public static IReadOnlyList<string> GetProvinces() => (IReadOnlyList<string>)System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(Provinces,p=>p.Name));
        public static IReadOnlyList<string> GetDistricts(string province)
        {
            var p = System.Linq.Enumerable.FirstOrDefault(Provinces, x => x.Name == province);
            return p?.Districts ?? new List<string>();
        }
    }

    public class ProvinceInfo
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Districts { get; set; } = new();
    }
}

