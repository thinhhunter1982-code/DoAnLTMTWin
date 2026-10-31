using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormKhachHang : Form
    {
        private DataGridView? dgvKhachHang;
        private TextBox? txtHoTen;
        private TextBox? txtSoDienThoai;
        private TextBox? txtEmail;
        private TextBox? txtDiaChi;

        private int? maKhachHangDangChon;
        private bool daKhoiTao = false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VaiTroNguoiDung { get; set; } = string.Empty;

        private bool LaAdmin =>
            string.Equals(
                VaiTroNguoiDung?.Trim(),
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        private bool LaAdminTamThoi =>
            string.Equals(
                VaiTroNguoiDung?.Trim(),
                "AdminTamThoi",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                VaiTroNguoiDung?.Trim(),
                "Admin (Tạm thời)",
                StringComparison.OrdinalIgnoreCase);

        private bool LaNhanVien =>
            string.Equals(
                VaiTroNguoiDung?.Trim(),
                "Nhân viên",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                VaiTroNguoiDung?.Trim(),
                "NhanVien",
                StringComparison.OrdinalIgnoreCase);

        private bool DuocQuanLy =>
            LaAdmin || LaAdminTamThoi;

        private bool DuocXem =>
            DuocQuanLy || LaNhanVien;

        public FormKhachHang()
        {
            InitializeComponent();
            Load += FormKhachHang_Load;
        }

        private void FormKhachHang_Load(object? sender, EventArgs e)
        {
            // Admin, Admin tạm thời và Nhân viên được truy cập.
            if (!DuocXem)
            {
                MessageBox.Show(
                    "Tài khoản hiện tại không có quyền truy cập khách hàng.",
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                BeginInvoke(new Action(Close));
                return;
            }

            TimDieuKhien();

            if (!KiemTraDieuKhien())
                return;

            dgvKhachHang!.ReadOnly = true;
            dgvKhachHang.AllowUserToAddRows = false;
            dgvKhachHang.AllowUserToDeleteRows = false;
            dgvKhachHang.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvKhachHang.MultiSelect = false;
            dgvKhachHang.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvKhachHang.RowHeadersVisible = false;

            dgvKhachHang.CellClick -= dgvKhachHang_CellClick;
            dgvKhachHang.CellClick += dgvKhachHang_CellClick;

            daKhoiTao = true;

            // Tải danh sách cho cả Nhân viên và Admin.
            TaiDanhSachKhachHang();

            // Nhân viên chỉ xem, không được thay đổi dữ liệu.
            if (!DuocQuanLy)
            {
                KhoaNhapLieu(this, true);
            }
        }

        private void TimDieuKhien()
        {
            dgvKhachHang = TimDataGridView(this);

            Dictionary<string, TextBox> textBoxes = TimTextBox(this)
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.First());

            txtHoTen = TimTextBoxTheoTen(
                textBoxes,
                "txtHoTen",
                "txtTenKhachHang",
                "txtHoTenKhachHang");

            txtSoDienThoai = TimTextBoxTheoTen(
                textBoxes,
                "txtSoDienThoai",
                "txtSDT",
                "txtDienThoai");

            txtEmail = TimTextBoxTheoTen(
                textBoxes,
                "txtEmail");

            txtDiaChi = TimTextBoxTheoTen(
                textBoxes,
                "txtDiaChi");

            List<TextBox> danhSach = TimTextBox(this)
                .Distinct()
                .OrderBy(x => x.Top)
                .ThenBy(x => x.Left)
                .ToList();

            if (danhSach.Count >= 4)
            {
                txtHoTen ??= danhSach[0];
                txtSoDienThoai ??= danhSach[1];
                txtEmail ??= danhSach[2];
                txtDiaChi ??= danhSach[3];
            }
        }

        private TextBox? TimTextBoxTheoTen(
            Dictionary<string, TextBox> danhSach,
            params string[] ten)
        {
            foreach (string key in ten)
            {
                if (danhSach.TryGetValue(key, out TextBox? textBox))
                    return textBox;
            }

            return null;
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

        private IEnumerable<TextBox> TimTextBox(Control cha)
        {
            foreach (Control control in cha.Controls)
            {
                if (control is TextBox textBox)
                    yield return textBox;

                foreach (TextBox con in TimTextBox(control))
                    yield return con;
            }
        }

        private void KhoaNhapLieu(Control cha, bool khoa)
        {
            foreach (Control control in cha.Controls)
            {
                if (control is TextBox textBox)
                    textBox.ReadOnly = khoa;

                // Nhân viên vẫn được tìm kiếm và làm mới danh sách.
                if (control is Button button)
                {
                    if (button.Name == "btnTimKiem" ||
                        button.Name == "btnLamMoi" ||
                        button.Name == "btnThoat")
                    {
                        button.Enabled = true;
                    }
                    else
                    {
                        button.Enabled = !khoa;
                    }
                }

                if (control.HasChildren)
                    KhoaNhapLieu(control, khoa);
            }
        }

        private bool KiemTraQuyenAdmin()
        {
            if (DuocQuanLy)
                return true;

            MessageBox.Show(
                "Nhân viên chỉ được xem danh sách khách hàng, " +
                "không được thêm, sửa hoặc xóa.",
                "Không có quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private bool KiemTraDieuKhien()
        {
            if (dgvKhachHang != null &&
                txtHoTen != null &&
                txtSoDienThoai != null &&
                txtEmail != null &&
                txtDiaChi != null)
            {
                return true;
            }

            MessageBox.Show(
                "Không tìm thấy đủ điều khiển trên form. " +
                "Hãy kiểm tra tên (Name) của DataGridView và 4 TextBox " +
                "họ tên, số điện thoại, email, địa chỉ trong Designer.",
                "Thiếu điều khiển",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private static bool LaSoDienThoaiHopLe(string sdt)
        {
            return Regex.IsMatch(sdt, @"^0\d{9}$");
        }

        private static bool LaEmailGmailHopLe(string email)
        {
            return Regex.IsMatch(
                email,
                @"^[A-Za-z0-9._%+\-]+@gmail\.com$",
                RegexOptions.IgnoreCase);
        }

        private bool KiemTraDinhDangNhap(string sdt, string email)
        {
            if (sdt != "" && !LaSoDienThoaiHopLe(sdt))
            {
                MessageBox.Show(
                    "Số điện thoại không hợp lệ.\n" +
                    "Số điện thoại phải gồm đúng 10 chữ số, bắt đầu bằng số 0 " +
                    "và không chứa chữ hoặc ký tự đặc biệt.",
                    "Sai định dạng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtSoDienThoai!.Focus();
                txtSoDienThoai.SelectAll();
                return false;
            }

            if (email != "" && !LaEmailGmailHopLe(email))
            {
                MessageBox.Show(
                    "Email không hợp lệ.\n" +
                    "Email phải có đuôi @gmail.com, ví dụ: tenban@gmail.com",
                    "Sai định dạng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtEmail!.Focus();
                txtEmail.SelectAll();
                return false;
            }

            return true;
        }

        private void ResetIdentityNeuBangRong(AppDbContext db)
        {
            try
            {
                if (db.KhachHangs.Any())
                    return;

                var entityType = db.Model.FindEntityType(typeof(KhachHang));
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
                    "Không reset được mã tự tăng:\n" +
                    ex.GetBaseException().Message,
                    "Lỗi reset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void TaiDanhSachKhachHang()
        {
            // Cho phép Nhân viên tải và xem danh sách.
            if (!DuocXem || !KiemTraDieuKhien())
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                var danhSach = db.KhachHangs
                    .AsNoTracking()
                    .OrderBy(x => x.MaKhachHang)
                    .Select(x => new
                    {
                        x.MaKhachHang,
                        x.TenKhachHang,
                        x.SoDienThoai,
                        x.Email,
                        x.DiaChi
                    })
                    .ToList();

                dgvKhachHang!.DataSource = null;
                dgvKhachHang.DataSource = danhSach;

                DatTieuDeCot("MaKhachHang", "Mã khách hàng");
                DatTieuDeCot("TenKhachHang", "Họ tên");
                DatTieuDeCot("SoDienThoai", "Số điện thoại");
                DatTieuDeCot("Email", "Email");
                DatTieuDeCot("DiaChi", "Địa chỉ");

                maKhachHangDangChon = null;
                dgvKhachHang.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách khách hàng.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DatTieuDeCot(string tenCot, string tieuDe)
        {
            if (dgvKhachHang?.Columns.Contains(tenCot) == true)
                dgvKhachHang.Columns[tenCot].HeaderText = tieuDe;
        }

        private void dgvKhachHang_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (!daKhoiTao ||
                dgvKhachHang == null ||
                e.RowIndex < 0 ||
                e.RowIndex >= dgvKhachHang.Rows.Count)
            {
                return;
            }

            DataGridViewRow dong = dgvKhachHang.Rows[e.RowIndex];

            if (dong.Cells["MaKhachHang"].Value == null)
                return;

            maKhachHangDangChon =
                Convert.ToInt32(dong.Cells["MaKhachHang"].Value);

            txtHoTen!.Text =
                dong.Cells["TenKhachHang"].Value?.ToString() ?? "";

            txtSoDienThoai!.Text =
                dong.Cells["SoDienThoai"].Value?.ToString() ?? "";

            txtEmail!.Text =
                dong.Cells["Email"].Value?.ToString() ?? "";

            txtDiaChi!.Text =
                dong.Cells["DiaChi"].Value?.ToString() ?? "";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || !KiemTraDieuKhien())
                return;

            string hoTen = txtHoTen!.Text.Trim();
            string sdt = txtSoDienThoai!.Text.Trim();
            string email = txtEmail!.Text.Trim();
            string diaChi = txtDiaChi!.Text.Trim();

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng.");
                txtHoTen.Focus();
                return;
            }

            if (!KiemTraDinhDangNhap(sdt, email))
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                // Không cho trùng họ tên.
                if (db.KhachHangs.Any(x => x.TenKhachHang == hoTen))
                {
                    MessageBox.Show(
                        "Họ tên khách hàng đã tồn tại.",
                        "Trùng họ tên",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtHoTen.Focus();
                    txtHoTen.SelectAll();
                    return;
                }

                if (sdt != "" &&
                    db.KhachHangs.Any(x => x.SoDienThoai == sdt))
                {
                    MessageBox.Show("Số điện thoại đã tồn tại.");
                    return;
                }

                if (email != "" &&
                    db.KhachHangs.Any(x => x.Email == email))
                {
                    MessageBox.Show("Email đã tồn tại.");
                    return;
                }

                ResetIdentityNeuBangRong(db);

                KhachHang khachHang = new KhachHang
                {
                    TenKhachHang = hoTen,
                    SoDienThoai = sdt == "" ? null : sdt,
                    Email = email == "" ? null : email,
                    DiaChi = diaChi == "" ? null : diaChi
                };

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                MessageBox.Show(
                    $"Thêm khách hàng thành công. Mã khách hàng: " +
                    $"{khachHang.MaKhachHang}");

                XoaTrang();
                TaiDanhSachKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm khách hàng.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || !KiemTraDieuKhien())
                return;

            if (maKhachHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.");
                return;
            }

            string hoTen = txtHoTen!.Text.Trim();
            string sdt = txtSoDienThoai!.Text.Trim();
            string email = txtEmail!.Text.Trim();
            string diaChi = txtDiaChi!.Text.Trim();

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng.");
                txtHoTen.Focus();
                return;
            }

            if (!KiemTraDinhDangNhap(sdt, email))
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                KhachHang? khachHang = db.KhachHangs.FirstOrDefault(
                    x => x.MaKhachHang == maKhachHangDangChon.Value);

                if (khachHang == null)
                {
                    MessageBox.Show("Khách hàng không còn tồn tại.");
                    TaiDanhSachKhachHang();
                    return;
                }

                // Không cho trùng họ tên với khách hàng khác.
                if (db.KhachHangs.Any(
                    x => x.TenKhachHang == hoTen &&
                         x.MaKhachHang != khachHang.MaKhachHang))
                {
                    MessageBox.Show(
                        "Họ tên khách hàng đã được sử dụng.",
                        "Trùng họ tên",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtHoTen.Focus();
                    txtHoTen.SelectAll();
                    return;
                }

                if (sdt != "" && db.KhachHangs.Any(
                    x => x.SoDienThoai == sdt &&
                         x.MaKhachHang != khachHang.MaKhachHang))
                {
                    MessageBox.Show("Số điện thoại đã được sử dụng.");
                    return;
                }

                if (email != "" && db.KhachHangs.Any(
                    x => x.Email == email &&
                         x.MaKhachHang != khachHang.MaKhachHang))
                {
                    MessageBox.Show("Email đã được sử dụng.");
                    return;
                }

                khachHang.TenKhachHang = hoTen;
                khachHang.SoDienThoai = sdt == "" ? null : sdt;
                khachHang.Email = email == "" ? null : email;
                khachHang.DiaChi = diaChi == "" ? null : diaChi;

                db.SaveChanges();

                MessageBox.Show("Cập nhật khách hàng thành công.");

                XoaTrang();
                TaiDanhSachKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể sửa khách hàng.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || !KiemTraDieuKhien())
                return;

            if (maKhachHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa.");
                return;
            }

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa khách hàng này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using AppDbContext db = new AppDbContext();

                KhachHang? khachHang = db.KhachHangs.FirstOrDefault(
                    x => x.MaKhachHang == maKhachHangDangChon.Value);

                if (khachHang == null)
                {
                    MessageBox.Show("Khách hàng không còn tồn tại.");
                    TaiDanhSachKhachHang();
                    return;
                }

                if (db.HoaDons.Any(
                    x => x.MaKhachHang == khachHang.MaKhachHang))
                {
                    MessageBox.Show(
                        "Không thể xóa khách hàng vì đã có hóa đơn liên quan.");
                    return;
                }

                db.KhachHangs.Remove(khachHang);
                db.SaveChanges();

                ResetIdentityNeuBangRong(db);

                MessageBox.Show("Xóa khách hàng thành công.");

                XoaTrang();
                TaiDanhSachKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa khách hàng.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            if (!DuocXem || !KiemTraDieuKhien())
                return;

            XoaTrang();
            TaiDanhSachKhachHang();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Nhân viên được tìm kiếm, nhưng không được sửa dữ liệu.
            if (!DuocXem || !KiemTraDieuKhien())
                return;

            string tuKhoa = txtHoTen!.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = txtSoDienThoai!.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = txtEmail!.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = txtDiaChi!.Text.Trim();

            try
            {
                using AppDbContext db = new AppDbContext();

                var truyVan = db.KhachHangs.AsNoTracking();

                if (tuKhoa != "")
                {
                    if (int.TryParse(tuKhoa, out int ma))
                    {
                        truyVan = truyVan.Where(x =>
                            x.MaKhachHang == ma ||
                            x.TenKhachHang.Contains(tuKhoa) ||
                            (x.SoDienThoai != null &&
                             x.SoDienThoai.Contains(tuKhoa)) ||
                            (x.Email != null &&
                             x.Email.Contains(tuKhoa)) ||
                            (x.DiaChi != null &&
                             x.DiaChi.Contains(tuKhoa)));
                    }
                    else
                    {
                        truyVan = truyVan.Where(x =>
                            x.TenKhachHang.Contains(tuKhoa) ||
                            (x.SoDienThoai != null &&
                             x.SoDienThoai.Contains(tuKhoa)) ||
                            (x.Email != null &&
                             x.Email.Contains(tuKhoa)) ||
                            (x.DiaChi != null &&
                             x.DiaChi.Contains(tuKhoa)));
                    }
                }

                var ketQua = truyVan
                    .OrderBy(x => x.MaKhachHang)
                    .Select(x => new
                    {
                        x.MaKhachHang,
                        x.TenKhachHang,
                        x.SoDienThoai,
                        x.Email,
                        x.DiaChi
                    })
                    .ToList();

                dgvKhachHang!.DataSource = null;
                dgvKhachHang.DataSource = ketQua;

                DatTieuDeCot("MaKhachHang", "Mã khách hàng");
                DatTieuDeCot("TenKhachHang", "Họ tên");
                DatTieuDeCot("SoDienThoai", "Số điện thoại");
                DatTieuDeCot("Email", "Email");
                DatTieuDeCot("DiaChi", "Địa chỉ");

                maKhachHangDangChon = null;
                dgvKhachHang.ClearSelection();

                if (ketQua.Count == 0)
                    MessageBox.Show("Không tìm thấy khách hàng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm khách hàng.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void XoaTrang()
        {
            maKhachHangDangChon = null;

            txtHoTen?.Clear();
            txtSoDienThoai?.Clear();
            txtEmail?.Clear();
            txtDiaChi?.Clear();

            dgvKhachHang?.ClearSelection();
        }
    }
}