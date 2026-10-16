using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormNhanVien : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VaiTro { get; set; } = string.Empty;

        private int maNhanVienDangChon;
        private bool dangNapDuLieu;

        public FormNhanVien()
        {
            InitializeComponent();
            GanSuKien();
        }

        public FormNhanVien(string vaiTro) : this()
        {
            VaiTro = vaiTro;
        }

        private IEnumerable<Control> TimTatCaControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;

                foreach (Control child in TimTatCaControls(control))
                    yield return child;
            }
        }

        private T? TimControl<T>(params string[] tenControls)
            where T : Control
        {
            var tatCa = TimTatCaControls(this).ToList();

            foreach (string ten in tenControls)
            {
                T? control = tatCa
                    .Where(c => string.Equals(
                        c.Name,
                        ten,
                        StringComparison.OrdinalIgnoreCase))
                    .OfType<T>()
                    .FirstOrDefault();

                if (control != null)
                    return control;
            }

            return null;
        }

        private DataGridView? LuoiNhanVien =>
            TimControl<DataGridView>(
                "dataGridView1",
                "dgvNhanVien",
                "dgvDanhSachNhanVien");

        private TextBox? OTenNhanVien =>
            TimControl<TextBox>(
                "txtTenNhanVien",
                "txtHoTen",
                "textBox1");

        private RadioButton? TimRadioTheoTenHoacChu(string gioiTinh)
        {
            var tatCaRadio = TimTatCaControls(this)
                .OfType<RadioButton>()
                .ToList();

            string[] tenCanTim = gioiTinh == "Nam"
                ? new[] { "radioButtonNam", "rdoNam", "radNam", "rdbNam" }
                : new[] { "radioButtonNu", "rdoNu", "radNu", "rdbNu" };

            RadioButton? radio = tatCaRadio.FirstOrDefault(r =>
                tenCanTim.Any(ten =>
                    string.Equals(
                        r.Name,
                        ten,
                        StringComparison.OrdinalIgnoreCase)));

            return radio ?? tatCaRadio.FirstOrDefault(r =>
                string.Equals(
                    r.Text.Trim(),
                    gioiTinh,
                    StringComparison.OrdinalIgnoreCase));
        }

        private RadioButton? RNam => TimRadioTheoTenHoacChu("Nam");

        private RadioButton? RNu => TimRadioTheoTenHoacChu("Nữ");

        private TextBox? OSdt =>
            TimControl<TextBox>("textBox4");

        private TextBox? OChucVu =>
            TimControl<TextBox>("txtChucVu", "textBox6");

        private TextBox? ODiaChi =>
            TimControl<TextBox>("txtDiaChi", "textBox5");

        private DateTimePicker? ONgaySinh =>
            TimControl<DateTimePicker>(
                "dtpNgaySinh",
                "dateTimePicker1");

        private TextBox? OTimKiem =>
            TimControl<TextBox>(
                "txtTimKiem",
                "txtTuKhoa",
                "textBoxTimKiem");

        private void GanSuKien()
        {
            GanClick<Button>(
                new[] { "button1", "btnThem" },
                button1_Click);

            GanClick<Button>(
                new[] { "btnSua" },
                btnSua_Click);

            GanClick<Button>(
                new[] { "btnXoa" },
                btnXoa_Click);

            GanClick<Button>(
                new[] { "btnLamMoi" },
                btnLamMoi_Click);

            GanClick<Button>(
                new[] { "btnTimKiem" },
                btnTimKiem_Click);

            DataGridView? luoi = LuoiNhanVien;

            if (luoi != null)
            {
                luoi.CellClick -= dgvNhanVien_CellClick;
                luoi.CellClick += dgvNhanVien_CellClick;
            }

            Load -= FormNhanVien_Load;
            Load += FormNhanVien_Load;
        }

        private void GanClick<T>(
            string[] tenControls,
            EventHandler xuLy)
            where T : Control
        {
            T? control = TimControl<T>(tenControls);

            if (control is Button button)
            {
                button.Click -= xuLy;
                button.Click += xuLy;
            }
        }

        private void KiemTraQuyen()
        {
            bool laAdmin = string.Equals(
                VaiTro,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

            foreach (string ten in new[]
            {
                "button1", "btnThem", "btnSua", "btnXoa"
            })
            {
                Button? button = TimControl<Button>(ten);

                if (button != null)
                    button.Enabled = laAdmin;
            }
        }

        private void FormNhanVien_Load(object? sender, EventArgs e)
        {
            KiemTraQuyen();

            DataGridView? luoi = LuoiNhanVien;

            if (luoi != null)
            {
                luoi.AutoGenerateColumns = true;
                luoi.ReadOnly = true;
                luoi.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                luoi.MultiSelect = false;
                luoi.AllowUserToAddRows = false;
            }

            DateTimePicker? ngaySinhPicker = ONgaySinh;

            if (ngaySinhPicker != null)
            {
                ngaySinhPicker.MaxDate = DateTime.Today;

                DateTime macDinh = DateTime.Today.AddYears(-18);

                ngaySinhPicker.Value = macDinh < ngaySinhPicker.MinDate
                    ? ngaySinhPicker.MinDate
                    : macDinh;
            }

            TaiDanhSachNhanVien();
        }

        private void TaiDanhSachNhanVien(string? tuKhoa = null)
        {
            try
            {
                using AppDbContext db = new AppDbContext();

                IQueryable<NhanVien> query =
                    db.NhanViens.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    string tuKhoaTim = tuKhoa.Trim();

                    query = query.Where(nv =>
                        nv.TenNhanVien.Contains(tuKhoaTim) ||
                        (nv.SoDienThoai != null &&
                         nv.SoDienThoai.Contains(tuKhoaTim)) ||
                        (nv.ChucVu != null &&
                         nv.ChucVu.Contains(tuKhoaTim)) ||
                        (nv.DiaChi != null &&
                         nv.DiaChi.Contains(tuKhoaTim)) ||
                        (nv.GioiTinh != null &&
                         nv.GioiTinh.Contains(tuKhoaTim)));
                }

                var danhSach = query
                    .OrderBy(nv => nv.MaNhanVien)
                    .ToList();

                DataGridView? luoi = LuoiNhanVien;

                if (luoi == null)
                    return;

                dangNapDuLieu = true;

                luoi.DataSource = null;
                luoi.DataSource = danhSach;

                var tieuDeCot = new Dictionary<string, string>
                {
                    ["MaNhanVien"] = "Mã nhân viên",
                    ["TenNhanVien"] = "Họ tên",
                    ["GioiTinh"] = "Giới tính",
                    ["NgaySinh"] = "Ngày sinh",
                    ["SoDienThoai"] = "Số điện thoại",
                    ["DiaChi"] = "Địa chỉ",
                    ["ChucVu"] = "Chức vụ"
                };

                foreach (var cot in tieuDeCot)
                {
                    DataGridViewColumn? cotLuoi = luoi.Columns[cot.Key];

                    if (cotLuoi != null)
                        cotLuoi.HeaderText = cot.Value;
                }

                foreach (string tenCot in new[] { "HoaDons", "PhieuNhaps" })
                {
                    DataGridViewColumn? cotLuoi = luoi.Columns[tenCot];

                    if (cotLuoi != null)
                        cotLuoi.Visible = false;
                }

                dangNapDuLieu = false;
            }
            catch (Exception ex)
            {
                dangNapDuLieu = false;

                MessageBox.Show(
                    "Không thể tải danh sách nhân viên.\n" + ex.Message,
                    "Lỗi dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool KiemTraDuLieu(
            out string ten,
            out string gioiTinh,
            out DateTime ngaySinh,
            out string soDienThoai,
            out string diaChi,
            out string chucVu)
        {
            ten = OTenNhanVien?.Text.Trim() ?? string.Empty;

            bool daChonNam = RNam?.Checked == true;
            bool daChonNu = RNu?.Checked == true;

            gioiTinh = daChonNam ? "Nam" : daChonNu ? "Nữ" : string.Empty;
            soDienThoai = OSdt?.Text.Trim() ?? string.Empty;
            diaChi = ODiaChi?.Text.Trim() ?? string.Empty;
            chucVu = OChucVu?.Text.Trim() ?? string.Empty;
            ngaySinh = ONgaySinh?.Value.Date ?? DateTime.Today;

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên.");
                OTenNhanVien?.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(gioiTinh))
            {
                MessageBox.Show("Vui lòng chọn giới tính Nam hoặc Nữ.");
                return false;
            }

            if (ngaySinh.Date >= DateTime.Today)
            {
                MessageBox.Show("Ngày sinh phải trước ngày hôm nay.");
                ONgaySinh?.Focus();
                return false;
            }

            TextBox? oSdt = OSdt;

            if (oSdt == null)
            {
                MessageBox.Show(
                    "Không tìm thấy ô số điện thoại textBox4. " +
                    "Hãy kiểm tra lại tên trong Designer.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.");
                oSdt.Focus();
                return false;
            }

            if (soDienThoai.Length != 10 ||
                soDienThoai[0] != '0' ||
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Số điện thoại phải có đúng 10 chữ số và bắt đầu bằng số 0.");
                oSdt.Focus();
                oSdt.SelectAll();
                return false;
            }

            return true;
        }

        private bool PhaiDungVaoVoiTrung(
            AppDbContext db,
            string ten,
            string sdt,
            int maBoQua)
        {
            if (db.NhanViens.Any(
                n => n.SoDienThoai == sdt &&
                     n.MaNhanVien != maBoQua))
            {
                MessageBox.Show(
                    "Số điện thoại này đã được nhân viên khác sử dụng.",
                    "Trùng số điện thoại",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                OSdt?.Focus();
                OSdt?.SelectAll();
                return true;
            }

            if (db.NhanViens.Any(
                n => n.TenNhanVien == ten &&
                     n.MaNhanVien != maBoQua))
            {
                DialogResult traLoi = MessageBox.Show(
                    "Đã có nhân viên trùng họ tên \"" + ten + "\".\n" +
                    "Số điện thoại khác nhau nên có thể là hai người khác nhau.\n\n" +
                    "Bạn có muốn tiếp tục không?",
                    "Trùng họ tên",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (traLoi != DialogResult.Yes)
                {
                    OTenNhanVien?.Focus();
                    OTenNhanVien?.SelectAll();
                    return true;
                }
            }

            return false;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (!string.Equals(VaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Chỉ Admin mới được thêm nhân viên.");
                return;
            }

            if (!KiemTraDuLieu(
                out string ten,
                out string gioiTinh,
                out DateTime ngaySinh,
                out string sdt,
                out string diaChi,
                out string chucVu))
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                if (PhaiDungVaoVoiTrung(db, ten, sdt, 0))
                    return;

                if (!db.NhanViens.Any() &&
                    !db.HoaDons.Any() &&
                    !db.PhieuNhaps.Any())
                {
                    var entityType = db.Model.FindEntityType(typeof(NhanVien));
                    string tenBang = entityType?.GetTableName() ?? "NhanViens";
                    string schema = entityType?.GetSchema() ?? "dbo";

                    string cauLenh =
                        $"DBCC CHECKIDENT ('[{schema}].[{tenBang}]', RESEED, 0)";

                    db.Database.ExecuteSqlRaw(cauLenh);
                }

                NhanVien nhanVien = new()
                {
                    TenNhanVien = ten,
                    GioiTinh = gioiTinh,
                    NgaySinh = ngaySinh,
                    SoDienThoai = sdt,
                    DiaChi = string.IsNullOrWhiteSpace(diaChi) ? null : diaChi,
                    ChucVu = string.IsNullOrWhiteSpace(chucVu) ? null : chucVu
                };

                db.NhanViens.Add(nhanVien);
                db.SaveChanges();

                MessageBox.Show(
                    "Thêm nhân viên thành công. Mã nhân viên: " +
                    nhanVien.MaNhanVien);

                LamMoiForm();
                TaiDanhSachNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm nhân viên.\n" + ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object? sender, EventArgs e)
        {
            if (VaiTro != "Admin")
            {
                MessageBox.Show("Chỉ Admin mới được sửa nhân viên.");
                return;
            }

            if (maNhanVienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.");
                return;
            }

            if (!KiemTraDuLieu(
                out string ten,
                out string gioiTinh,
                out DateTime ngaySinh,
                out string sdt,
                out string diaChi,
                out string chucVu))
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                NhanVien? nv = db.NhanViens.FirstOrDefault(
                    n => n.MaNhanVien == maNhanVienDangChon);

                if (nv == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên cần sửa.");
                    TaiDanhSachNhanVien();
                    return;
                }

                if (PhaiDungVaoVoiTrung(db, ten, sdt, maNhanVienDangChon))
                    return;

                nv.TenNhanVien = ten;
                nv.GioiTinh = gioiTinh;
                nv.NgaySinh = ngaySinh;
                nv.SoDienThoai = sdt;
                nv.DiaChi = string.IsNullOrWhiteSpace(diaChi) ? null : diaChi;
                nv.ChucVu = string.IsNullOrWhiteSpace(chucVu) ? null : chucVu;

                db.SaveChanges();

                MessageBox.Show("Cập nhật nhân viên thành công.");
                LamMoiForm();
                TaiDanhSachNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật nhân viên.\n" + ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            if (VaiTro != "Admin")
            {
                MessageBox.Show("Chỉ Admin mới được xóa nhân viên.");
                return;
            }

            if (maNhanVienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.");
                return;
            }

            if (MessageBox.Show(
                "Bạn có chắc chắn muốn xóa nhân viên này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using AppDbContext db = new AppDbContext();

                NhanVien? nv = db.NhanViens.FirstOrDefault(
                    n => n.MaNhanVien == maNhanVienDangChon);

                if (nv == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên cần xóa.");
                    TaiDanhSachNhanVien();
                    return;
                }

                bool coHoaDon = db.HoaDons.Any(
                    hd => hd.MaNhanVien == maNhanVienDangChon);

                bool coPhieuNhap = db.PhieuNhaps.Any(
                    pn => pn.MaNhanVien == maNhanVienDangChon);

                if (coHoaDon || coPhieuNhap)
                {
                    MessageBox.Show(
                        "Không thể xóa nhân viên đã có hóa đơn hoặc phiếu nhập.",
                        "Không thể xóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                db.NhanViens.Remove(nv);
                db.SaveChanges();

                MessageBox.Show("Xóa nhân viên thành công.");
                LamMoiForm();
                TaiDanhSachNhanVien();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Không thể xóa nhân viên vì dữ liệu đang được sử dụng ở bảng khác.",
                    "Không thể xóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa nhân viên.\n" + ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object? sender, EventArgs e)
        {
            LamMoiForm();
            TaiDanhSachNhanVien();
        }

        private void LamMoiForm()
        {
            maNhanVienDangChon = 0;

            OTenNhanVien?.Clear();
            OSdt?.Clear();
            OChucVu?.Clear();
            ODiaChi?.Clear();
            OTimKiem?.Clear();

            RadioButton? rNam = RNam;
            RadioButton? rNu = RNu;

            if (rNam != null) rNam.Checked = false;
            if (rNu != null) rNu.Checked = false;

            DateTimePicker? ngaySinhPicker = ONgaySinh;

            if (ngaySinhPicker != null)
            {
                DateTime macDinh = DateTime.Today.AddYears(-18);

                ngaySinhPicker.Value = macDinh < ngaySinhPicker.MinDate
                    ? ngaySinhPicker.MinDate
                    : macDinh;
            }

            LuoiNhanVien?.ClearSelection();
        }

        private void btnTimKiem_Click(object? sender, EventArgs e)
        {
            TaiDanhSachNhanVien(OTimKiem?.Text.Trim());
        }

        private void dgvNhanVien_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            DataGridView? luoi = LuoiNhanVien;

            if (dangNapDuLieu || e.RowIndex < 0 || luoi == null)
                return;

            DataGridViewRow dong = luoi.Rows[e.RowIndex];

            if (dong.Cells["MaNhanVien"].Value == null)
                return;

            maNhanVienDangChon =
                Convert.ToInt32(dong.Cells["MaNhanVien"].Value);

            TextBox? oTen = OTenNhanVien;

            if (oTen != null)
                oTen.Text =
                    dong.Cells["TenNhanVien"].Value?.ToString() ?? string.Empty;

            string gioiTinh =
                dong.Cells["GioiTinh"].Value?.ToString() ?? string.Empty;

            RadioButton? rNam = RNam;
            RadioButton? rNu = RNu;

            if (rNam != null) rNam.Checked = gioiTinh == "Nam";
            if (rNu != null) rNu.Checked = gioiTinh == "Nữ";

            TextBox? oSdt = OSdt;

            if (oSdt != null)
                oSdt.Text =
                    dong.Cells["SoDienThoai"].Value?.ToString() ?? string.Empty;

            TextBox? oChucVu = OChucVu;

            if (oChucVu != null)
                oChucVu.Text =
                    dong.Cells["ChucVu"].Value?.ToString() ?? string.Empty;

            TextBox? oDiaChi = ODiaChi;

            if (oDiaChi != null)
                oDiaChi.Text =
                    dong.Cells["DiaChi"].Value?.ToString() ?? string.Empty;

            DateTimePicker? ngaySinhPicker = ONgaySinh;
            string? chuoiNgaySinh = dong.Cells["NgaySinh"].Value?.ToString();

            if (ngaySinhPicker != null &&
                chuoiNgaySinh != null &&
                DateTime.TryParse(chuoiNgaySinh, out DateTime ngaySinh))
            {
                ngaySinhPicker.Value = ngaySinh < ngaySinhPicker.MinDate
                    ? ngaySinhPicker.MinDate
                    : ngaySinh > ngaySinhPicker.MaxDate
                        ? ngaySinhPicker.MaxDate
                        : ngaySinh;
            }
        }
    }
}