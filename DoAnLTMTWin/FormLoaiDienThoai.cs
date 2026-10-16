using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormLoaiDienThoai : Form
    {
        private int? maLoaiDangChon;

        private DataGridView? dgvLoaiDienThoai;
        private TextBox? txtTenLoai;
        private TextBox? txtMoTa;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VaiTroNguoiDung { get; set; } = string.Empty;

        private bool LaAdmin =>
            VaiTroNguoiDung == "Admin";

        public FormLoaiDienThoai()
        {
            InitializeComponent();

            TimDieuKhien();

            Load += FormLoaiDienThoai_Load;
        }

        private void TimDieuKhien()
        {
            dgvLoaiDienThoai = TimDataGridView(this);

            TextBox[] danhSachTextBox = TimTextBox(this)
                .OrderBy(x => x.Top)
                .ThenBy(x => x.Left)
                .ToArray();

            if (danhSachTextBox.Length >= 2)
            {
                txtTenLoai = danhSachTextBox[0];
                txtMoTa = danhSachTextBox[1];
            }
        }

        private DataGridView? TimDataGridView(Control cha)
        {
            foreach (Control control in cha.Controls)
            {
                if (control is DataGridView dgv)
                    return dgv;

                DataGridView? ketQua = TimDataGridView(control);

                if (ketQua != null)
                    return ketQua;
            }

            return null;
        }

        private System.Collections.Generic.IEnumerable<TextBox>
            TimTextBox(Control cha)
        {
            foreach (Control control in cha.Controls)
            {
                if (control is TextBox textBox)
                    yield return textBox;

                foreach (TextBox textBoxCon in TimTextBox(control))
                    yield return textBoxCon;
            }
        }

        private void FormLoaiDienThoai_Load(
            object? sender,
            EventArgs e)
        {
            if (dgvLoaiDienThoai == null)
            {
                MessageBox.Show(
                    "Không tìm thấy DataGridView trên form.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (txtTenLoai == null || txtMoTa == null)
            {
                MessageBox.Show(
                    "Không tìm thấy đủ hai TextBox tên loại và mô tả. " +
                    "Hãy kiểm tra lại các điều khiển trên Designer.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            dgvLoaiDienThoai.ReadOnly = true;
            dgvLoaiDienThoai.AllowUserToAddRows = false;
            dgvLoaiDienThoai.AllowUserToDeleteRows = false;
            dgvLoaiDienThoai.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvLoaiDienThoai.MultiSelect = false;
            dgvLoaiDienThoai.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvLoaiDienThoai.CellClick -= dgvLoaiDienThoai_CellClick;
            dgvLoaiDienThoai.CellClick += dgvLoaiDienThoai_CellClick;

            CapNhatQuyen();
            TaiDanhSach();
        }

        private void CapNhatQuyen()
        {
            foreach (Control control in TimTatCaDieuKhien(this))
            {
                string ten = control.Name.ToLowerInvariant();

                if (ten.Contains("them") ||
                    ten.Contains("sua") ||
                    ten.Contains("xoa"))
                {
                    control.Enabled = LaAdmin;
                }
            }

            if (txtTenLoai != null)
                txtTenLoai.ReadOnly = !LaAdmin;

            if (txtMoTa != null)
                txtMoTa.ReadOnly = !LaAdmin;
        }

        private System.Collections.Generic.IEnumerable<Control>
            TimTatCaDieuKhien(Control cha)
        {
            foreach (Control control in cha.Controls)
            {
                yield return control;

                foreach (Control con in TimTatCaDieuKhien(control))
                    yield return con;
            }
        }

        private bool KiemTraQuyenAdmin()
        {
            if (LaAdmin)
                return true;

            MessageBox.Show(
                "Bạn cần đăng nhập bằng tài khoản Admin để thực hiện chức năng này.",
                "Không có quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private void ResetIdentityNeuBangRong(AppDbContext db)
        {
            try
            {
                if (db.LoaiDienThoais.Any())
                    return;

                var entityType = db.Model.FindEntityType(typeof(LoaiDienThoai));
                string? tenBang = entityType?.GetTableName();

                if (string.IsNullOrWhiteSpace(tenBang))
                    return;

                string schema = entityType?.GetSchema() ?? "dbo";

                string cauLenh =
                    $"DBCC CHECKIDENT ('[{schema}].[{tenBang}]', RESEED, 0)";

                db.Database.ExecuteSqlRaw(cauLenh);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không reset được mã tự tăng:\n" + ex.GetBaseException().Message,
                    "Lỗi reset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void TaiDanhSach()
        {
            try
            {
                using AppDbContext db = new AppDbContext();

                var danhSach = db.LoaiDienThoais
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai)
                    .Select(x => new
                    {
                        x.MaLoai,
                        x.TenLoai,
                        x.MoTa
                    })
                    .ToList();

                if (dgvLoaiDienThoai != null)
                {
                    dgvLoaiDienThoai.DataSource = null;
                    dgvLoaiDienThoai.DataSource = danhSach;
                }

                maLoaiDangChon = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách loại điện thoại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvLoaiDienThoai_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (dgvLoaiDienThoai == null ||
                e.RowIndex < 0 ||
                e.RowIndex >= dgvLoaiDienThoai.Rows.Count)
            {
                return;
            }

            DataGridViewRow dong =
                dgvLoaiDienThoai.Rows[e.RowIndex];

            if (dong.Cells["MaLoai"].Value == null)
                return;

            maLoaiDangChon = Convert.ToInt32(
                dong.Cells["MaLoai"].Value);

            if (txtTenLoai != null)
            {
                txtTenLoai.Text =
                    dong.Cells["TenLoai"].Value?.ToString() ?? "";
            }

            if (txtMoTa != null)
            {
                txtMoTa.Text =
                    dong.Cells["MoTa"].Value?.ToString() ?? "";
            }
        }

        private void btnThem_Click(object? sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (txtTenLoai == null || txtMoTa == null)
                return;

            string ten = txtTenLoai.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên loại điện thoại.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenLoai.Focus();
                return;
            }

            try
            {
                using AppDbContext db = new AppDbContext();

                bool daTonTai = db.LoaiDienThoais
                    .Any(x => x.TenLoai == ten);

                if (daTonTai)
                {
                    MessageBox.Show(
                        "Tên loại điện thoại này đã tồn tại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                LoaiDienThoai loaiMoi = new LoaiDienThoai
                {
                    TenLoai = ten,
                    MoTa = moTa
                };

                db.LoaiDienThoais.Add(loaiMoi);
                db.SaveChanges();

                MessageBox.Show(
                    $"Thêm loại điện thoại thành công. Mã loại mới: {loaiMoi.MaLoai}",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrang();
                TaiDanhSach();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Không thể thêm loại điện thoại. Hãy kiểm tra dữ liệu và cấu hình khóa chính.\n" +
                    ex.GetBaseException().Message,
                    "Lỗi cơ sở dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi khi thêm loại điện thoại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object? sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (maLoaiDangChon == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn loại điện thoại cần sửa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (txtTenLoai == null || txtMoTa == null)
                return;

            string ten = txtTenLoai.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên loại điện thoại.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenLoai.Focus();
                return;
            }

            try
            {
                using AppDbContext db = new AppDbContext();

                LoaiDienThoai? loai = db.LoaiDienThoais
                    .FirstOrDefault(x => x.MaLoai == maLoaiDangChon.Value);

                if (loai == null)
                {
                    MessageBox.Show(
                        "Loại điện thoại không còn tồn tại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    TaiDanhSach();
                    return;
                }

                bool trungTen = db.LoaiDienThoais.Any(
                    x => x.TenLoai == ten &&
                         x.MaLoai != maLoaiDangChon.Value);

                if (trungTen)
                {
                    MessageBox.Show(
                        "Tên loại điện thoại này đã được sử dụng.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                loai.TenLoai = ten;
                loai.MoTa = moTa;

                db.SaveChanges();

                MessageBox.Show(
                    "Cập nhật loại điện thoại thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrang();
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi khi sửa loại điện thoại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (maLoaiDangChon == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn loại điện thoại cần xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa loại điện thoại đang chọn?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                LoaiDienThoai? loai = db.LoaiDienThoais
                    .FirstOrDefault(x => x.MaLoai == maLoaiDangChon.Value);

                if (loai == null)
                {
                    MessageBox.Show(
                        "Loại điện thoại không còn tồn tại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    TaiDanhSach();
                    return;
                }

                bool dangDuocSuDung = db.DienThoais
                    .Any(x => x.MaLoai == loai.MaLoai);

                if (dangDuocSuDung)
                {
                    MessageBox.Show(
                        "Không thể xóa loại này vì vẫn còn điện thoại đang sử dụng loại đó.",
                        "Không thể xóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                db.LoaiDienThoais.Remove(loai);
                db.SaveChanges();

                ResetIdentityNeuBangRong(db);

                MessageBox.Show(
                    "Xóa loại điện thoại thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrang();
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi khi xóa loại điện thoại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnTimKiem_Click(object? sender, EventArgs e)
        {
            if (txtTenLoai == null || dgvLoaiDienThoai == null)
                return;

            string tuKhoa = txtTenLoai.Text.Trim();

            try
            {
                using AppDbContext db = new AppDbContext();

                var truyVan = db.LoaiDienThoais
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    truyVan = truyVan.Where(
                        x => x.TenLoai.Contains(tuKhoa));
                }

                var ketQua = truyVan
                    .OrderBy(x => x.MaLoai)
                    .Select(x => new
                    {
                        x.MaLoai,
                        x.TenLoai,
                        x.MoTa
                    })
                    .ToList();

                dgvLoaiDienThoai.DataSource = null;
                dgvLoaiDienThoai.DataSource = ketQua;

                maLoaiDangChon = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi khi tìm kiếm loại điện thoại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object? sender, EventArgs e)
        {
            XoaTrang();
            TaiDanhSach();
        }

        private void XoaTrang()
        {
            maLoaiDangChon = null;

            if (txtTenLoai != null)
                txtTenLoai.Clear();

            if (txtMoTa != null)
                txtMoTa.Clear();

            dgvLoaiDienThoai?.ClearSelection();
        }
    }
}