
namespace DoAnLTMTWin
{
    public class ChiTietPhieuNhap
    {
        public int MaChiTietPhieuNhap { get; set; }
        public int MaPhieuNhap { get; set; }
        public int MaDienThoai { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien { get; set; }

        public PhieuNhap? PhieuNhap { get; set; }
        public DienThoai? DienThoai { get; set; }
    }
}
