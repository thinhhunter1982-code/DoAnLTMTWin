using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormNhaCungCap : Form
    {
        private string VaiTro = "KhachHang";
        private int? maDangChon;

        private TextBox OTen = null!;
        private TextBox OSdt = null!;
        private TextBox OEmail = null!;
        private TextBox ODiaChi = null!;
        private DataGridView Bang = null!;

        public FormNhaCungCap() : this("Admin")
        {
        }

        public FormNhaCungCap(string vaiTro)
        {
            InitializeComponent();

            VaiTro = vaiTro ?? "KhachHang";

            TimDieuKhien();
            GanSuKien();
            PhanQuyen();
            Load += FormNhaCungCap_Load;
        }

        private IEnumerable<Control> TimTatCaControls(Control cha)
        {
            foreach (Control c in cha.Controls)
            {
                yield return c;

                foreach (Control con in TimTatCaControls(c))
                    yield return con;
            }
        }

        private static TextBox? TimTheoTen(
            List<TextBox> danhSach,
            params string[] ten)
        {
            return danhSach.FirstOrDefault(t => ten.Contains(t.Name));
        }

        private void TimDieuKhien()
        {
            List<Control> tatCa = TimTatCaControls(this).ToList();
            List<TextBox> cacTextBox = tatCa.OfType<TextBox>().ToList();

            Bang = tatCa.OfType<DataGridView>()
                       .FirstOrDefault(d => new[]
                       {
                           "dataGridView1",
                           "dgvNhaCungCap",
                           "dgvDanhSachNhaCungCap"
                       }.Contains(d.Name))
                   ?? tatCa.OfType<DataGridView>().FirstOrDefault()
                   ?? throw new InvalidOperationException(
                       "Không tìm thấy DataGridView trên form.");

            TextBox? ten = TimTheoTen(
                cacTextBox,
                "txtTenNhaCungCap", "txtTenNCC", "txtTen");

            TextBox? sdt = TimTheoTen(
                cacTextBox,
                "txtSoDienThoai", "txtSDT", "txtDienThoai");

            TextBox? email = TimTheoTen(
                cacTextBox,
                "txtEmail");

            TextBox? diaChi = TimTheoTen(
                cacTextBox,
                "txtDiaChi");

            List<TextBox> daGan = new List<TextBox?> { ten, sdt, email, diaChi }
                .Where(t => t != null)
                .Select(t => t!)
                .ToList();

            Queue<TextBox> conLai = new Queue<TextBox>(
                cacTextBox
                    .Where(t => !daGan.Contains(t))
                    .OrderBy(t => t.Top)
                    .ThenBy(t => t.Left));

            ten ??= conLai.Count > 0 ? conLai.Dequeue() : null;
            sdt ??= conLai.Count > 0 ? conLai.Dequeue() : null;
            email ??= conLai.Count > 0 ? conLai.Dequeue() : null;
            diaChi ??= conLai.Count > 0 ? conLai.Dequeue() : null;

            if (ten == null || sdt == null || email == null || diaChi == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy đủ 4 TextBox (tên, số điện thoại, " +
                    "email, địa chỉ). " +
                    "Hãy kiểm tra thuộc tính Name trong Designer.");
            }

            OTen = ten;
            OSdt = sdt;
            OEmail = email;
            ODiaChi = diaChi;
        }

        private void GanSuKien()
        {
            btnThem.Click -= btnThem_Click;
            btnThem.Click += btnThem_Click;

            btnSua.Click -= btnSua_Click;
            btnSua.Click += btnSua_Click;

            btnXoa.Click -= btnXoa_Click;
            btnXoa.Click += btnXoa_Click;

            btnLamMoi.Click -= btnLamMoi_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            btnTimKiem.Click -= btnTimKiem_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            Bang.CellClick -= Bang_CellClick;
            Bang.CellClick += Bang_CellClick;
        }

        private void PhanQuyen()
        {
            bool laAdmin = string.Equals(
                VaiTro, "Admin",
                StringComparison.OrdinalIgnoreCase);

            btnThem.Enabled = laAdmin;
            btnSua.Enabled = laAdmin;
            btnXoa.Enabled = laAdmin;

            btnLamMoi.Enabled = true;
            btnTimKiem.Enabled = true;

            OTen.ReadOnly = !laAdmin;
            OSdt.ReadOnly = !laAdmin;
            OEmail.ReadOnly = !laAdmin;
            ODiaChi.ReadOnly = !laAdmin;
        }

        private void FormNhaCungCap_Load(object? sender, EventArgs e)
        {
            try
            {
                TaiDanhSach();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách nhà cung cấp.\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int TaiDanhSach(string tuKhoa = "")
        {
            using var db = new AppDbContext();

            var query = db.NhaCungCaps
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(n =>
                    n.TenNhaCungCap.Contains(tuKhoa) ||
                    (n.SoDienThoai != null &&
                     n.SoDienThoai.Contains(tuKhoa)) ||
                    (n.Email != null &&
                     n.Email.Contains(tuKhoa)) ||
                    (n.DiaChi != null &&
                     n.DiaChi.Contains(tuKhoa)));
            }

            var danhSach = query
                .OrderBy(n => n.MaNhaCungCap)
                .Select(n => new
                {
                    n.MaNhaCungCap,
                    n.TenNhaCungCap,
                    n.SoDienThoai,
                    n.Email,
                    n.DiaChi
                })
                .ToList();

            Bang.DataSource = null;
            Bang.DataSource = danhSach;

            Bang.ReadOnly = true;
            Bang.AllowUserToAddRows = false;
            Bang.AllowUserToDeleteRows = false;
            Bang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Bang.MultiSelect = false;
            Bang.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            maDangChon = null;
            Bang.ClearSelection();

            return danhSach.Count;
        }

        private bool LaAdmin()
        {
            if (string.Equals(
                VaiTro, "Admin",
                StringComparison.OrdinalIgnoreCase))
                return true;

            MessageBox.Show(
                "Tài khoản Khách hàng không có quyền thực hiện chức năng này!",
                "Thông báo", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private static string ChuanHoaSdt(string sdt)
        {
            return Regex.Replace(
                sdt,
                @"[\s\.\-\(\)\u00A0\u200B\u200C\u200D\uFEFF]",
                "");
        }

        private bool KiemTraDuLieu(
            out string ten, out string sdt,
            out string email, out string diaChi)
        {
            ten = OTen.Text.Trim();
            sdt = ChuanHoaSdt(OSdt.Text);
            email = OEmail.Text.Trim();
            diaChi = ODiaChi.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp.");
                OTen.Focus();
                return false;
            }

            if (!Regex.IsMatch(sdt, @"^0[0-9]{9}$"))
            {
                MessageBox.Show(
                    "Số điện thoại phải có đúng 10 chữ số và bắt đầu bằng số 0.\n\n" +
                    $"Giá trị đọc được: \"{sdt}\" ({sdt.Length} ký tự)");
                OSdt.Focus();
                OSdt.SelectAll();
                return false;
            }

            OSdt.Text = sdt;

            if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@gmail\.com$",
                RegexOptions.IgnoreCase))
            {
                MessageBox.Show(
                    "Email phải hợp lệ và có đuôi @gmail.com, " +
                    "ví dụ: tenban@gmail.com");
                OEmail.Focus();
                OEmail.SelectAll();
                return false;
            }

            if (string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ nhà cung cấp.");
                ODiaChi.Focus();
                return false;
            }

            return true;
        }

        private bool KiemTraTrung(
            AppDbContext db, string ten, string sdt,
            string email, int maBoQua = 0)
        {
            var query = db.NhaCungCaps
                .AsNoTracking()
                .Where(n => n.MaNhaCungCap != maBoQua);

            if (query.Any(n => n.TenNhaCungCap == ten))
            {
                MessageBox.Show("Tên nhà cung cấp đã tồn tại.");
                return true;
            }

            if (query.Any(n => n.SoDienThoai == sdt))
            {
                MessageBox.Show("Số điện thoại đã được sử dụng.");
                return true;
            }

            if (query.Any(n => n.Email == email))
            {
                MessageBox.Show("Email đã được sử dụng.");
                return true;
            }

            return false;
        }

        private static int TimMaTrong(AppDbContext db)
        {
            List<int> cacMa = db.NhaCungCaps
                .AsNoTracking()
                .Select(n => n.MaNhaCungCap)
                .OrderBy(m => m)
                .ToList();

            int ma = 1;

            foreach (int m in cacMa)
            {
                if (m == ma)
                    ma++;
                else if (m > ma)
                    break;
            }

            return ma;
        }

        private static void ThemVaoMaTrong(
            AppDbContext db, NhaCungCap nhaCungCap)
        {
            var entityType = db.Model.FindEntityType(typeof(NhaCungCap));
            string tenBang = entityType?.GetTableName() ?? "NhaCungCap";
            string schema = entityType?.GetSchema() ?? "dbo";

            string bat = $"SET IDENTITY_INSERT [{schema}].[{tenBang}] ON";
            string tat = $"SET IDENTITY_INSERT [{schema}].[{tenBang}] OFF";

            nhaCungCap.MaNhaCungCap = TimMaTrong(db);

            using var giaoDich = db.Database.BeginTransaction();

            try
            {
                db.Database.ExecuteSqlRaw(bat);

                db.NhaCungCaps.Add(nhaCungCap);
                db.SaveChanges();

                db.Database.ExecuteSqlRaw(tat);
                giaoDich.Commit();
            }
            catch
            {
                try
                {
                    db.Database.ExecuteSqlRaw(tat);
                }
                catch
                {
                }

                throw;
            }
        }

        private void Bang_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || Bang.Rows[e.RowIndex].IsNewRow)
                return;

            var row = Bang.Rows[e.RowIndex];

            if (row.Cells["MaNhaCungCap"].Value == null)
                return;

            maDangChon = Convert.ToInt32(row.Cells["MaNhaCungCap"].Value);

            OTen.Text = row.Cells["TenNhaCungCap"].Value?.ToString() ?? "";
            OSdt.Text = row.Cells["SoDienThoai"].Value?.ToString() ?? "";
            OEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            ODiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
        }

        private void btnThem_Click(object? sender, EventArgs e)
        {
            if (!LaAdmin()) return;

            if (!KiemTraDuLieu(
                out string ten, out string sdt,
                out string email, out string diaChi))
                return;

            try
            {
                using var db = new AppDbContext();

                if (KiemTraTrung(db, ten, sdt, email))
                    return;

                var nhaCungCap = new NhaCungCap
                {
                    TenNhaCungCap = ten,
                    SoDienThoai = sdt,
                    Email = email,
                    DiaChi = diaChi
                };

                ThemVaoMaTrong(db, nhaCungCap);

                MessageBox.Show(
                    "Thêm nhà cung cấp thành công! Mã nhà cung cấp: " +
                    nhaCungCap.MaNhaCungCap);

                TaiDanhSach();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi thêm nhà cung cấp:\n" +
                    ex.GetBaseException().Message);
            }
        }

        private void btnSua_Click(object? sender, EventArgs e)
        {
            if (!LaAdmin()) return;

            int? ma = maDangChon;

            if (ma == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.");
                return;
            }

            if (!KiemTraDuLieu(
                out string ten, out string sdt,
                out string email, out string diaChi))
                return;

            try
            {
                using var db = new AppDbContext();

                if (KiemTraTrung(db, ten, sdt, email, ma.Value))
                    return;

                var nhaCungCap = db.NhaCungCaps
                    .FirstOrDefault(n => n.MaNhaCungCap == ma.Value);

                if (nhaCungCap == null)
                {
                    MessageBox.Show("Nhà cung cấp không còn tồn tại.");
                    TaiDanhSach();
                    return;
                }

                nhaCungCap.TenNhaCungCap = ten;
                nhaCungCap.SoDienThoai = sdt;
                nhaCungCap.Email = email;
                nhaCungCap.DiaChi = diaChi;

                db.SaveChanges();

                MessageBox.Show("Cập nhật nhà cung cấp thành công!");
                TaiDanhSach();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi sửa nhà cung cấp:\n" +
                    ex.GetBaseException().Message);
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            if (!LaAdmin()) return;

            int? ma = maDangChon;

            if (ma == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.");
                return;
            }

            if (MessageBox.Show(
                "Bạn có chắc chắn muốn xóa nhà cung cấp này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();

                var nhaCungCap = db.NhaCungCaps
                    .Include(n => n.PhieuNhaps)
                    .FirstOrDefault(n => n.MaNhaCungCap == ma.Value);

                if (nhaCungCap == null)
                {
                    MessageBox.Show("Nhà cung cấp không còn tồn tại.");
                    TaiDanhSach();
                    return;
                }

                if (nhaCungCap.PhieuNhaps.Any())
                {
                    MessageBox.Show(
                        "Không thể xóa nhà cung cấp đã có phiếu nhập hàng.");
                    return;
                }

                db.NhaCungCaps.Remove(nhaCungCap);
                db.SaveChanges();

                MessageBox.Show("Xóa nhà cung cấp thành công!");
                TaiDanhSach();
                XoaTrang();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Không thể xóa nhà cung cấp vì dữ liệu đang được sử dụng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi xóa nhà cung cấp:\n" +
                    ex.GetBaseException().Message);
            }
        }

        private void btnLamMoi_Click(object? sender, EventArgs e)
        {
            XoaTrang();

            try
            {
                TaiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách:\n" + ex.Message);
            }
        }

        private void btnTimKiem_Click(object? sender, EventArgs e)
        {
            string tuKhoa = OTen.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = OSdt.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = OEmail.Text.Trim();

            if (tuKhoa == "")
                tuKhoa = ODiaChi.Text.Trim();

            try
            {
                int soKetQua = TaiDanhSach(tuKhoa);

                if (tuKhoa != "" && soKetQua == 0)
                    MessageBox.Show("Không tìm thấy nhà cung cấp.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm:\n" + ex.Message);
            }
        }

        private void XoaTrang()
        {
            maDangChon = null;

            OTen.Clear();
            OSdt.Clear();
            OEmail.Clear();
            ODiaChi.Clear();

            Bang.ClearSelection();
            OTen.Focus();
        }
    }
}