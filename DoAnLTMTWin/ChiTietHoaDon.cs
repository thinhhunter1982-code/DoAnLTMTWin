
namespace DoAnLTMTWin
{
    public class ChiTietHoaDon
    {
        public int MaChiTietHoaDon { get; set; }
        public int MaHoaDon { get; set; }
        public int MaDienThoai { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        public HoaDon? HoaDon { get; set; }
        public DienThoai? DienThoai { get; set; }
    }
}
