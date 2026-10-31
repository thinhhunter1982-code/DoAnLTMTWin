namespace DoAnLTMTWin
{
    partial class FormThanhToanQR
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
            label3 = new Label();
            lblMaHoaDon = new Label();
            label5 = new Label();
            lblTongTien = new Label();
            pictureBox1 = new PictureBox();
            lblTrangThai = new Label();
            btnThanhToan = new Button();
            label1 = new Label();
            label2 = new Label();
            btnDong = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(12, 147);
            label3.Name = "label3";
            label3.Size = new Size(132, 28);
            label3.TabIndex = 1;
            label3.Text = "Mã hóa đơn:";
            // 
            // lblMaHoaDon
            // 
            lblMaHoaDon.AutoSize = true;
            lblMaHoaDon.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblMaHoaDon.ForeColor = SystemColors.ControlLightLight;
            lblMaHoaDon.Location = new Point(482, 147);
            lblMaHoaDon.Name = "lblMaHoaDon";
            lblMaHoaDon.Size = new Size(143, 28);
            lblMaHoaDon.TabIndex = 2;
            lblMaHoaDon.Text = "lblMaHoaDon";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label5.ForeColor = Color.FromArgb(0, 192, 0);
            label5.Location = new Point(12, 188);
            label5.Name = "label5";
            label5.Size = new Size(108, 28);
            label5.TabIndex = 3;
            label5.Text = "Tổng tiền:";
            // 
            // lblTongTien
            // 
            lblTongTien.AutoSize = true;
            lblTongTien.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblTongTien.ForeColor = Color.FromArgb(0, 192, 0);
            lblTongTien.Location = new Point(482, 188);
            lblTongTien.Name = "lblTongTien";
            lblTongTien.Size = new Size(123, 28);
            lblTongTien.TabIndex = 4;
            lblTongTien.Text = "lblTongTien";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.Image = Properties.Resources.Screenshot_2026_10_31_174149;
            pictureBox1.Location = new Point(207, 279);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(223, 202);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // lblTrangThai
            // 
            lblTrangThai.AutoSize = true;
            lblTrangThai.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblTrangThai.ForeColor = SystemColors.ControlLightLight;
            lblTrangThai.Location = new Point(207, 525);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(223, 28);
            lblTrangThai.TabIndex = 6;
            lblTrangThai.Text = "Đang chờ thanh toán..";
            // 
            // btnThanhToan
            // 
            btnThanhToan.BackColor = Color.LimeGreen;
            btnThanhToan.FlatAppearance.BorderSize = 0;
            btnThanhToan.FlatStyle = FlatStyle.Flat;
            btnThanhToan.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnThanhToan.ForeColor = SystemColors.ControlLightLight;
            btnThanhToan.Location = new Point(12, 594);
            btnThanhToan.Name = "btnThanhToan";
            btnThanhToan.Size = new Size(613, 49);
            btnThanhToan.TabIndex = 7;
            btnThanhToan.Text = "Thanh toán";
            btnThanhToan.UseVisualStyleBackColor = false;
            btnThanhToan.Click += btnThanhToan_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(151, 9);
            label1.Name = "label1";
            label1.Size = new Size(335, 60);
            label1.TabIndex = 0;
            label1.Text = "Thanh toán QR";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(118, 80);
            label2.Name = "label2";
            label2.Size = new Size(406, 23);
            label2.TabIndex = 1;
            label2.Text = "Quét mã QR bằng camera điện thoại để thanh toán";
            // 
            // btnDong
            // 
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnDong.ForeColor = SystemColors.ControlLightLight;
            btnDong.Location = new Point(207, 649);
            btnDong.Name = "btnDong";
            btnDong.Size = new Size(223, 49);
            btnDong.TabIndex = 8;
            btnDong.Text = "Đóng";
            btnDong.UseVisualStyleBackColor = true;
            btnDong.Click += btnDong_Click_1;
            // 
            // FormThanhToanQR
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(637, 738);
            Controls.Add(btnDong);
            Controls.Add(label2);
            Controls.Add(btnThanhToan);
            Controls.Add(label1);
            Controls.Add(lblTrangThai);
            Controls.Add(pictureBox1);
            Controls.Add(lblTongTien);
            Controls.Add(label5);
            Controls.Add(lblMaHoaDon);
            Controls.Add(label3);
            Name = "FormThanhToanQR";
            Text = "Thanh Toán QR";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label3;
        private Label lblMaHoaDon;
        private Label label5;
        private Label lblTongTien;
        private PictureBox pictureBox1;
        private Label lblTrangThai;
        private Button btnThanhToan;
        private Label label1;
        private Label label2;
        private Button btnDong;
    }
}