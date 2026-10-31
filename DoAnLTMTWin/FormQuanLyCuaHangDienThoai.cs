using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormQuanLyCuaHangDienThoai : Form
    {
        private string vaiTro = string.Empty;
        private string tenDangNhap = string.Empty;
        private int? maTaiKhoanDangNhap;

        private readonly Dictionary<Type, Form> cacFormDangMo = new();
        private Panel? panelNoiDung;
        private readonly List<Control> cacControlTrangChu = new();

        private bool DaDangNhap =>
            !string.IsNullOrWhiteSpace(vaiTro);

        private bool LaAdmin =>
            string.Equals(vaiTro?.Trim(), "Admin",
                StringComparison.OrdinalIgnoreCase);

        private bool LaAdminTamThoi =>
            string.Equals(vaiTro?.Trim(), "AdminTamThoi",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(vaiTro?.Trim(), "Admin (Tạm thời)",
                StringComparison.OrdinalIgnoreCase);

        private bool LaNhanVien =>
            string.Equals(vaiTro?.Trim(), "Nhân viên",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(vaiTro?.Trim(), "NhanVien",
                StringComparison.OrdinalIgnoreCase);

        private bool LaKhachHang =>
            string.Equals(vaiTro?.Trim(), "Khách hàng",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(vaiTro?.Trim(), "KhachHang",
                StringComparison.OrdinalIgnoreCase);

        public FormQuanLyCuaHangDienThoai()
        {
            InitializeComponent();

            label3.Text = "Xin chào!";
            btnDangNhap.Visible = true;
            btnDangKy.Visible = true;
            btnQuanLyTaiKhoan.Visible = false;

            Load += FormQuanLyCuaHangDienThoai_Load;
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

        private void FormQuanLyCuaHangDienThoai_Load(
            object? sender, EventArgs e)
        {
            TimPanelNoiDung();

            cacControlTrangChu.Clear();

            if (panelNoiDung != null)
            {
                foreach (Control c in panelNoiDung.Controls)
                {
                    if (c is not Form)
                        cacControlTrangChu.Add(c);
                }
            }

            CapNhatGiaoDienTaiKhoan();
            CapNhatThongKe();
        }

        private void TimPanelNoiDung()
        {
            var controls = LayTatCaControl(this).ToList();

            panelNoiDung = controls
                .OfType<Panel>()
                .Where(p => p.Visible)
                .Where(p => p.Width >= ClientSize.Width / 2)
                .Where(p => p.Left > 100)
                .OrderByDescending(p => p.Width * p.Height)
                .FirstOrDefault();

            if (panelNoiDung == null)
            {
                Control[] labels =
                {
                    label8, label11, label14, label17
                };

                panelNoiDung = controls
                    .OfType<Panel>()
                    .Where(p => labels.All(l => p == l || p.Contains(l)))
                    .OrderByDescending(p => p.Width * p.Height)
                    .FirstOrDefault();
            }
        }

        private void CapNhatGiaoDienTaiKhoan()
        {
            btnDangNhap.Visible = !DaDangNhap;
            btnDangKy.Visible = !DaDangNhap;
            btnQuanLyTaiKhoan.Visible = LaAdmin;

            label3.Text = DaDangNhap
                ? "Xin chào " + tenDangNhap +
                  " (" + ChuanHoaVaiTro(vaiTro) + ")!"
                : "Xin chào!";
        }

        private string ChuanHoaVaiTro(string role)
        {
            if (string.Equals(role, "AdminTamThoi",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Admin (Tạm thời)",
                StringComparison.OrdinalIgnoreCase))
                return "Admin (Tạm thời)";

            if (string.Equals(role, "NhanVien",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Nhân viên",
                StringComparison.OrdinalIgnoreCase))
                return "Nhân viên";

            if (string.Equals(role, "KhachHang",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Khách hàng",
                StringComparison.OrdinalIgnoreCase))
                return "Khách hàng";

            return role;
        }

        private void DatThongKeVeKhong()
        {
            label8.Text = "0";
            label11.Text = "0";
            label14.Text = "0";
            label17.Text = "0";
        }


        private void CapNhatThongKe()
        {
            try
            {
                DateTime homNay = DateTime.Today;
                DateTime ngayMai = homNay.AddDays(1);

                using var db = new AppDbContext();

                // 1. Tổng số điện thoại
                int tongDienThoai = db.DienThoais.Count();

                // 2. Số hóa đơn được lập hôm nay
                int hoaDonHomNay = db.HoaDons.Count(h =>
                    h.NgayLap >= homNay &&
                    h.NgayLap < ngayMai);

                // 3. Tổng số khách hàng
                int tongKhachHang = db.KhachHangs.Count();

                // 4. Tổng doanh thu hôm nay
                // Chỉ tính hóa đơn đã thanh toán
                decimal doanhThuHomNay = db.HoaDons
                    .Where(h =>
                        h.NgayLap >= homNay &&
                        h.NgayLap < ngayMai &&
                        h.TrangThai == "Đã thanh toán")
                    .Sum(h => (decimal?)h.TongTien) ?? 0m;

                // 5. Hiển thị lên giao diện trang chủ
                label8.Text = tongDienThoai.ToString("N0");
                label11.Text = hoaDonHomNay.ToString("N0");
                label14.Text = tongKhachHang.ToString("N0");
                label17.Text = doanhThuHomNay.ToString("N0");
            }
            catch (Exception ex)
            {
                DatThongKeVeKhong();

                MessageBox.Show(
                    "Không thể tải thống kê từ cơ sở dữ liệu.\n\n" +
                    ex.GetBaseException().Message,
                    "Lỗi thống kê",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool KiemTraDaDangNhap()
        {
            if (DaDangNhap)
                return true;

            MessageBox.Show(
                "Bạn cần đăng nhập để sử dụng chức năng này!",
                "Chưa đăng nhập",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private bool KiemTraQuyen(string chucNang)
        {
            if (!KiemTraDaDangNhap())
                return false;

            if (LaAdmin)
                return true;

            if (LaAdminTamThoi)
            {
                if (chucNang == "Quản lý tài khoản")
                {
                    MessageBox.Show(
                        "Admin (Tạm thời) không được quản lý tài khoản.",
                        "Không đủ quyền",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return false;
                }

                return true;
            }

            if (LaNhanVien)
                return true;

            if (LaKhachHang)
            {
                bool duocXem =
                    chucNang == "Điện thoại" ||
                    chucNang == "Loại điện thoại";

                if (duocXem)
                    return true;

                MessageBox.Show(
                    "Khách hàng chỉ được xem Điện thoại và Loại điện thoại.",
                    "Không đủ quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            MessageBox.Show(
                "Vai trò tài khoản không hợp lệ hoặc chưa được cấp quyền.",
                "Không đủ quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private bool KiemTraQuyenAdmin()
        {
            if (LaAdmin)
                return true;

            MessageBox.Show(
                "Chỉ Admin hệ thống mới được quản lý tài khoản.",
                "Không đủ quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private void AnGiaoDienTrangChu()
        {
            foreach (Control c in cacControlTrangChu)
            {
                if (!c.IsDisposed)
                    c.Visible = false;
            }
        }

        private void HienGiaoDienTrangChu()
        {
            foreach (Form f in cacFormDangMo.Values.ToList())
            {
                if (!f.IsDisposed)
                    f.Close();
            }

            cacFormDangMo.Clear();

            if (panelNoiDung != null && !panelNoiDung.IsDisposed)
            {
                foreach (Form f in panelNoiDung.Controls
                    .OfType<Form>().ToList())
                {
                    panelNoiDung.Controls.Remove(f);
                    f.Dispose();
                }
            }

            foreach (Control c in cacControlTrangChu)
            {
                if (!c.IsDisposed)
                    c.Visible = true;
            }

            CapNhatGiaoDienTaiKhoan();
            CapNhatThongKe();
        }

        private void MoForm(Form formMoi, string chucNang)
        {
            if (!KiemTraQuyen(chucNang))
            {
                formMoi.Dispose();
                return;
            }

            if (panelNoiDung == null || panelNoiDung.IsDisposed)
            {
                formMoi.Dispose();

                MessageBox.Show(
                    "Không tìm thấy vùng nội dung để hiển thị form.",
                    "Lỗi giao diện",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (formMoi is FormDienThoai fdt)
                fdt.VaiTroNguoiDung = vaiTro;

            if (formMoi is FormLoaiDienThoai fl)
                fl.VaiTroNguoiDung = vaiTro;

            if (formMoi is FormKhachHang fkh)
                fkh.VaiTroNguoiDung = vaiTro;

            if (formMoi is FormNhanVien fnv)
                fnv.VaiTro = vaiTro;

            Type loaiForm = formMoi.GetType();

            if (cacFormDangMo.TryGetValue(loaiForm, out Form? formCu))
            {
                if (!formCu.IsDisposed)
                {
                    formMoi.Dispose();
                    formCu.BringToFront();
                    formCu.Focus();
                    return;
                }

                cacFormDangMo.Remove(loaiForm);
            }

            AnGiaoDienTrangChu();

            formMoi.TopLevel = false;
            formMoi.FormBorderStyle = FormBorderStyle.None;
            formMoi.Dock = DockStyle.Fill;

            panelNoiDung.Controls.Add(formMoi);
            cacFormDangMo[loaiForm] = formMoi;

            formMoi.FormClosed += (s, e) =>
            {
                cacFormDangMo.Remove(loaiForm);

                if (panelNoiDung != null &&
                    !panelNoiDung.IsDisposed &&
                    panelNoiDung.Controls.Contains(formMoi))
                {
                    panelNoiDung.Controls.Remove(formMoi);
                }

                if (cacFormDangMo.Count == 0)
                {
                    foreach (Control c in cacControlTrangChu)
                    {
                        if (!c.IsDisposed)
                            c.Visible = true;
                    }

                    CapNhatGiaoDienTaiKhoan();
                    CapNhatThongKe();
                }
            };

            formMoi.Show();
            formMoi.BringToFront();
        }

        // Mở Form bán hàng và truyền mã điện thoại được chọn.
        public void MoFormBanHang(int maDienThoai)
        {
            if (!KiemTraQuyen("Bán hàng"))
                return;

            // Mở hoặc đưa Form bán hàng đang mở lên trước.
            MoForm(new FormBanHang(), "Bán hàng");

            // Lấy đúng Form bán hàng đang được quản lý.
            if (cacFormDangMo.TryGetValue(
                typeof(FormBanHang), out Form? form))
            {
                if (form is FormBanHang formBanHang &&
                    !formBanHang.IsDisposed)
                {
                    formBanHang.ThemDienThoaiTuDanhSach(maDienThoai);
                    formBanHang.BringToFront();
                }
            }
        }

        private void MoFormQuanLyTaiKhoan()
        {
            if (!KiemTraQuyenAdmin())
                return;

            if (panelNoiDung == null || panelNoiDung.IsDisposed)
            {
                MessageBox.Show(
                    "Không tìm thấy vùng nội dung.",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            Type loaiForm = typeof(FormQuanLyTaiKhoan);

            if (cacFormDangMo.TryGetValue(loaiForm, out Form? cu) &&
                !cu.IsDisposed)
            {
                cu.BringToFront();
                return;
            }

            var form = new FormQuanLyTaiKhoan
            {
                VaiTroNguoiDung = vaiTro,
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            AnGiaoDienTrangChu();

            form.YeuCauVeTrangChu += HienGiaoDienTrangChu;

            form.FormClosed += (s, e) =>
            {
                cacFormDangMo.Remove(loaiForm);

                if (panelNoiDung != null &&
                    !panelNoiDung.IsDisposed &&
                    panelNoiDung.Controls.Contains(form))
                {
                    panelNoiDung.Controls.Remove(form);
                }

                if (cacFormDangMo.Count == 0)
                {
                    foreach (Control c in cacControlTrangChu)
                    {
                        if (!c.IsDisposed)
                            c.Visible = true;
                    }

                    CapNhatGiaoDienTaiKhoan();
                    CapNhatThongKe();
                }
            };

            panelNoiDung.Controls.Add(form);
            cacFormDangMo[loaiForm] = form;

            form.Show();
            form.BringToFront();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if (DaDangNhap)
            {
                MessageBox.Show(
                    "Bạn đang đăng nhập bằng tài khoản " +
                    tenDangNhap + ".",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using var form = new FormDangNhap();

            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            tenDangNhap = form.TenDangNhap;
            vaiTro = form.VaiTro;

            if (string.IsNullOrWhiteSpace(vaiTro))
            {
                tenDangNhap = string.Empty;

                MessageBox.Show(
                    "Form đăng nhập chưa trả về vai trò tài khoản.",
                    "Lỗi đăng nhập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (LaAdmin)
            {
                maTaiKhoanDangNhap = null;
            }
            else
            {
                try
                {
                    using var db = new AppDbContext();

                    var tk = db.TaiKhoans
                        .AsNoTracking()
                        .FirstOrDefault(x =>
                            x.TenDangNhap == tenDangNhap);

                    if (tk == null)
                    {
                        vaiTro = string.Empty;
                        tenDangNhap = string.Empty;
                        maTaiKhoanDangNhap = null;

                        MessageBox.Show(
                            "Không tìm thấy tài khoản trong cơ sở dữ liệu. " +
                            "Hãy kiểm tra tên đăng nhập và database mà FormDangNhap sử dụng.",
                            "Lỗi đăng nhập",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    vaiTro = tk.VaiTro;
                    maTaiKhoanDangNhap = tk.MaTaiKhoan;
                }
                catch (Exception ex)
                {
                    vaiTro = string.Empty;
                    tenDangNhap = string.Empty;
                    maTaiKhoanDangNhap = null;

                    MessageBox.Show(
                        "Không thể kiểm tra tài khoản trong cơ sở dữ liệu.\n" +
                        ex.GetBaseException().Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }
            }

            CapNhatGiaoDienTaiKhoan();
            CapNhatThongKe();
        }

        private void btnDienThoai_Click(object sender, EventArgs e)
            => MoForm(new FormDienThoai(), "Điện thoại");

        private void btnLoaiDienThoai_Click(object sender, EventArgs e)
            => MoForm(new FormLoaiDienThoai(), "Loại điện thoại");

        private void btnKhachHang_Click(object sender, EventArgs e)
            => MoForm(new FormKhachHang(), "Khách hàng");

        private void btnNhanVien_Click(object sender, EventArgs e)
            => MoForm(new FormNhanVien(vaiTro), "Nhân viên");

        private void btnNhaCungCap_Click(object sender, EventArgs e)
            => MoForm(new FormNhaCungCap(), "Nhà cung cấp");

        private void btnNhapHang_Click(object sender, EventArgs e)
            => MoForm(new FormNhapHang(), "Nhập hàng");

        private void btnBanHang_Click(object sender, EventArgs e)
            => MoForm(new FormBanHang(), "Bán hàng");

        private void btnThongKe_DoanhThu_Click(object sender, EventArgs e)
            => MoForm(new FormThongKe_DoanhThu(), "Thống kê-Doanh thu");

        private void btnQuanLyTaiKhoan_Click(object sender, EventArgs e)
            => MoFormQuanLyTaiKhoan();

        private void btnTrangChu_Click(object sender, EventArgs e)
            => HienGiaoDienTrangChu();

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            if (DaDangNhap)
            {
                MessageBox.Show(
                    "Bạn cần đăng xuất trước khi đăng ký tài khoản mới.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using var form = new FormDangKy();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            if (!DaDangNhap)
            {
                MessageBox.Show(
                    "Bạn chưa đăng nhập!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            HienGiaoDienTrangChu();

            vaiTro = string.Empty;
            tenDangNhap = string.Empty;
            maTaiKhoanDangNhap = null;

            CapNhatGiaoDienTaiKhoan();
            DatThongKeVeKhong();
        }
    }
}