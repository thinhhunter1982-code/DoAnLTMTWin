namespace DoAnLTMTWin
{
    public class LoaiDienThoai
    {
        public int MaLoai { get; set; }
        public string TenLoai { get; set; } = string.Empty;
        public string? MoTa { get; set; }

        public ICollection<DienThoai> DienThoais { get; set; } = new List<DienThoai>();
    }
}