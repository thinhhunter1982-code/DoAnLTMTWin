using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormDangKy : Form
    {
        public FormDangKy()
        {
            InitializeComponent();

            textBox2.UseSystemPasswordChar = true;
            textBox3.UseSystemPasswordChar = true;
        }

        private void btnTaoTaiKhoan_Click(object sender, EventArgs e)
        {
            string tenDangNhap = textBox1.Text.Trim();
            string matKhau = textBox2.Text;
            string xacNhanMatKhau = textBox3.Text;
            string hoTen = textBox4.Text.Trim();
            string soDienThoai = textBox5.Text.Trim();
            string diaChi = textBox6.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(xacNhanMatKhau) ||
                string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(soDienThoai) ||
                string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show(
                    "Bạn vui lòng nhập đầy đủ thông tin.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (tenDangNhap.Length > 50 ||
                matKhau.Length > 255 ||
                hoTen.Length > 100 ||
                soDienThoai.Length != 10 ||
                diaChi.Length > 200)
            {
                MessageBox.Show(
                    "Tên tài khoản, mật khẩu, họ tên hoặc địa chỉ vượt quá độ dài cho phép; số điện thoại phải có đúng 10 số.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (matKhau != xacNhanMatKhau)
            {
                MessageBox.Show(
                    "Mật khẩu xác nhận không khớp.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            if (matKhau.Length < 6)
            {
                MessageBox.Show(
                    "Mật khẩu phải có ít nhất 6 ký tự.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            if (soDienThoai[0] != '0' ||
                soDienThoai.Length != 10 ||
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Số điện thoại phải bắt đầu bằng số 0 và gồm đúng 10 chữ số, không chứa chữ hoặc ký tự đặc biệt.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    bool trungTenDangNhap = db.TaiKhoans
                        .Any(x => x.TenDangNhap == tenDangNhap);

                    if (trungTenDangNhap)
                    {
                        MessageBox.Show(
                            "Tên đăng nhập đã tồn tại. Bạn hãy chọn tên khác.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        textBox1.Focus();
                        return;
                    }

                    bool trungMatKhau = db.TaiKhoans
                        .Any(x => x.MatKhau == matKhau);

                    if (trungMatKhau)
                    {
                        MessageBox.Show(
                            "Mật khẩu này đã được sử dụng. Bạn hãy chọn mật khẩu khác.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        textBox2.Focus();
                        return;
                    }

                    bool trungSoDienThoai = db.TaiKhoans
                        .Any(x => x.SoDienThoai == soDienThoai);

                    if (trungSoDienThoai)
                    {
                        MessageBox.Show(
                            "Số điện thoại đã được sử dụng.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        textBox5.Focus();
                        return;
                    }

                    TaiKhoan taiKhoan = new TaiKhoan
                    {
                        TenDangNhap = tenDangNhap,
                        MatKhau = matKhau,
                        HoTen = hoTen,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi,
                        VaiTro = "KhachHang"
                    };

                    db.TaiKhoans.Add(taiKhoan);
                    db.SaveChanges();
                }

                MessageBox.Show(
                    "Đăng ký tài khoản thành công! Vai trò mặc định của bạn là KhachHang.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Không thể lưu tài khoản. Có thể tên đăng nhập hoặc số điện thoại đã được sử dụng.",
                    "Lỗi dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đăng ký thất bại: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnHien_An1_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar =
                !textBox2.UseSystemPasswordChar;
        }

        private void btnHien_An2_Click(object sender, EventArgs e)
        {
            textBox3.UseSystemPasswordChar =
                !textBox3.UseSystemPasswordChar;
        }
    }
}