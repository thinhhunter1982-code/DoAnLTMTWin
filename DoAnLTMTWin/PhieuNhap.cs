namespace DoAnLTMTWin
{
    public class PhieuNhap
    {
        public int MaPhieuNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public int MaNhaCungCap { get; set; }
        public int MaNhanVien { get; set; }

        public NhaCungCap? NhaCungCap { get; set; }
        public NhanVien? NhanVien { get; set; }
        public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}