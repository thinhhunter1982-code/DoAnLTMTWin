namespace DoAnLTMTWin
{
    partial class FormLoaiDienThoai
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoaiDienThoai));
            panel1 = new Panel();
            btnTimKiem = new Button();
            textBox3 = new TextBox();
            btnLamMoi = new Button();
            textBox2 = new TextBox();
            btnXoa = new Button();
            label4 = new Label();
            btnSua = new Button();
            label3 = new Label();
            btnThem = new Button();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(btnTimKiem);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(btnLamMoi);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(btnXoa);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(btnSua);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnThem);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(421, 613);
            panel1.TabIndex = 0;
            // 
            // btnTimKiem
            // 
            btnTimKiem.BackColor = Color.Gray;
            btnTimKiem.FlatAppearance.BorderSize = 0;
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnTimKiem.ForeColor = SystemColors.ControlLightLight;
            btnTimKiem.Image = (Image)resources.GetObject("btnTimKiem.Image");
            btnTimKiem.ImageAlign = ContentAlignment.MiddleLeft;
            btnTimKiem.Location = new Point(23, 431);
            btnTimKiem.Name = "btnTimKiem";
            btnTimKiem.Size = new Size(379, 47);
            btnTimKiem.TabIndex = 11;
            btnTimKiem.Text = "Tìm kiếm";
            btnTimKiem.UseVisualStyleBackColor = false;
            btnTimKiem.Click += btnTimKiem_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(23, 249);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(379, 27);
            textBox3.TabIndex = 6;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BackColor = Color.FromArgb(106, 45, 186);
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnLamMoi.ForeColor = SystemColors.ControlLightLight;
            btnLamMoi.Image = (Image)resources.GetObject("btnLamMoi.Image");
            btnLamMoi.ImageAlign = ContentAlignment.MiddleLeft;
            btnLamMoi.Location = new Point(214, 378);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Size = new Size(188, 47);
            btnLamMoi.TabIndex = 10;
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.UseVisualStyleBackColor = false;
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(23, 145);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(379, 27);
            textBox2.TabIndex = 5;
            // 
            // btnXoa
            // 
            btnXoa.BackColor = Color.FromArgb(20, 118, 75);
            btnXoa.FlatAppearance.BorderSize = 0;
            btnXoa.FlatStyle = FlatStyle.Flat;
            btnXoa.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnXoa.ForeColor = SystemColors.ControlLightLight;
            btnXoa.Image = (Image)resources.GetObject("btnXoa.Image");
            btnXoa.ImageAlign = ContentAlignment.MiddleLeft;
            btnXoa.Location = new Point(214, 325);
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(188, 47);
            btnXoa.TabIndex = 9;
            btnXoa.Text = "Xóa";
            btnXoa.UseVisualStyleBackColor = false;
            btnXoa.Click += btnXoa_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(23, 209);
            label4.Name = "label4";
            label4.Size = new Size(67, 25);
            label4.TabIndex = 3;
            label4.Text = "Mô tả:";
            // 
            // btnSua
            // 
            btnSua.BackColor = Color.FromArgb(182, 73, 27);
            btnSua.FlatAppearance.BorderSize = 0;
            btnSua.FlatStyle = FlatStyle.Flat;
            btnSua.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnSua.ForeColor = SystemColors.ControlLightLight;
            btnSua.Image = (Image)resources.GetObject("btnSua.Image");
            btnSua.ImageAlign = ContentAlignment.MiddleLeft;
            btnSua.Location = new Point(23, 378);
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(189, 47);
            btnSua.TabIndex = 8;
            btnSua.Text = "Sửa";
            btnSua.UseVisualStyleBackColor = false;
            btnSua.Click += btnSua_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(23, 117);
            label3.Name = "label3";
            label3.Size = new Size(83, 25);
            label3.TabIndex = 2;
            label3.Text = "Tên loại:";
            // 
            // btnThem
            // 
            btnThem.BackColor = Color.FromArgb(18, 97, 197);
            btnThem.FlatAppearance.BorderSize = 0;
            btnThem.FlatStyle = FlatStyle.Flat;
            btnThem.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnThem.ForeColor = SystemColors.ControlLightLight;
            btnThem.Image = (Image)resources.GetObject("btnThem.Image");
            btnThem.ImageAlign = ContentAlignment.MiddleLeft;
            btnThem.Location = new Point(23, 325);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(189, 47);
            btnThem.TabIndex = 7;
            btnThem.Text = "Thêm";
            btnThem.UseVisualStyleBackColor = false;
            btnThem.Click += btnThem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(23, 29);
            label1.Name = "label1";
            label1.Size = new Size(379, 38);
            label1.TabIndex = 0;
            label1.Text = "QUẢN LÝ LOẠI ĐIỆN THOẠI";
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.ControlLightLight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(421, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(902, 613);
            dataGridView1.TabIndex = 1;
            // 
            // FormLoaiDienThoai
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1323, 613);
            Controls.Add(dataGridView1);
            Controls.Add(panel1);
            Name = "FormLoaiDienThoai";
            Text = "Loại điện thoại";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label4;
        private Label label3;
        private Label label1;
        private TextBox textBox3;
        private TextBox textBox2;
        private DataGridView dataGridView1;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;
    }
}