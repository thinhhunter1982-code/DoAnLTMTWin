
using System;
using System.Linq;
using System.Windows.Forms;

namespace DoAnLTMTWin
{
    public partial class FormThanhToanQR : Form
    {
        private int maHoaDon;
        private decimal tongTien;

        public FormThanhToanQR(int maHoaDon, decimal tongTien)
        {
            InitializeComponent();

            this.maHoaDon = maHoaDon;
            this.tongTien = tongTien;

            lblMaHoaDon.Text = maHoaDon.ToString();
            lblTongTien.Text = tongTien.ToString("N0") + " VNĐ";
            lblTrangThai.Text = "Chờ thanh toán";
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var hoaDon = db.HoaDons
                    .FirstOrDefault(h => h.MaHoaDon == maHoaDon);

                if (hoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!");
                    return;
                }

                if (hoaDon.TrangThai == "Đã thanh toán")
                {
                    MessageBox.Show("Hóa đơn đã được thanh toán!");
                    return;
                }

                hoaDon.TrangThai = "Đã thanh toán";
                db.SaveChanges();

                lblTrangThai.Text = "Đã thanh toán";

                MessageBox.Show("Thanh toán thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                btnThanhToan.Enabled = false;
            }
        }

        private void btnDong_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
