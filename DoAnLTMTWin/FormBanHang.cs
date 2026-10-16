
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormBanHang : Form
    {
        private readonly List<ChiTietHoaDon> danhSachChiTiet = new();

        private int? maHoaDonDaLuu;
        private int? maDienThoaiDangSua;

        private Button? nutLuu;
        private string chuNutLuuGoc = "Cập nhật hóa đơn";

        public FormBanHang()
        {
            InitializeComponent();
            Load += FormBanHang_Load;
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

        private void FormBanHang_Load(object? sender, EventArgs e)
        {
            nutLuu = TimTatCaControls(this)
                .OfType<Button>()
                .FirstOrDefault(b => string.Equals(
                    b.Name,
                    "btnCapNhatHoaDon",
                    StringComparison.OrdinalIgnoreCase));

            if (nutLuu != null)
                chuNutLuuGoc = nutLuu.Text;

            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;

            CauHinhDataGridView();
            TaiDuLieuComboBox();
            CapNhatTongTien();
            CapNhatNutLuu();
        }

        private void CauHinhDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMaDienThoai",
                HeaderText = "Mã điện thoại",
                DataPropertyName = "MaDienThoai",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTenDienThoai",
                HeaderText = "Điện Thoại",
                DataPropertyName = "TenDienThoai",
                Width = 220
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số Lượng",
                DataPropertyName = "SoLuong",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonGia",
                HeaderText = "Đơn Giá",
                DataPropertyName = "DonGia",
                Width = 150,
                DefaultCellStyle = { Format = "N0" }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThanhTien",
                HeaderText = "Thành Tiền",
                DataPropertyName = "ThanhTien",
                Width = 170,
                DefaultCellStyle = { Format = "N0" }
            });
        }

        private void TaiDuLieuComboBox()
        {
            try
            {
                using var db = new AppDbContext();

                comboBox1.DataSource = db.KhachHangs
                    .AsNoTracking()
                    .OrderBy(x => x.TenKhachHang)
                    .ToList();

                comboBox1.DisplayMember = "TenKhachHang";
                comboBox1.ValueMember = "MaKhachHang";
                comboBox1.SelectedIndex = -1;

                comboBox2.DataSource = db.NhanViens
                    .AsNoTracking()
                    .OrderBy(x => x.TenNhanVien)
                    .ToList();

                comboBox2.DisplayMember = "TenNhanVien";
                comboBox2.ValueMember = "MaNhanVien";
                comboBox2.SelectedIndex = -1;

                comboBox3.DataSource = db.DienThoais
                    .AsNoTracking()
                    .OrderBy(x => x.TenDienThoai)
                    .ToList();

                comboBox3.DisplayMember = "TenDienThoai";
                comboBox3.ValueMember = "MaDienThoai";
                comboBox3.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu từ SQL Server.\n" +
                    ex.GetBaseException().Message,
                    "Lỗi tải dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatNutLuu()
        {
            if (nutLuu == null)
                return;

            nutLuu.Text = maHoaDonDaLuu == null
                ? chuNutLuuGoc
                : "Cập nhật hóa đơn";
        }

        private bool DocSoLuongDonGia(
            out int soLuong,
            out decimal donGia)
        {
            soLuong = 0;
            donGia = 0;

            if (!int.TryParse(textBox1.Text.Trim(), out soLuong)
                || soLuong <= 0)
            {
                MessageBox.Show(
                    "Số lượng phải là số nguyên lớn hơn 0.");
                textBox1.Focus();
                return false;
            }

            if (!decimal.TryParse(textBox2.Text.Trim(), out donGia)
                || donGia <= 0)
            {
                MessageBox.Show(
                    "Đơn giá phải lớn hơn 0.");
                textBox2.Focus();
                return false;
            }

            return true;
        }

        private void XoaONhapChiTiet()
        {
            maDienThoaiDangSua = null;
            comboBox3.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
        }

        private void dataGridView1_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= dataGridView1.Rows.Count)
                return;

            object? giaTri = dataGridView1.Rows[e.RowIndex]
                .Cells["colMaDienThoai"].Value;

            if (giaTri == null)
                return;

            int maDienThoai = Convert.ToInt32(giaTri);

            ChiTietHoaDon? dong = danhSachChiTiet
                .FirstOrDefault(x => x.MaDienThoai == maDienThoai);

            if (dong == null)
                return;

            maDienThoaiDangSua = dong.MaDienThoai;

            comboBox3.SelectedValue = dong.MaDienThoai;
            textBox1.Text = dong.SoLuong.ToString();
            textBox2.Text = dong.DonGia.ToString("0.##");
        }

        private void btnThemVaoHoaDon_Click(
            object? sender,
            EventArgs e)
        {
            if (comboBox3.SelectedItem is not DienThoai dienThoai)
            {
                MessageBox.Show("Vui lòng chọn điện thoại.");
                comboBox3.Focus();
                return;
            }

            if (!DocSoLuongDonGia(out int soLuong, out decimal donGia))
                return;

            if (maDienThoaiDangSua != null)
            {
                if (!ApDungSuaDongDangChon())
                    return;

                return;
            }

            if (danhSachChiTiet.Any(
                x => x.MaDienThoai == dienThoai.MaDienThoai))
            {
                MessageBox.Show(
                    "Điện thoại này đã có trong danh sách.\n" +
                    "Hãy chọn dòng đó để chỉnh sửa.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var dienThoaiTrongSql = db.DienThoais
                    .AsNoTracking()
                    .FirstOrDefault(
                        x => x.MaDienThoai == dienThoai.MaDienThoai);

                if (dienThoaiTrongSql == null)
                {
                    MessageBox.Show("Điện thoại không còn tồn tại.");
                    TaiDuLieuComboBox();
                    return;
                }

                if (soLuong > dienThoaiTrongSql.SoLuongTon)
                {
                    MessageBox.Show(
                        $"Tồn kho chỉ còn {dienThoaiTrongSql.SoLuongTon} chiếc.");
                    return;
                }

                if (maHoaDonDaLuu != null)
                {
                    using var dbHoaDon = new AppDbContext();

                    var chiTietCu = dbHoaDon.ChiTietHoaDons
                        .Where(x => x.MaHoaDon == maHoaDonDaLuu.Value)
                        .FirstOrDefault(
                            x => x.MaDienThoai == dienThoai.MaDienThoai);

                    int soLuongCu = chiTietCu?.SoLuong ?? 0;

                    if (soLuong > dienThoaiTrongSql.SoLuongTon
                        + soLuongCu)
                    {
                        MessageBox.Show(
                            "Số lượng vượt quá tồn kho có thể sử dụng.");
                        return;
                    }
                }
            }

            danhSachChiTiet.Add(new ChiTietHoaDon
            {
                MaDienThoai = dienThoai.MaDienThoai,
                SoLuong = soLuong,
                DonGia = donGia,
                ThanhTien = soLuong * donGia,
                DienThoai = dienThoai
            });

            HienThiChiTiet();
            XoaONhapChiTiet();
            comboBox3.Focus();
        }

        private bool ApDungSuaDongDangChon()
        {
            if (maDienThoaiDangSua == null)
                return true;

            bool oTrong = comboBox3.SelectedItem == null
                && string.IsNullOrWhiteSpace(textBox1.Text)
                && string.IsNullOrWhiteSpace(textBox2.Text);

            if (oTrong)
            {
                maDienThoaiDangSua = null;
                return true;
            }

            if (comboBox3.SelectedItem is not DienThoai dienThoai)
            {
                MessageBox.Show("Vui lòng chọn điện thoại.");
                return false;
            }

            if (!DocSoLuongDonGia(out int soLuong, out decimal donGia))
                return false;

            int maCu = maDienThoaiDangSua.Value;

            if (danhSachChiTiet.Any(
                x => x.MaDienThoai == dienThoai.MaDienThoai
                     && x.MaDienThoai != maCu))
            {
                MessageBox.Show(
                    "Điện thoại này đã có ở dòng khác.");
                return false;
            }

            using (var db = new AppDbContext())
            {
                var dienThoaiTrongSql = db.DienThoais
                    .AsNoTracking()
                    .FirstOrDefault(
                        x => x.MaDienThoai == dienThoai.MaDienThoai);

                if (dienThoaiTrongSql == null)
                {
                    MessageBox.Show("Điện thoại không còn tồn tại.");
                    return false;
                }

                int soLuongCuTrongHoaDon = 0;

                if (maHoaDonDaLuu != null)
                {
                    soLuongCuTrongHoaDon = db.ChiTietHoaDons
                        .Where(x => x.MaHoaDon == maHoaDonDaLuu.Value
                                 && x.MaDienThoai == dienThoai.MaDienThoai)
                        .Select(x => (int?)x.SoLuong)
                        .FirstOrDefault() ?? 0;
                }

                int soLuongDangCoTrongDanhSach = danhSachChiTiet
                    .Where(x => x.MaDienThoai == dienThoai.MaDienThoai
                             && x.MaDienThoai != maCu)
                    .Sum(x => x.SoLuong);

                int tonCoTheBan = dienThoaiTrongSql.SoLuongTon
                    + soLuongCuTrongHoaDon
                    - soLuongDangCoTrongDanhSach;

                if (soLuong > tonCoTheBan)
                {
                    MessageBox.Show(
                        $"Không đủ tồn kho. Số lượng tối đa có thể chọn: " +
                        $"{Math.Max(0, tonCoTheBan)}.");
                    return false;
                }
            }

            ChiTietHoaDon? dong = danhSachChiTiet
                .FirstOrDefault(x => x.MaDienThoai == maCu);

            if (dong != null)
            {
                dong.MaDienThoai = dienThoai.MaDienThoai;
                dong.SoLuong = soLuong;
                dong.DonGia = donGia;
                dong.ThanhTien = soLuong * donGia;
                dong.DienThoai = dienThoai;
            }

            HienThiChiTiet();
            XoaONhapChiTiet();

            return true;
        }

        private void HienThiChiTiet()
        {
            dataGridView1.DataSource = null;

            dataGridView1.DataSource = danhSachChiTiet
                .Select(x => new
                {
                    x.MaDienThoai,
                    TenDienThoai = x.DienThoai?.TenDienThoai ?? "",
                    x.SoLuong,
                    x.DonGia,
                    x.ThanhTien
                })
                .ToList();

            dataGridView1.ClearSelection();
            CapNhatTongTien();
        }

        private void CapNhatTongTien()
        {
            decimal tongTien = danhSachChiTiet.Sum(x => x.ThanhTien);

            label8.Text = tongTien.ToString("N0") + " VNĐ";
        }

        private void btnCapNhatHoaDon_Click(
            object? sender,
            EventArgs e)
        {
            if (!ApDungSuaDongDangChon())
                return;

            if (comboBox1.SelectedValue == null
                || comboBox2.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khách hàng và nhân viên.");
                return;
            }

            if (danhSachChiTiet.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng thêm ít nhất một điện thoại.");
                return;
            }

            if (!int.TryParse(
                    Convert.ToString(comboBox1.SelectedValue),
                    out int maKhachHang)
                || !int.TryParse(
                    Convert.ToString(comboBox2.SelectedValue),
                    out int maNhanVien))
            {
                MessageBox.Show("Thông tin khách hàng hoặc nhân viên không hợp lệ.");
                return;
            }

            if (maHoaDonDaLuu == null)
                LuuHoaDonMoi(maKhachHang, maNhanVien);
            else
                CapNhatHoaDonDaLuu(
                    maHoaDonDaLuu.Value,
                    maKhachHang,
                    maNhanVien);
        }

        private void LuuHoaDonMoi(
            int maKhachHang,
            int maNhanVien)
        {
            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                if (!db.KhachHangs.Any(
                        x => x.MaKhachHang == maKhachHang)
                    || !db.NhanViens.Any(
                        x => x.MaNhanVien == maNhanVien))
                {
                    MessageBox.Show(
                        "Khách hàng hoặc nhân viên không còn tồn tại.");
                    transaction.Rollback();
                    return;
                }

                var maDienThoais = danhSachChiTiet
                    .Select(x => x.MaDienThoai)
                    .Distinct()
                    .ToList();

                if (maDienThoais.Count != danhSachChiTiet.Count)
                {
                    MessageBox.Show(
                        "Danh sách có điện thoại bị trùng.");
                    transaction.Rollback();
                    return;
                }

                var dienThoais = db.DienThoais
                    .Where(x => maDienThoais.Contains(x.MaDienThoai))
                    .ToList();

                if (dienThoais.Count != maDienThoais.Count)
                {
                    MessageBox.Show(
                        "Có điện thoại không còn tồn tại.");
                    transaction.Rollback();
                    return;
                }

                foreach (var ct in danhSachChiTiet)
                {
                    var dienThoai = dienThoais.First(
                        x => x.MaDienThoai == ct.MaDienThoai);

                    if (ct.SoLuong > dienThoai.SoLuongTon)
                    {
                        MessageBox.Show(
                            $"Điện thoại \"{dienThoai.TenDienThoai}\" " +
                            $"không đủ tồn kho. Hiện có: {dienThoai.SoLuongTon}.");
                        transaction.Rollback();
                        return;
                    }
                }

                decimal tongTien = danhSachChiTiet.Sum(x => x.ThanhTien);

                var hoaDon = new HoaDon
                {
                    NgayLap = dateTimePicker1.Value,
                    TongTien = tongTien,
                    TrangThai = "Hoàn thành",
                    MaKhachHang = maKhachHang,
                    MaNhanVien = maNhanVien
                };

                foreach (var ct in danhSachChiTiet)
                {
                    hoaDon.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        MaDienThoai = ct.MaDienThoai,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        ThanhTien = ct.ThanhTien
                    });

                    var dienThoai = dienThoais.First(
                        x => x.MaDienThoai == ct.MaDienThoai);

                    dienThoai.SoLuongTon -= ct.SoLuong;
                }

                db.HoaDons.Add(hoaDon);
                db.SaveChanges();
                transaction.Commit();

                maHoaDonDaLuu = hoaDon.MaHoaDon;
                maDienThoaiDangSua = null;
                CapNhatNutLuu();

                MessageBox.Show(
                    $"Lưu hóa đơn thành công!\n" +
                    $"Mã hóa đơn: {hoaDon.MaHoaDon}\n" +
                    $"Tổng tiền: {hoaDon.TongTien:N0} VNĐ\n\n" +
                    "Dữ liệu vẫn được giữ lại trên form.\n" +
                    "Bạn có thể chỉnh sửa chi tiết rồi bấm Cập nhật hóa đơn.\n" +
                    "Bấm Làm mới hóa đơn để lập hóa đơn mới.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDuLieuComboBox();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch { }

                MessageBox.Show(
                    "Lưu hóa đơn thất bại.\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatHoaDonDaLuu(
            int maHoaDon,
            int maKhachHang,
            int maNhanVien)
        {
            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                HoaDon? hoaDon = db.HoaDons
                    .Include(x => x.ChiTietHoaDons)
                    .FirstOrDefault(x => x.MaHoaDon == maHoaDon);

                if (hoaDon == null)
                {
                    MessageBox.Show(
                        "Hóa đơn không còn tồn tại trong SQL Server.");
                    transaction.Rollback();
                    LamMoiHoaDon();
                    return;
                }

                if (hoaDon.TrangThai == "Đã hủy")
                {
                    MessageBox.Show(
                        "Hóa đơn đã hủy không thể cập nhật.");
                    transaction.Rollback();
                    return;
                }

                if (!db.KhachHangs.Any(
                        x => x.MaKhachHang == maKhachHang)
                    || !db.NhanViens.Any(
                        x => x.MaNhanVien == maNhanVien))
                {
                    MessageBox.Show(
                        "Khách hàng hoặc nhân viên không còn tồn tại.");
                    transaction.Rollback();
                    return;
                }

                var chiTietCu = hoaDon.ChiTietHoaDons.ToList();

                var tatCaMa = chiTietCu
                    .Select(x => x.MaDienThoai)
                    .Union(danhSachChiTiet.Select(x => x.MaDienThoai))
                    .Distinct()
                    .ToList();

                var dienThoais = db.DienThoais
                    .Where(x => tatCaMa.Contains(x.MaDienThoai))
                    .ToList();

                if (dienThoais.Count != tatCaMa.Count)
                {
                    MessageBox.Show(
                        "Có điện thoại không còn tồn tại trong SQL Server.");
                    transaction.Rollback();
                    return;
                }

                foreach (int ma in tatCaMa)
                {
                    var cu = chiTietCu.FirstOrDefault(
                        x => x.MaDienThoai == ma);

                    var moi = danhSachChiTiet.FirstOrDefault(
                        x => x.MaDienThoai == ma);

                    int soLuongCu = cu?.SoLuong ?? 0;
                    int soLuongMoi = moi?.SoLuong ?? 0;

                    var dienThoai = dienThoais.First(
                        x => x.MaDienThoai == ma);

                    int tonMoi = dienThoai.SoLuongTon
                        + soLuongCu - soLuongMoi;

                    if (tonMoi < 0)
                    {
                        MessageBox.Show(
                            $"Không đủ tồn kho cho điện thoại " +
                            $"\"{dienThoai.TenDienThoai}\".",
                            "Không thể cập nhật",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        transaction.Rollback();
                        return;
                    }

                    dienThoai.SoLuongTon = tonMoi;

                    if (cu != null && moi != null)
                    {
                        cu.SoLuong = moi.SoLuong;
                        cu.DonGia = moi.DonGia;
                        cu.ThanhTien = moi.ThanhTien;
                    }
                    else if (cu != null)
                    {
                        db.ChiTietHoaDons.Remove(cu);
                    }
                    else if (moi != null)
                    {
                        hoaDon.ChiTietHoaDons.Add(new ChiTietHoaDon
                        {
                            MaDienThoai = moi.MaDienThoai,
                            SoLuong = moi.SoLuong,
                            DonGia = moi.DonGia,
                            ThanhTien = moi.ThanhTien
                        });
                    }
                }

                hoaDon.NgayLap = dateTimePicker1.Value;
                hoaDon.MaKhachHang = maKhachHang;
                hoaDon.MaNhanVien = maNhanVien;
                hoaDon.TongTien = danhSachChiTiet.Sum(x => x.ThanhTien);

                db.SaveChanges();
                transaction.Commit();

                MessageBox.Show(
                    $"Cập nhật hóa đơn thành công!\n" +
                    $"Mã hóa đơn: {hoaDon.MaHoaDon}\n" +
                    $"Tổng tiền: {hoaDon.TongTien:N0} VNĐ",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDuLieuComboBox();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch { }

                MessageBox.Show(
                    "Cập nhật hóa đơn thất bại.\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoiHoaDon_Click(
            object? sender,
            EventArgs e)
        {
            LamMoiHoaDon();
            TaiDuLieuComboBox();
        }

        private void LamMoiHoaDon()
        {
            danhSachChiTiet.Clear();

            maHoaDonDaLuu = null;
            maDienThoaiDangSua = null;

            CapNhatNutLuu();

            dateTimePicker1.Value = DateTime.Today;

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;

            textBox1.Clear();
            textBox2.Clear();

            dataGridView1.DataSource = null;

            CapNhatTongTien();
        }
    }
}
