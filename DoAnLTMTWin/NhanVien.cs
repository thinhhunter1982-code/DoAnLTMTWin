namespace DoAnLTMTWin
{
    public class NhanVien
    {
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public string? GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? ChucVu { get; set; }

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
    }
}