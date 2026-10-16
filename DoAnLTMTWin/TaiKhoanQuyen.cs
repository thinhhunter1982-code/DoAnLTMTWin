namespace DoAnLTMTWin
{
    public class TaiKhoanQuyen
    {
        public int TaiKhoanQuyenId { get; set; }

        public int MaTaiKhoan { get; set; }

        public string ChucNang { get; set; } = string.Empty;

        public bool DuocTruyCap { get; set; }

        public bool DuocSua { get; set; }
    }
}