
using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormQuanLyTaiKhoan : Form
    {
        public event Action? YeuCauVeTrangChu;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string VaiTroNguoiDung { get; set; } = "";

        private int? maTaiKhoanDangChon;

        private bool LaAdmin =>
            string.Equals(
                VaiTroNguoiDung,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

        public FormQuanLyTaiKhoan()
        {
            InitializeComponent();

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Tất cả vai trò");
            comboBox1.Items.Add("Admin (Tạm thời)");
            comboBox1.Items.Add("Nhân viên");
            comboBox1.Items.Add("Khách hàng");
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.CellClick += dataGridView1_CellClick;
            btnTimKiem.Click += btnTimKiem_Click;
            btnCapNhat.Click += btnCapNhat_Click;
            btnXoa.Click += btnXoa_Click;
            btnThoat.Click += btnThoat_Click;
            Load += FormQuanLyTaiKhoan_Load;
        }

        private void FormQuanLyTaiKhoan_Load(object? sender, EventArgs e)
        {
            if (!LaAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin hệ thống mới được quản lý tài khoản.",
                    "Không có quyền truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                BeginInvoke(new Action(Close));
                return;
            }

            comboBox1.SelectedIndex = 0;
            TaiKhoan_LoadDanhSach();
        }

        private string ChuyenVaiTroHienThi(string? vaiTro)
        {
            if (string.IsNullOrWhiteSpace(vaiTro))
                return "Khách hàng";

            if (string.Equals(vaiTro, "AdminTamThoi",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vaiTro, "Admin (Tạm thời)",
                StringComparison.OrdinalIgnoreCase))
                return "Admin (Tạm thời)";

            if (string.Equals(vaiTro, "NhanVien",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vaiTro, "Nhân viên",
                StringComparison.OrdinalIgnoreCase))
                return "Nhân viên";

            return "Khách hàng";
        }

        private string ChuyenVaiTroLuu(string vaiTro)
        {
            return vaiTro switch
            {
                "Admin (Tạm thời)" => "AdminTamThoi",
                "Nhân viên" => "Nhân viên",
                "Khách hàng" => "Khách hàng",
                _ => "Khách hàng"
            };
        }

        private void CauHinhCot()
        {
            if (dataGridView1.Columns["MaTaiKhoan"] != null)
                dataGridView1.Columns["MaTaiKhoan"].HeaderText = "Mã tài khoản";

            if (dataGridView1.Columns["TenDangNhap"] != null)
                dataGridView1.Columns["TenDangNhap"].HeaderText = "Tên tài khoản";

            if (dataGridView1.Columns["HoTen"] != null)
                dataGridView1.Columns["HoTen"].HeaderText = "Họ tên";

            if (dataGridView1.Columns["SoDienThoai"] != null)
                dataGridView1.Columns["SoDienThoai"].HeaderText = "Số điện thoại";

            if (dataGridView1.Columns["VaiTro"] != null)
                dataGridView1.Columns["VaiTro"].HeaderText = "Vai trò";
        }

        private void TaiKhoan_LoadDanhSach()
        {
            try
            {
                using var db = new AppDbContext();

                var danhSach = db.TaiKhoans
                    .AsNoTracking()
                    .OrderBy(x => x.MaTaiKhoan)
                    .Select(x => new
                    {
                        x.MaTaiKhoan,
                        x.TenDangNhap,
                        x.HoTen,
                        x.SoDienThoai,
                        x.VaiTro
                    })
                    .ToList()
                    .Where(x =>
                        !string.Equals(x.VaiTro, "Admin",
                            StringComparison.OrdinalIgnoreCase))
                    .Select(x => new
                    {
                        x.MaTaiKhoan,
                        x.TenDangNhap,
                        x.HoTen,
                        x.SoDienThoai,
                        VaiTro = ChuyenVaiTroHienThi(x.VaiTro)
                    })
                    .ToList();

                dataGridView1.DataSource = danhSach;
                label5.Text = danhSach.Count.ToString();

                CauHinhCot();
                maTaiKhoanDangChon = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách tài khoản.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnTimKiem_Click(object? sender, EventArgs e)
        {
            try
            {
                string ma = textBox1.Text.Trim();
                string ten = textBox2.Text.Trim();
                string vaiTro = comboBox1.Text.Trim();

                using var db = new AppDbContext();

                var truyVan = db.TaiKhoans
                    .AsNoTracking()
                    .Where(x => x.VaiTro != "Admin")
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(ma))
                {
                    if (!int.TryParse(ma, out int maTaiKhoan))
                    {
                        MessageBox.Show(
                            "Mã tài khoản phải là số.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    truyVan = truyVan.Where(
                        x => x.MaTaiKhoan == maTaiKhoan);
                }

                if (!string.IsNullOrWhiteSpace(ten))
                {
                    truyVan = truyVan.Where(
                        x => x.TenDangNhap.Contains(ten));
                }

                if (vaiTro == "Admin (Tạm thời)")
                {
                    truyVan = truyVan.Where(
                        x => x.VaiTro == "AdminTamThoi" ||
                             x.VaiTro == "Admin (Tạm thời)");
                }
                else if (vaiTro == "Nhân viên")
                {
                    truyVan = truyVan.Where(
                        x => x.VaiTro == "Nhân viên" ||
                             x.VaiTro == "NhanVien");
                }
                else if (vaiTro == "Khách hàng")
                {
                    truyVan = truyVan.Where(
                        x => x.VaiTro == "Khách hàng" ||
                             x.VaiTro == "KhachHang");
                }

                var ketQua = truyVan
                    .OrderBy(x => x.MaTaiKhoan)
                    .Select(x => new
                    {
                        x.MaTaiKhoan,
                        x.TenDangNhap,
                        x.HoTen,
                        x.SoDienThoai,
                        x.VaiTro
                    })
                    .ToList()
                    .Select(x => new
                    {
                        x.MaTaiKhoan,
                        x.TenDangNhap,
                        x.HoTen,
                        x.SoDienThoai,
                        VaiTro = ChuyenVaiTroHienThi(x.VaiTro)
                    })
                    .ToList();

                dataGridView1.DataSource = ketQua;
                label5.Text = ketQua.Count.ToString();
                maTaiKhoanDangChon = null;

                CauHinhCot();

                if (ketQua.Count == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy tài khoản phù hợp.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Tìm kiếm thất bại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow dong = dataGridView1.Rows[e.RowIndex];

            if (dong.Cells["MaTaiKhoan"].Value == null)
                return;

            if (!int.TryParse(
                dong.Cells["MaTaiKhoan"].Value.ToString(),
                out int ma))
                return;

            maTaiKhoanDangChon = ma;
            textBox1.Text = ma.ToString();
            textBox2.Text =
                dong.Cells["TenDangNhap"].Value?.ToString() ?? "";

            string vaiTro =
                dong.Cells["VaiTro"].Value?.ToString() ?? "Khách hàng";

            int viTri = comboBox1.Items.IndexOf(vaiTro);

            if (viTri >= 1)
                comboBox1.SelectedIndex = viTri;
        }

        private void btnCapNhat_Click(object? sender, EventArgs e)
        {
            if (!LaAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin hệ thống được cập nhật tài khoản.");
                return;
            }

            if (maTaiKhoanDangChon == null)
            {
                MessageBox.Show(
                    "Bạn vui lòng chọn tài khoản trong bảng trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string vaiTroChon = comboBox1.Text.Trim();

            if (vaiTroChon == "Tất cả vai trò" ||
                string.IsNullOrWhiteSpace(vaiTroChon))
            {
                MessageBox.Show(
                    "Bạn vui lòng chọn một trong ba vai trò.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var taiKhoan = db.TaiKhoans.FirstOrDefault(
                    x => x.MaTaiKhoan == maTaiKhoanDangChon.Value);

                if (taiKhoan == null)
                {
                    MessageBox.Show("Tài khoản không còn tồn tại.");
                    TaiKhoan_LoadDanhSach();
                    return;
                }

                if (string.Equals(taiKhoan.VaiTro, "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Không thể thay đổi tài khoản Admin hệ thống tại đây.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                taiKhoan.VaiTro = ChuyenVaiTroLuu(vaiTroChon);
                db.SaveChanges();

                TaiKhoan_LoadDanhSach();

                MessageBox.Show(
                    "Đã cập nhật vai trò tài khoản.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Cập nhật tài khoản thất bại.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            if (!LaAdmin)
            {
                MessageBox.Show(
                    "Chỉ Admin hệ thống được xóa tài khoản.");
                return;
            }

            if (maTaiKhoanDangChon == null)
            {
                MessageBox.Show(
                    "Bạn vui lòng chọn tài khoản cần xóa trong bảng.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa tài khoản này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();

                int ma = maTaiKhoanDangChon.Value;

                var taiKhoan = db.TaiKhoans.FirstOrDefault(
                    x => x.MaTaiKhoan == ma);

                if (taiKhoan == null)
                {
                    MessageBox.Show("Tài khoản không còn tồn tại.");
                    TaiKhoan_LoadDanhSach();
                    return;
                }

                if (string.Equals(taiKhoan.VaiTro, "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Không thể xóa tài khoản Admin hệ thống tại đây.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                db.TaiKhoans.Remove(taiKhoan);
                db.SaveChanges();

                maTaiKhoanDangChon = null;
                textBox1.Clear();
                textBox2.Clear();
                comboBox1.SelectedIndex = 0;

                TaiKhoan_LoadDanhSach();

                MessageBox.Show(
                    "Đã xóa tài khoản.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa tài khoản.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object? sender, EventArgs e)
        {
            if (YeuCauVeTrangChu != null)
                YeuCauVeTrangChu.Invoke();
            else
                Close();
        }
    }
}
