namespace DoAnLTMTWin
{
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "Hoàn thành";
        public int MaKhachHang { get; set; }
        public int MaNhanVien { get; set; }

        public KhachHang? KhachHang { get; set; }
        public NhanVien? NhanVien { get; set; }
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}