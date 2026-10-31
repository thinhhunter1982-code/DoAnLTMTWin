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
        private TextBox? txtBaoHanh;

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
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo form:\n" + ex.Message);
            }
        }

        private void AnhXaControl()
        {
            List<Control> controls = LayTatCaControl(this).ToList();

            dgv = controls.OfType<DataGridView>().FirstOrDefault();

            if (dgv == null)
                throw new Exception("Không tìm thấy DataGridView.");

            GroupBox? khung = controls.OfType<GroupBox>()
                .FirstOrDefault(g => g.Text.Contains(
                    "QUẢN LÝ ĐIỆN THOẠI",
                    StringComparison.OrdinalIgnoreCase));

            if (khung == null)
                throw new Exception("Không tìm thấy GroupBox quản lý điện thoại.");

            // textBox1 là ô thời gian bảo hành.
            txtBaoHanh = controls.OfType<TextBox>()
                .FirstOrDefault(t => t.Name == "textBox1");

            if (txtBaoHanh == null)
                throw new Exception("Không tìm thấy textBox1 bảo hành.");

            // Lấy 6 TextBox còn lại, không lấy textBox1.
            List<TextBox> oNhap = LayTatCaControl(khung)
                .OfType<TextBox>()
                .Where(t => t.Name != "textBox1")
                .OrderBy(t => t.Top)
                .ThenBy(t => t.Left)
                .ToList();

            if (oNhap.Count < 6)
                throw new Exception("Không tìm thấy đủ TextBox nhập liệu.");

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
            dgv!.AutoGenerateColumns = true;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

            txtMaLoai!.ReadOnly = !LaAdmin;
            txtTen!.ReadOnly = !LaAdmin;
            txtHang!.ReadOnly = !LaAdmin;
            txtMau!.ReadOnly = !LaAdmin;
            txtGia!.ReadOnly = !LaAdmin;
            txtSoLuong!.ReadOnly = !LaAdmin;
            txtBaoHanh!.ReadOnly = !LaAdmin;
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
            try
            {
                dangNap = true;

                using var db = new AppDbContext();

                dgv!.DataSource = db.DienThoais
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
                        d.MaLoai,
                        d.ThoiGianBaoHanh
                    })
                    .ToList();

                DatTieuDeCot();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách:\n" + ex.Message);
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
            DatCot("ThoiGianBaoHanh", "Thời gian bảo hành (tháng)");

            if (dgv?.Columns["GiaBan"] != null)
                dgv.Columns["GiaBan"]!.DefaultCellStyle.Format = "N0";
        }

        private void DatCot(string tenCot, string tieuDe)
        {
            if (dgv?.Columns[tenCot] != null)
                dgv.Columns[tenCot]!.HeaderText = tieuDe;
        }

        private bool KiemTraDuLieu()
        {
            if (string.IsNullOrWhiteSpace(txtTen!.Text))
            {
                MessageBox.Show("Vui lòng nhập tên điện thoại.");
                txtTen.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHang!.Text))
            {
                MessageBox.Show("Vui lòng nhập hãng sản xuất.");
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

            if (!int.TryParse(txtBaoHanh!.Text.Trim(), out int baoHanh)
                || baoHanh <= 0)
            {
                MessageBox.Show("Thời gian bảo hành phải là số tháng lớn hơn 0.");
                txtBaoHanh.Focus();
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
                MessageBox.Show("Mã loại không tồn tại.");
                txtMaLoai!.Focus();
                return false;
            }

            if (!string.Equals(
                hangNhap,
                loai.TenLoai.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Hãng sản xuất không khớp với tên loại: " +
                    loai.TenLoai);

                txtHang!.Focus();
                return false;
            }

            return true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || !KiemTraDuLieu())
                return;

            try
            {
                using var db = new AppDbContext();

                int maLoai = int.Parse(txtMaLoai!.Text.Trim());

                if (!KiemTraMaLoaiVaHang(db, maLoai))
                    return;

                var dt = new DienThoai
                {
                    TenDienThoai = txtTen!.Text.Trim(),
                    HangSanXuat = txtHang!.Text.Trim(),
                    MauSac = GiaTriHoacNull(txtMau!.Text),
                    GiaBan = decimal.Parse(txtGia!.Text.Trim()),
                    SoLuongTon = int.Parse(txtSoLuong!.Text.Trim()),
                    MaLoai = maLoai,
                    ThoiGianBaoHanh = int.Parse(txtBaoHanh!.Text.Trim())
                };

                db.DienThoais.Add(dt);
                db.SaveChanges();

                MessageBox.Show(
                    "Thêm điện thoại thành công!\nMã điện thoại: " +
                    dt.MaDienThoai);

                TaiDanhSach();
                XoaTrang();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show("Lỗi SQL:\n" +
                    ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm điện thoại:\n" + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (dgv?.CurrentRow == null ||
                dgv.CurrentRow.Cells["MaDienThoai"].Value == null)
            {
                MessageBox.Show("Hãy chọn điện thoại cần sửa.");
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
                dt.ThoiGianBaoHanh = int.Parse(txtBaoHanh!.Text.Trim());

                db.SaveChanges();

                MessageBox.Show("Sửa điện thoại thành công.");

                TaiDanhSach();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa điện thoại:\n" + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (dgv?.CurrentRow == null ||
                dgv.CurrentRow.Cells["MaDienThoai"].Value == null)
            {
                MessageBox.Show("Hãy chọn điện thoại cần xóa.");
                return;
            }

            int ma = Convert.ToInt32(
                dgv.CurrentRow.Cells["MaDienThoai"].Value);

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa điện thoại mã " + ma + " không?",
                "Xác nhận",
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
                    return;
                }

                bool coHoaDon = db.ChiTietHoaDons
                    .Any(x => x.MaDienThoai == ma);

                bool coPhieuNhap = db.ChiTietPhieuNhaps
                    .Any(x => x.MaDienThoai == ma);

                if (coHoaDon || coPhieuNhap)
                {
                    MessageBox.Show(
                        "Không thể xóa điện thoại đã có trong hóa đơn hoặc phiếu nhập.");
                    return;
                }

                db.DienThoais.Remove(dt);
                db.SaveChanges();

                MessageBox.Show("Xóa điện thoại thành công.");

                TaiDanhSach();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa điện thoại:\n" + ex.Message);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            TaiDanhSach();
            XoaTrang();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maLoaiNhap = txtMaLoai!.Text.Trim();
            string tenNhap = txtTen!.Text.Trim();

            if (maLoaiNhap == "" && tenNhap == "")
            {
                MessageBox.Show("Nhập mã loại hoặc tên điện thoại cần tìm.");
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var truyVan = db.DienThoais.AsNoTracking();

                if (maLoaiNhap != "")
                {
                    if (!int.TryParse(maLoaiNhap, out int maLoai) ||
                        maLoai <= 0)
                    {
                        MessageBox.Show("Mã loại không hợp lệ.");
                        return;
                    }

                    truyVan = truyVan.Where(d => d.MaLoai == maLoai);
                }
                else
                {
                    truyVan = truyVan.Where(d =>
                        d.TenDienThoai.Contains(tenNhap));
                }

                dangNap = true;

                dgv!.DataSource = truyVan
                    .OrderBy(d => d.MaDienThoai)
                    .Select(d => new
                    {
                        d.MaDienThoai,
                        d.TenDienThoai,
                        d.HangSanXuat,
                        d.MauSac,
                        d.GiaBan,
                        d.SoLuongTon,
                        d.MaLoai,
                        d.ThoiGianBaoHanh
                    })
                    .ToList();

                DatTieuDeCot();

                if (dgv.Rows.Count == 0)
                    MessageBox.Show("Không tìm thấy điện thoại.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm:\n" + ex.Message);
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
            txtBaoHanh!.Text = DocGiaTri(row, "ThoiGianBaoHanh");
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
            bool trangThaiCu = dangNap;
            dangNap = true;

            try
            {
                txtTen?.Clear();
                txtHang?.Clear();
                txtMau?.Clear();
                txtGia?.Clear();
                txtSoLuong?.Clear();
                txtMaLoai?.Clear();
                txtBaoHanh?.Clear();

                if (dgv != null)
                {
                    dgv.ClearSelection();
                    dgv.CurrentCell = null;
                }
            }
            finally
            {
                dangNap = trangThaiCu;
            }

            txtTen?.Focus();
        }

        private string? GiaTriHoacNull(string value)
        {
            value = value.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private void btnMua_Click(object sender, EventArgs e)
        {
            // Kiểm tra đã chọn điện thoại chưa.
            if (dgv?.CurrentRow == null ||
                dgv.CurrentRow.Cells["MaDienThoai"].Value == null)
            {
                MessageBox.Show("Vui lòng chọn điện thoại cần mua.");
                return;
            }

            // Lấy mã điện thoại đang chọn.
            int maDienThoai = Convert.ToInt32(
                dgv.CurrentRow.Cells["MaDienThoai"].Value);

            // Tìm Form chính.
            FormQuanLyCuaHangDienThoai? formChinh =
                Application.OpenForms
                .OfType<FormQuanLyCuaHangDienThoai>()
                .FirstOrDefault();

            if (formChinh == null)
            {
                MessageBox.Show("Không tìm thấy Form chính.");
                return;
            }

            // Gửi mã điện thoại sang Form bán hàng.
            formChinh.MoFormBanHang(maDienThoai);
        }
    }
}