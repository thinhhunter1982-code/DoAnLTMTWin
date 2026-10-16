using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    internal class AppDbContext : DbContext
    {
        public DbSet<DienThoai> DienThoais { get; set; } = null!;
        public DbSet<LoaiDienThoai> LoaiDienThoais { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<NhanVien> NhanViens { get; set; } = null!;
        public DbSet<NhaCungCap> NhaCungCaps { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; } = null!;
        public DbSet<PhieuNhap> PhieuNhaps { get; set; } = null!;
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = null!;
        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=LAPTOP-6A06S927\MSSQLSERVER02;Database=QuanLyCuaHangDienThoai;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DienThoai>().HasKey(x => x.MaDienThoai);
            modelBuilder.Entity<LoaiDienThoai>().HasKey(x => x.MaLoai);
            modelBuilder.Entity<KhachHang>().HasKey(x => x.MaKhachHang);
            modelBuilder.Entity<NhanVien>().HasKey(x => x.MaNhanVien);
            modelBuilder.Entity<NhaCungCap>().HasKey(x => x.MaNhaCungCap);
            modelBuilder.Entity<HoaDon>().HasKey(x => x.MaHoaDon);
            modelBuilder.Entity<ChiTietHoaDon>().HasKey(x => x.MaChiTietHoaDon);
            modelBuilder.Entity<PhieuNhap>().HasKey(x => x.MaPhieuNhap);
            modelBuilder.Entity<ChiTietPhieuNhap>().HasKey(x => x.MaChiTietPhieuNhap);

            modelBuilder.Entity<TaiKhoan>().HasKey(x => x.MaTaiKhoan);

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.SoDienThoai)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.TenDangNhap)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.MatKhau)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.HoTen)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.SoDienThoai)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.DiaChi)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<TaiKhoan>()
                .Property(x => x.VaiTro)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<DienThoai>().Property(x => x.GiaBan).HasPrecision(18, 2);
            modelBuilder.Entity<HoaDon>().Property(x => x.TongTien).HasPrecision(18, 2);
            modelBuilder.Entity<ChiTietHoaDon>().Property(x => x.DonGia).HasPrecision(18, 2);
            modelBuilder.Entity<ChiTietHoaDon>().Property(x => x.ThanhTien).HasPrecision(18, 2);
            modelBuilder.Entity<PhieuNhap>().Property(x => x.TongTien).HasPrecision(18, 2);
            modelBuilder.Entity<ChiTietPhieuNhap>().Property(x => x.DonGiaNhap).HasPrecision(18, 2);
            modelBuilder.Entity<ChiTietPhieuNhap>().Property(x => x.ThanhTien).HasPrecision(18, 2);

            modelBuilder.Entity<DienThoai>()
                .HasOne(x => x.LoaiDienThoai)
                .WithMany(x => x.DienThoais)
                .HasForeignKey(x => x.MaLoai)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.HoaDon)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.MaHoaDon)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.DienThoai)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.MaDienThoai)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuNhap>()
                .HasOne(x => x.NhaCungCap)
                .WithMany(x => x.PhieuNhaps)
                .HasForeignKey(x => x.MaNhaCungCap)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuNhap>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.PhieuNhaps)
                .HasForeignKey(x => x.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietPhieuNhap>()
                .HasOne(x => x.PhieuNhap)
                .WithMany(x => x.ChiTietPhieuNhaps)
                .HasForeignKey(x => x.MaPhieuNhap)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietPhieuNhap>()
                .HasOne(x => x.DienThoai)
                .WithMany(x => x.ChiTietPhieuNhaps)
                .HasForeignKey(x => x.MaDienThoai)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}