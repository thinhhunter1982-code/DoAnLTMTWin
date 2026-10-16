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

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;

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

        private void FormThongKe_DoanhThu_Load(object? sender, EventArgs e)
        {
            ThongKeDoanhThu();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
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

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;
            ThongKeDoanhThu();
        }

        private void ThongKeDoanhThu()
        {
            try
            {
                DateTime tuNgay = dateTimePicker1.Value.Date;
                DateTime denNgay = dateTimePicker2.Value.Date.AddDays(1);

                using (var db = new AppDbContext())
                {
                    var hoaDons = db.HoaDons
                        .AsNoTracking()
                        .Where(h =>
                            h.NgayLap >= tuNgay &&
                            h.NgayLap < denNgay &&
                            h.TrangThai == "Hoàn thành");

                    decimal tongDoanhThu = hoaDons
                        .Select(h => (decimal?)h.TongTien)
                        .Sum() ?? 0m;

                    int soHoaDon = hoaDons.Count();

                    var maHoaDonTrongKy = hoaDons
                        .Select(h => h.MaHoaDon);

                    int soDienThoaiBan = db.ChiTietHoaDons
                        .AsNoTracking()
                        .Where(ct => maHoaDonTrongKy.Contains(ct.MaHoaDon))
                        .Select(ct => (int?)ct.SoLuong)
                        .Sum() ?? 0;

                    GanGiaTriLabel(
                        "label4",
                        tongDoanhThu.ToString("N0"));

                    GanGiaTriLabel(
                        "label6",
                        soHoaDon.ToString("N0"));

                    GanGiaTriLabel(
                        "label9",
                        soDienThoaiBan.ToString("N0"));

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

                    var danhSach = danhSachHoaDon.Select(h => new
                    {
                        TenSanPham = string.Join(", ",
                            db.ChiTietHoaDons
                                .AsNoTracking()
                                .Where(ct => ct.MaHoaDon == h.MaHoaDon)
                                .Join(
                                    db.DienThoais.AsNoTracking(),
                                    ct => ct.MaDienThoai,
                                    dt => dt.MaDienThoai,
                                    (ct, dt) => dt.TenDienThoai)
                                .ToList()),

                        NgayLap = h.NgayLap,

                        NguoiMua = db.KhachHangs
                            .AsNoTracking()
                            .Where(k => k.MaKhachHang == h.MaKhachHang)
                            .Select(k => k.TenKhachHang)
                            .FirstOrDefault() ?? "",

                        NhanVien = db.NhanViens
                            .AsNoTracking()
                            .Where(n => n.MaNhanVien == h.MaNhanVien)
                            .Select(n => n.TenNhanVien)
                            .FirstOrDefault() ?? "",

                        TongTien = h.TongTien,
                        TrangThai = h.TrangThai
                    }).ToList();

                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.DataSource = danhSach;

                    DatTieuDeCot("TenSanPham", "Tên sản phẩm");
                    DatTieuDeCot("NgayLap", "Ngày lập");
                    DatTieuDeCot("NguoiMua", "Người mua");
                    DatTieuDeCot("NhanVien", "Nhân viên");
                    DatTieuDeCot("TongTien", "Tổng tiền");
                    DatTieuDeCot("TrangThai", "Trạng thái");

                    if (dataGridView1.Columns["NgayLap"] != null)
                    {
                        dataGridView1.Columns["NgayLap"]
                            .DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }

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
                    "Không thể thống kê doanh thu.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DatTieuDeCot(string tenCot, string tieuDe)
        {
            if (dataGridView1.Columns[tenCot] != null)
            {
                dataGridView1.Columns[tenCot].HeaderText = tieuDe;
            }
        }

        private void GanGiaTriLabel(string tenLabel, string giaTri)
        {
            Control[] controls = Controls.Find(tenLabel, true);

            if (controls.Length > 0 && controls[0] is Label lbl)
            {
                lbl.Text = giaTri;
            }
        }
    }
}