using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormDangNhap : Form
    {
        public string VaiTro { get; private set; } = string.Empty;

        public string TenDangNhap { get; private set; } = string.Empty;

        public FormDangNhap()
        {
            InitializeComponent();

            textBox2.UseSystemPasswordChar = true;
            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = textBox1.Text.Trim();
            string matKhau = textBox2.Text;

            if (string.IsNullOrWhiteSpace(taiKhoan) ||
                string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show(
                    "Bạn vui lòng nhập đầy đủ tên tài khoản và mật khẩu!",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.Equals(
                    taiKhoan,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase)
                && matKhau == "123456")
            {
                VaiTro = "Admin";
                TenDangNhap = "Admin";

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    var taiKhoanDb = db.TaiKhoans
                        .AsNoTracking()
                        .FirstOrDefault(x => x.TenDangNhap == taiKhoan);

                    if (taiKhoanDb == null || taiKhoanDb.MatKhau != matKhau)
                    {
                        MessageBox.Show(
                            "Tên tài khoản hoặc mật khẩu không đúng!",
                            "Đăng nhập thất bại",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        textBox2.Clear();
                        textBox2.Focus();
                        return;
                    }

                    VaiTro = taiKhoanDb.VaiTro;
                    TenDangNhap = taiKhoanDb.TenDangNhap;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kết nối hoặc đọc dữ liệu tài khoản từ SQL Server.\nChi tiết: " + ex.Message,
                    "Lỗi đăng nhập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}