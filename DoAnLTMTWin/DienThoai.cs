namespace DoAnLTMTWin
{
    public class DienThoai
    {
        public int MaDienThoai { get; set; }
        public string TenDienThoai { get; set; } = string.Empty;
        public string? HangSanXuat { get; set; }
        public string? MauSac { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public int MaLoai { get; set; }
        public int ThoiGianBaoHanh { get; set; }

        public LoaiDienThoai? LoaiDienThoai { get; set; }
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
        public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}