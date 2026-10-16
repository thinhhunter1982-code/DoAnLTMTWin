namespace DoAnLTMTWin
{
    public class NhaCungCap
{
    public int MaNhaCungCap { get; set; }
    public string TenNhaCungCap { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? Email { get; set; }

    public ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
}
}