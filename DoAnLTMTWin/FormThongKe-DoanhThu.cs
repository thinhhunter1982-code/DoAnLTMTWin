
using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormThongKe_DoanhThu : Form
    {
        public FormThongKe_DoanhThu()
        {
            InitializeComponent();

            // Thiết lập ngày mặc định
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;

            // Thiết lập DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.Columns.Clear();

            Load += FormThongKe_DoanhThu_Load;
        }

        // ==============================
        // FORM LOAD
        // ==============================
        private void FormThongKe_DoanhThu_Load(
            object? sender, EventArgs e)
        {
            ThongKeDoanhThu();
        }

        // ==============================
        // NÚT THỐNG KÊ
        // ==============================
        private void btnThongKe_Click(
            object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date >
                dateTimePicker2.Value.Date)
            {
                MessageBox.Show(
                    "Ngày bắt đầu không được lớn hơn ngày kết thúc!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ThongKeDoanhThu();
        }

        // ==============================
        // NÚT LÀM MỚI
        // ==============================
        private void btnLamMoi_Click(
            object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;

            ThongKeDoanhThu();
        }

        // ==============================
        // THỐNG KÊ DOANH THU
        // ==============================
        private void ThongKeDoanhThu()
        {
            try
            {
                DateTime tuNgay =
                    dateTimePicker1.Value.Date;

                DateTime denNgay =
                    dateTimePicker2.Value.Date.AddDays(1);

                using (var db = new AppDbContext())
                {
                    // Chỉ lấy hóa đơn đã thanh toán
                    var hoaDons = db.HoaDons
                        .AsNoTracking()
                        .Where(h =>
                            h.NgayLap >= tuNgay &&
                            h.NgayLap < denNgay &&
                            h.TrangThai == "Đã thanh toán");

                    // 1. TỔNG DOANH THU
                    decimal tongDoanhThu = hoaDons
                        .Select(h => (decimal?)h.TongTien)
                        .Sum() ?? 0m;

                    // 2. SỐ HÓA ĐƠN
                    int soHoaDon = hoaDons.Count();

                    // 3. SỐ ĐIỆN THOẠI ĐÃ BÁN
                    var maHoaDonTrongKy = hoaDons
                        .Select(h => h.MaHoaDon);

                    int soDienThoaiBan =
                        db.ChiTietHoaDons
                        .AsNoTracking()
                        .Where(ct =>
                            maHoaDonTrongKy.Contains(ct.MaHoaDon))
                        .Select(ct => (int?)ct.SoLuong)
                        .Sum() ?? 0;

                    // CẬP NHẬT CÁC LABEL
                    GanGiaTriLabel(
                        "label4",
                        tongDoanhThu.ToString("N0"));

                    GanGiaTriLabel(
                        "label6",
                        soHoaDon.ToString("N0"));

                    GanGiaTriLabel(
                        "label9",
                        soDienThoaiBan.ToString("N0"));

                    // 4. LẤY DANH SÁCH HÓA ĐƠN
                    var danhSachHoaDon = hoaDons
                        .OrderByDescending(h => h.NgayLap)
                        .Select(h => new
                        {
                            h.MaHoaDon,
                            h.NgayLap,
                            h.MaKhachHang,
                            h.MaNhanVien,
                            h.TongTien,
                            h.TrangThai
                        })
                        .ToList();

                    // 5. LẤY THÔNG TIN CHI TIẾT HÓA ĐƠN
                    var danhSach = danhSachHoaDon
                        .Select(h => new
                        {
                            TenSanPham = string.Join(", ",
                                db.ChiTietHoaDons
                                    .AsNoTracking()
                                    .Where(ct =>
                                        ct.MaHoaDon == h.MaHoaDon)
                                    .Join(
                                        db.DienThoais.AsNoTracking(),
                                        ct => ct.MaDienThoai,
                                        dt => dt.MaDienThoai,
                                        (ct, dt) => dt.TenDienThoai)
                                    .ToList()),

                            NgayLap = h.NgayLap,

                            NguoiMua = db.KhachHangs
                                .AsNoTracking()
                                .Where(k =>
                                    k.MaKhachHang == h.MaKhachHang)
                                .Select(k => k.TenKhachHang)
                                .FirstOrDefault() ?? "",

                            NhanVien = db.NhanViens
                                .AsNoTracking()
                                .Where(n =>
                                    n.MaNhanVien == h.MaNhanVien)
                                .Select(n => n.TenNhanVien)
                                .FirstOrDefault() ?? "",

                            TongTien = h.TongTien,

                            TrangThai = h.TrangThai
                        })
                        .ToList();

                    // 6. HIỂN THỊ DATAGRIDVIEW
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.DataSource = danhSach;

                    // Đặt tiêu đề cột
                    DatTieuDeCot(
                        "TenSanPham", "Tên sản phẩm");

                    DatTieuDeCot(
                        "NgayLap", "Ngày lập");

                    DatTieuDeCot(
                        "NguoiMua", "Người mua");

                    DatTieuDeCot(
                        "NhanVien", "Nhân viên");

                    DatTieuDeCot(
                        "TongTien", "Tổng tiền");

                    DatTieuDeCot(
                        "TrangThai", "Trạng thái");

                    // Định dạng ngày lập
                    if (dataGridView1.Columns["NgayLap"] != null)
                    {
                        dataGridView1.Columns["NgayLap"]
                            .DefaultCellStyle.Format =
                            "dd/MM/yyyy HH:mm";
                    }

                    // Định dạng tiền
                    if (dataGridView1.Columns["TongTien"] != null)
                    {
                        dataGridView1.Columns["TongTien"]
                            .DefaultCellStyle.Format = "N0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thống kê doanh thu.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi thống kê",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==============================
        // ĐẶT TIÊU ĐỀ CỘT
        // ==============================
        private void DatTieuDeCot(
            string tenCot, string tieuDe)
        {
            if (dataGridView1.Columns[tenCot] != null)
            {
                dataGridView1.Columns[tenCot].HeaderText =
                    tieuDe;
            }
        }

        // ==============================
        // CẬP NHẬT LABEL
        // ==============================
        private void GanGiaTriLabel(
            string tenLabel, string giaTri)
        {
            Control[] controls =
                Controls.Find(tenLabel, true);

            if (controls.Length > 0 &&
                controls[0] is Label lbl)
            {
                lbl.Text = giaTri;
            }
        }
    }
}
