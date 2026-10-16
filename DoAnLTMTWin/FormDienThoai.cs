
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormDienThoai : Form
    {
        private DataGridView? dgv;
        private TextBox? txtTen;
        private TextBox? txtHang;
        private TextBox? txtMau;
        private TextBox? txtGia;
        private TextBox? txtSoLuong;
        private TextBox? txtMaLoai;

        private bool dangNap;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VaiTroNguoiDung { get; set; } = string.Empty;

        private bool LaAdmin => VaiTroNguoiDung == "Admin";

        public FormDienThoai()
        {
            InitializeComponent();
            Load += FormDienThoai_Load;
        }

        private IEnumerable<Control> LayTatCaControl(Control cha)
        {
            foreach (Control c in cha.Controls)
            {
                yield return c;

                foreach (Control con in LayTatCaControl(c))
                    yield return con;
            }
        }

        private void FormDienThoai_Load(object? sender, EventArgs e)
        {
            try
            {
                AnhXaControl();
                CauHinhBang();
                CapNhatQuyen();
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể khởi tạo form điện thoại.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AnhXaControl()
        {
            List<Control> controls = LayTatCaControl(this).ToList();

            dgv = controls.OfType<DataGridView>().FirstOrDefault();

            if (dgv == null)
                throw new InvalidOperationException(
                    "Không tìm thấy DataGridView trên form.");

            GroupBox? khung = controls.OfType<GroupBox>()
                .FirstOrDefault(g => g.Text.Contains(
                    "QUẢN LÝ ĐIỆN THOẠI",
                    StringComparison.OrdinalIgnoreCase));

            if (khung == null)
                throw new InvalidOperationException(
                    "Không tìm thấy khung QUẢN LÝ ĐIỆN THOẠI.");

            List<TextBox> oNhap = LayTatCaControl(khung)
                .OfType<TextBox>()
                .OrderBy(t => t.Top)
                .ThenBy(t => t.Left)
                .ToList();

            if (oNhap.Count < 6)
                throw new InvalidOperationException(
                    "Không tìm thấy đủ 6 TextBox nhập liệu.");

            txtMaLoai = oNhap[0];
            txtTen = oNhap[1];
            txtHang = oNhap[2];
            txtMau = oNhap[3];
            txtGia = oNhap[4];
            txtSoLuong = oNhap[5];

            dgv.SelectionChanged -= dgv_SelectionChanged;
            dgv.SelectionChanged += dgv_SelectionChanged;
        }

        private void CauHinhBang()
        {
            if (dgv == null)
                return;

            dgv.AutoGenerateColumns = true;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
        }

        private void CapNhatQuyen()
        {
            foreach (Control control in LayTatCaControl(this))
            {
                if (control is Button nut)
                {
                    string ten = nut.Name.ToLowerInvariant();

                    if (ten.Contains("them") ||
                        ten.Contains("sua") ||
                        ten.Contains("xoa"))
                    {
                        nut.Enabled = LaAdmin;
                    }
                }
            }

            if (txtMaLoai != null)
                txtMaLoai.ReadOnly = !LaAdmin;

            if (txtTen != null)
                txtTen.ReadOnly = !LaAdmin;

            if (txtHang != null)
                txtHang.ReadOnly = !LaAdmin;

            if (txtMau != null)
                txtMau.ReadOnly = !LaAdmin;

            if (txtGia != null)
                txtGia.ReadOnly = !LaAdmin;

            if (txtSoLuong != null)
                txtSoLuong.ReadOnly = !LaAdmin;
        }

        private bool KiemTraQuyenAdmin()
        {
            if (LaAdmin)
                return true;

            MessageBox.Show(
                "Tài khoản khách hàng chỉ được xem và tìm kiếm dữ liệu.",
                "Không đủ quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private void TaiDanhSach()
        {
            if (dgv == null)
                return;

            try
            {
                dangNap = true;

                using var db = new AppDbContext();

                dgv.DataSource = db.DienThoais
                    .AsNoTracking()
                    .OrderBy(d => d.MaDienThoai)
                    .Select(d => new
                    {
                        d.MaDienThoai,
                        d.TenDienThoai,
                        d.HangSanXuat,
                        d.MauSac,
                        d.GiaBan,
                        d.SoLuongTon,
                        d.MaLoai
                    })
                    .ToList();

                DatTieuDeCot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu SQL Server.\n\n" + ex.Message,
                    "Lỗi dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                dangNap = false;
            }
        }

        private void DatTieuDeCot()
        {
            DatCot("MaDienThoai", "Mã điện thoại");
            DatCot("TenDienThoai", "Tên điện thoại");
            DatCot("HangSanXuat", "Hãng sản xuất");
            DatCot("MauSac", "Màu sắc");
            DatCot("GiaBan", "Giá bán");
            DatCot("SoLuongTon", "Số lượng tồn");
            DatCot("MaLoai", "Mã loại");

            if (dgv?.Columns["GiaBan"] != null)
                dgv.Columns["GiaBan"]!.DefaultCellStyle.Format = "N0";
        }

        private void DatCot(string tenCot, string tieuDe)
        {
            if (dgv?.Columns[tenCot] != null)
                dgv.Columns[tenCot]!.HeaderText = tieuDe;
        }

        private bool KiemTraControl()
        {
            if (dgv != null &&
                txtTen != null &&
                txtHang != null &&
                txtMau != null &&
                txtGia != null &&
                txtSoLuong != null &&
                txtMaLoai != null)
                return true;

            MessageBox.Show(
                "Không tìm thấy đầy đủ control trên form.",
                "Lỗi giao diện",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        private bool KiemTraDuLieu()
        {
            if (string.IsNullOrWhiteSpace(txtTen!.Text))
            {
                MessageBox.Show("Bạn vui lòng nhập tên điện thoại.");
                txtTen.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHang!.Text))
            {
                MessageBox.Show("Bạn vui lòng nhập hãng sản xuất.");
                txtHang.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGia!.Text.Trim(), out decimal gia)
                || gia < 0)
            {
                MessageBox.Show("Giá bán không hợp lệ.");
                txtGia.Focus();
                return false;
            }

            if (!int.TryParse(txtSoLuong!.Text.Trim(), out int soLuong)
                || soLuong < 0)
            {
                MessageBox.Show("Số lượng tồn không hợp lệ.");
                txtSoLuong.Focus();
                return false;
            }

            if (!int.TryParse(txtMaLoai!.Text.Trim(), out int maLoai)
                || maLoai <= 0)
            {
                MessageBox.Show("Mã loại không hợp lệ.");
                txtMaLoai.Focus();
                return false;
            }

            return true;
        }

        private bool KiemTraMaLoaiVaHang(AppDbContext db, int maLoai)
        {
            string hangNhap = txtHang!.Text.Trim();

            var loai = db.LoaiDienThoais
                .AsNoTracking()
                .FirstOrDefault(l => l.MaLoai == maLoai);

            if (loai == null)
            {
                MessageBox.Show(
                    "Mã loại " + maLoai + " không tồn tại.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtMaLoai!.Focus();
                return false;
            }

            if (!string.Equals(
                hangNhap,
                loai.TenLoai.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Hãng sản xuất không khớp với mã loại!\n\n" +
                    "Mã loại: " + loai.MaLoai + "\n" +
                    "Tên loại tương ứng: " + loai.TenLoai + "\n" +
                    "Hãng bạn nhập: " + hangNhap + "\n\n" +
                    "Bạn hãy nhập hãng sản xuất khớp với tên loại.",
                    "Dữ liệu không khớp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtHang!.Focus();
                return false;
            }

            return true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (!KiemTraControl() || !KiemTraDuLieu())
                return;

            try
            {
                using var db = new AppDbContext();

                int maLoai = int.Parse(txtMaLoai!.Text.Trim());

                if (!KiemTraMaLoaiVaHang(db, maLoai))
                    return;

                if (!db.DienThoais.Any())
                {
                    db.Database.ExecuteSqlRaw(
                        "DBCC CHECKIDENT ('dbo.DienThoais', RESEED, 0)");
                }

                var dt = new DienThoai
                {
                    TenDienThoai = txtTen!.Text.Trim(),
                    HangSanXuat = txtHang!.Text.Trim(),
                    MauSac = GiaTriHoacNull(txtMau!.Text),
                    GiaBan = decimal.Parse(txtGia!.Text.Trim()),
                    SoLuongTon = int.Parse(txtSoLuong!.Text.Trim()),
                    MaLoai = maLoai
                };

                db.DienThoais.Add(dt);
                db.SaveChanges();

                MessageBox.Show(
                    "Thêm điện thoại thành công!\nMã điện thoại: " +
                    dt.MaDienThoai,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrang();
                TaiDanhSach();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Không thể thêm điện thoại.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi thêm điện thoại.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (!KiemTraControl())
                return;

            if (dgv?.CurrentRow == null ||
                dgv.CurrentRow.IsNewRow ||
                dgv.CurrentRow.Cells["MaDienThoai"].Value == null)
            {
                MessageBox.Show("Bạn hãy chọn điện thoại cần sửa.");
                return;
            }

            if (!KiemTraDuLieu())
                return;

            int ma = Convert.ToInt32(
                dgv.CurrentRow.Cells["MaDienThoai"].Value);

            try
            {
                using var db = new AppDbContext();

                var dt = db.DienThoais
                    .FirstOrDefault(d => d.MaDienThoai == ma);

                if (dt == null)
                {
                    MessageBox.Show("Không tìm thấy điện thoại.");
                    TaiDanhSach();
                    return;
                }

                int maLoai = int.Parse(txtMaLoai!.Text.Trim());

                if (!KiemTraMaLoaiVaHang(db, maLoai))
                    return;

                dt.TenDienThoai = txtTen!.Text.Trim();
                dt.HangSanXuat = txtHang!.Text.Trim();
                dt.MauSac = GiaTriHoacNull(txtMau!.Text);
                dt.GiaBan = decimal.Parse(txtGia!.Text.Trim());
                dt.SoLuongTon = int.Parse(txtSoLuong!.Text.Trim());
                dt.MaLoai = maLoai;

                db.SaveChanges();

                MessageBox.Show("Sửa điện thoại thành công.");

                XoaTrang();
                TaiDanhSach();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật điện thoại.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi sửa điện thoại.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (!KiemTraControl())
                return;

            if (dgv?.CurrentRow == null ||
                dgv.CurrentRow.IsNewRow ||
                dgv.CurrentRow.Cells["MaDienThoai"].Value == null)
            {
                MessageBox.Show("Bạn hãy chọn điện thoại cần xóa.");
                return;
            }

            int ma = Convert.ToInt32(
                dgv.CurrentRow.Cells["MaDienThoai"].Value);

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa điện thoại mã " + ma + " không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();

                var dt = db.DienThoais
                    .FirstOrDefault(d => d.MaDienThoai == ma);

                if (dt == null)
                {
                    MessageBox.Show("Điện thoại không tồn tại.");
                    TaiDanhSach();
                    XoaTrang();
                    return;
                }

                bool coHoaDon = db.ChiTietHoaDons
                    .Any(x => x.MaDienThoai == ma);

                bool coPhieuNhap = db.ChiTietPhieuNhaps
                    .Any(x => x.MaDienThoai == ma);

                if (coHoaDon || coPhieuNhap)
                {
                    MessageBox.Show(
                        "Không thể xóa điện thoại đã được sử dụng " +
                        "trong hóa đơn hoặc phiếu nhập.");
                    return;
                }

                db.DienThoais.Remove(dt);
                db.SaveChanges();

                if (!db.DienThoais.Any())
                {
                    db.Database.ExecuteSqlRaw(
                        "DBCC CHECKIDENT ('dbo.DienThoais', RESEED, 0)");
                }

                MessageBox.Show("Xóa điện thoại thành công.");

                XoaTrang();
                TaiDanhSach();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Không thể xóa điện thoại.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi xóa điện thoại.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            if (!KiemTraControl())
                return;

            XoaTrang();
            TaiDanhSach();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (!KiemTraControl())
                return;

            string tenNhap = txtTen!.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenNhap))
            {
                MessageBox.Show("Bạn vui lòng nhập tên điện thoại cần tìm.");
                txtTen.Focus();
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var ketQua = db.DienThoais
                    .AsNoTracking()
                    .Where(d => d.TenDienThoai.Contains(tenNhap))
                    .OrderBy(d => d.MaDienThoai)
                    .Select(d => new
                    {
                        d.MaDienThoai,
                        d.TenDienThoai,
                        d.HangSanXuat,
                        d.MauSac,
                        d.GiaBan,
                        d.SoLuongTon,
                        d.MaLoai
                    })
                    .ToList();

                dangNap = true;
                dgv!.DataSource = ketQua;
                DatTieuDeCot();

                if (ketQua.Count == 0)
                    MessageBox.Show("Không tìm thấy điện thoại phù hợp.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                dangNap = false;
            }
        }

        private void dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dangNap ||
                dgv?.CurrentRow == null ||
                dgv.CurrentRow.IsNewRow)
                return;

            DataGridViewRow row = dgv.CurrentRow;

            if (!dgv.Columns.Contains("MaDienThoai"))
                return;

            txtTen!.Text = DocGiaTri(row, "TenDienThoai");
            txtHang!.Text = DocGiaTri(row, "HangSanXuat");
            txtMau!.Text = DocGiaTri(row, "MauSac");
            txtGia!.Text = DocGiaTri(row, "GiaBan");
            txtSoLuong!.Text = DocGiaTri(row, "SoLuongTon");
            txtMaLoai!.Text = DocGiaTri(row, "MaLoai");
        }

        private string DocGiaTri(DataGridViewRow row, string cot)
        {
            object? value = row.Cells[cot].Value;

            return value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value) ?? string.Empty;
        }

        private void XoaTrang()
        {
            txtTen?.Clear();
            txtHang?.Clear();
            txtMau?.Clear();
            txtGia?.Clear();
            txtSoLuong?.Clear();
            txtMaLoai?.Clear();

            if (dgv != null)
            {
                dgv.ClearSelection();
                dgv.CurrentCell = null;
            }

            txtTen?.Focus();
        }

        private string? GiaTriHoacNull(string value)
        {
            value = value.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
