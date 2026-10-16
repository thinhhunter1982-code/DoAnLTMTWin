namespace DoAnLTMTWin
{
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}