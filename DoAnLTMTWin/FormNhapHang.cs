using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTMTWin
{
    public partial class FormNhapHang : Form
    {
        private readonly List<ChiTietPhieuNhap> danhSachChiTiet = new();

        private int? maPhieuNhapDaLuu;
        private int? maDienThoaiDangSua;

        private Button? nutLuu;
        private string chuNutLuuGoc = "Lưu phiếu nhập";

        public FormNhapHang()
        {
            InitializeComponent();
            Load += FormNhapHang_Load;
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

        private void FormNhapHang_Load(object? sender, EventArgs e)
        {
            nutLuu = TimTatCaControls(this)
                         .OfType<Button>()
                         .FirstOrDefault(b => string.Equals(
                             b.Name,
                             "btnLuuPhieuNhap",
                             StringComparison.OrdinalIgnoreCase))
                     ?? TimTatCaControls(this)
                         .OfType<Button>()
                         .FirstOrDefault(b => b.Text.Contains(
                             "Lưu",
                             StringComparison.OrdinalIgnoreCase));

            if (nutLuu != null)
                chuNutLuuGoc = nutLuu.Text;

            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;

            CauHinhDataGridView();
            TaiDuLieuComboBox();
            CapNhatTongTien();
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
                HeaderText = "Điện thoại",
                DataPropertyName = "TenDienThoai",
                Width = 220
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonGia",
                HeaderText = "Đơn giá nhập",
                DataPropertyName = "DonGiaNhap",
                Width = 150,
                DefaultCellStyle = { Format = "N0" }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThanhTien",
                HeaderText = "Thành tiền",
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

                comboBox1.DataSource = db.NhaCungCaps
                    .AsNoTracking()
                    .OrderBy(x => x.TenNhaCungCap)
                    .ToList();

                comboBox1.DisplayMember = "TenNhaCungCap";
                comboBox1.ValueMember = "MaNhaCungCap";
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
                    "Không thể tải dữ liệu từ SQL Server.\n" + ex.Message,
                    "Lỗi tải dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatNutLuu()
        {
            if (nutLuu == null)
                return;

            nutLuu.Text = maPhieuNhapDaLuu == null
                ? chuNutLuuGoc
                : "Cập nhật phiếu nhập";
        }

        private bool DocSoLuongDonGia(out int soLuong, out decimal donGia)
        {
            soLuong = 0;
            donGia = 0;

            if (!int.TryParse(textBox1.Text.Trim(), out soLuong)
                || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.");
                textBox1.Focus();
                return false;
            }

            if (!decimal.TryParse(textBox2.Text.Trim(), out donGia)
                || donGia <= 0)
            {
                MessageBox.Show("Đơn giá nhập phải lớn hơn 0.");
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
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
                return;

            object? giaTri =
                dataGridView1.Rows[e.RowIndex].Cells["colMaDienThoai"].Value;

            if (giaTri == null)
                return;

            int maDienThoai = Convert.ToInt32(giaTri);

            ChiTietPhieuNhap? dong = danhSachChiTiet
                .FirstOrDefault(x => x.MaDienThoai == maDienThoai);

            if (dong == null)
                return;

            maDienThoaiDangSua = dong.MaDienThoai;

            comboBox3.SelectedValue = dong.MaDienThoai;
            textBox1.Text = dong.SoLuong.ToString();
            textBox2.Text = dong.DonGiaNhap.ToString("0.##");
        }

        private void btnThemChiTiet_Click(object? sender, EventArgs e)
        {
            if (comboBox3.SelectedItem is not DienThoai dienThoai)
            {
                MessageBox.Show("Vui lòng chọn điện thoại.");
                comboBox3.Focus();
                return;
            }

            if (!DocSoLuongDonGia(out int soLuong, out decimal donGia))
                return;

            if (danhSachChiTiet.Any(
                x => x.MaDienThoai == dienThoai.MaDienThoai))
            {
                MessageBox.Show(
                    "Điện thoại này đã có trong danh sách.\n\n" +
                    "Nếu muốn sửa dòng này, hãy bấm vào dòng đó trong bảng, " +
                    "chỉnh lại rồi bấm nút Lưu/Cập nhật.");
                return;
            }

            danhSachChiTiet.Add(new ChiTietPhieuNhap
            {
                MaDienThoai = dienThoai.MaDienThoai,
                SoLuong = soLuong,
                DonGiaNhap = donGia,
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
                comboBox3.Focus();
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
                    "Điện thoại này đã có ở một dòng khác trong danh sách.");
                return false;
            }

            ChiTietPhieuNhap? dong = danhSachChiTiet
                .FirstOrDefault(x => x.MaDienThoai == maCu);

            if (dong != null)
            {
                dong.MaDienThoai = dienThoai.MaDienThoai;
                dong.SoLuong = soLuong;
                dong.DonGiaNhap = donGia;
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
                    x.DonGiaNhap,
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

        private void btnLuuPhieuNhap_Click(object? sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null
                || comboBox2.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn nhà cung cấp và nhân viên.");
                return;
            }

            if (!ApDungSuaDongDangChon())
                return;

            if (danhSachChiTiet.Count == 0)
            {
                MessageBox.Show(
                    "Vui lòng thêm ít nhất một điện thoại.");
                return;
            }

            if (!int.TryParse(
                    Convert.ToString(comboBox1.SelectedValue),
                    out int maNhaCungCap)
                || !int.TryParse(
                    Convert.ToString(comboBox2.SelectedValue),
                    out int maNhanVien))
            {
                MessageBox.Show("Thông tin đã chọn không hợp lệ.");
                return;
            }

            if (maPhieuNhapDaLuu == null)
                LuuPhieuMoi(maNhaCungCap, maNhanVien);
            else
                CapNhatPhieu(maPhieuNhapDaLuu.Value, maNhaCungCap, maNhanVien);
        }

        private void LuuPhieuMoi(int maNhaCungCap, int maNhanVien)
        {
            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                bool nhaCungCapTonTai = db.NhaCungCaps
                    .Any(x => x.MaNhaCungCap == maNhaCungCap);

                bool nhanVienTonTai = db.NhanViens
                    .Any(x => x.MaNhanVien == maNhanVien);

                if (!nhaCungCapTonTai || !nhanVienTonTai)
                {
                    MessageBox.Show(
                        "Nhà cung cấp hoặc nhân viên không còn tồn tại.");
                    transaction.Rollback();
                    return;
                }

                var maDienThoais = danhSachChiTiet
                    .Select(x => x.MaDienThoai)
                    .ToList();

                var dienThoaiTrongSql = db.DienThoais
                    .Where(x => maDienThoais.Contains(x.MaDienThoai))
                    .ToList();

                if (dienThoaiTrongSql.Count != maDienThoais.Count)
                {
                    MessageBox.Show(
                        "Có điện thoại không còn tồn tại trong SQL Server.");
                    transaction.Rollback();
                    return;
                }

                decimal tongTien = danhSachChiTiet.Sum(x => x.ThanhTien);

                var phieuNhap = new PhieuNhap
                {
                    NgayNhap = dateTimePicker1.Value,
                    MaNhaCungCap = maNhaCungCap,
                    MaNhanVien = maNhanVien,
                    TongTien = tongTien
                };

                foreach (var ct in danhSachChiTiet)
                {
                    phieuNhap.ChiTietPhieuNhaps.Add(
                        new ChiTietPhieuNhap
                        {
                            MaDienThoai = ct.MaDienThoai,
                            SoLuong = ct.SoLuong,
                            DonGiaNhap = ct.DonGiaNhap,
                            ThanhTien = ct.ThanhTien
                        });
                }

                db.PhieuNhaps.Add(phieuNhap);

                foreach (var ct in danhSachChiTiet)
                {
                    var dienThoai = dienThoaiTrongSql.First(
                        x => x.MaDienThoai == ct.MaDienThoai);

                    checked
                    {
                        dienThoai.SoLuongTon += ct.SoLuong;
                    }
                }

                db.SaveChanges();
                transaction.Commit();

                maPhieuNhapDaLuu = phieuNhap.MaPhieuNhap;
                maDienThoaiDangSua = null;
                CapNhatNutLuu();

                MessageBox.Show(
                    $"Lưu phiếu nhập thành công!\n" +
                    $"Mã phiếu nhập: {phieuNhap.MaPhieuNhap}\n" +
                    $"Tổng tiền: {phieuNhap.TongTien:N0} VNĐ\n\n" +
                    "Dữ liệu vẫn được giữ lại trên form.\n" +
                    "Nếu nhập sai, bấm vào dòng cần sửa, chỉnh lại rồi bấm " +
                    "Cập nhật phiếu nhập.\n" +
                    "Bấm Làm mới để nhập phiếu mới.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }

                MessageBox.Show(
                    "Lưu phiếu nhập thất bại.\n" + ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatPhieu(
            int maPhieuNhap,
            int maNhaCungCap,
            int maNhanVien)
        {
            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                PhieuNhap? phieu = db.PhieuNhaps
                    .Include(p => p.ChiTietPhieuNhaps)
                    .FirstOrDefault(p => p.MaPhieuNhap == maPhieuNhap);

                if (phieu == null)
                {
                    MessageBox.Show(
                        "Phiếu nhập này không còn tồn tại trong SQL Server.");
                    transaction.Rollback();
                    LamMoiForm();
                    return;
                }

                bool nhaCungCapTonTai = db.NhaCungCaps
                    .Any(x => x.MaNhaCungCap == maNhaCungCap);

                bool nhanVienTonTai = db.NhanViens
                    .Any(x => x.MaNhanVien == maNhanVien);

                if (!nhaCungCapTonTai || !nhanVienTonTai)
                {
                    MessageBox.Show(
                        "Nhà cung cấp hoặc nhân viên không còn tồn tại.");
                    transaction.Rollback();
                    return;
                }

                List<ChiTietPhieuNhap> chiTietCu =
                    phieu.ChiTietPhieuNhaps.ToList();

                List<int> tatCaMa = chiTietCu
                    .Select(x => x.MaDienThoai)
                    .Union(danhSachChiTiet.Select(x => x.MaDienThoai))
                    .Distinct()
                    .ToList();

                var dienThoais = db.DienThoais
                    .Where(x => tatCaMa.Contains(x.MaDienThoai))
                    .ToList();

                bool thieuDienThoai = danhSachChiTiet.Any(
                    ct => dienThoais.All(d => d.MaDienThoai != ct.MaDienThoai));

                if (thieuDienThoai)
                {
                    MessageBox.Show(
                        "Có điện thoại không còn tồn tại trong SQL Server.");
                    transaction.Rollback();
                    return;
                }

                foreach (int ma in tatCaMa)
                {
                    ChiTietPhieuNhap? cu = chiTietCu
                        .FirstOrDefault(x => x.MaDienThoai == ma);

                    ChiTietPhieuNhap? moi = danhSachChiTiet
                        .FirstOrDefault(x => x.MaDienThoai == ma);

                    int soLuongCu = cu?.SoLuong ?? 0;
                    int soLuongMoi = moi?.SoLuong ?? 0;

                    var dienThoai = dienThoais
                        .FirstOrDefault(x => x.MaDienThoai == ma);

                    if (dienThoai != null)
                    {
                        var tonMoi = dienThoai.SoLuongTon
                            + (soLuongMoi - soLuongCu);

                        if (tonMoi < 0)
                        {
                            MessageBox.Show(
                                "Không thể cập nhật vì tồn kho của \"" +
                                dienThoai.TenDienThoai +
                                "\" sẽ bị âm (hàng đã được bán bớt).",
                                "Không thể cập nhật",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        dienThoai.SoLuongTon = tonMoi;
                    }

                    if (cu != null && moi != null)
                    {
                        cu.SoLuong = moi.SoLuong;
                        cu.DonGiaNhap = moi.DonGiaNhap;
                        cu.ThanhTien = moi.ThanhTien;
                    }
                    else if (cu != null)
                    {
                        db.Remove(cu);
                    }
                    else if (moi != null)
                    {
                        phieu.ChiTietPhieuNhaps.Add(
                            new ChiTietPhieuNhap
                            {
                                MaDienThoai = moi.MaDienThoai,
                                SoLuong = moi.SoLuong,
                                DonGiaNhap = moi.DonGiaNhap,
                                ThanhTien = moi.ThanhTien
                            });
                    }
                }

                phieu.NgayNhap = dateTimePicker1.Value;
                phieu.MaNhaCungCap = maNhaCungCap;
                phieu.MaNhanVien = maNhanVien;
                phieu.TongTien = danhSachChiTiet.Sum(x => x.ThanhTien);

                db.SaveChanges();
                transaction.Commit();

                MessageBox.Show(
                    $"Cập nhật phiếu nhập thành công!\n" +
                    $"Mã phiếu nhập: {phieu.MaPhieuNhap}\n" +
                    $"Tổng tiền: {phieu.TongTien:N0} VNĐ",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }

                MessageBox.Show(
                    "Cập nhật phiếu nhập thất bại.\n" +
                    ex.GetBaseException().Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object? sender, EventArgs e)
        {
            LamMoiForm();
            TaiDuLieuComboBox();
        }

        private void LamMoiForm()
        {
            danhSachChiTiet.Clear();

            maPhieuNhapDaLuu = null;
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