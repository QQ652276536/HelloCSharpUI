namespace HelloCSharp.UI
{
    partial class ImageTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageTest));
            this.btn_open = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbx_parity = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbx_data = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_rate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbx_port = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txt_log1 = new System.Windows.Forms.RichTextBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_start = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(647, 17);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(100, 27);
            this.btn_open.TabIndex = 5;
            this.btn_open.Text = "打开串口";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbx_parity);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbx_data);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbx_rate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_open);
            this.groupBox1.Controls.Add(this.cbx_port);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(16, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(768, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "串口配置";
            // 
            // cbx_parity
            // 
            this.cbx_parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_parity.FormattingEnabled = true;
            this.cbx_parity.Location = new System.Drawing.Point(538, 18);
            this.cbx_parity.Name = "cbx_parity";
            this.cbx_parity.Size = new System.Drawing.Size(80, 24);
            this.cbx_parity.TabIndex = 7;
            this.cbx_parity.SelectedIndexChanged += new System.EventHandler(this.cbx_parity_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(467, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "校验位：";
            // 
            // cbx_data
            // 
            this.cbx_data.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_data.FormattingEnabled = true;
            this.cbx_data.Location = new System.Drawing.Point(380, 18);
            this.cbx_data.Name = "cbx_data";
            this.cbx_data.Size = new System.Drawing.Size(80, 24);
            this.cbx_data.TabIndex = 0;
            this.cbx_data.SelectedIndexChanged += new System.EventHandler(this.cbx_data_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(308, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "数据位：";
            // 
            // cbx_rate
            // 
            this.cbx_rate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_rate.FormattingEnabled = true;
            this.cbx_rate.Location = new System.Drawing.Point(222, 18);
            this.cbx_rate.Name = "cbx_rate";
            this.cbx_rate.Size = new System.Drawing.Size(80, 24);
            this.cbx_rate.TabIndex = 2;
            this.cbx_rate.SelectedIndexChanged += new System.EventHandler(this.cbx_rate_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "波特率：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "串口：";
            // 
            // cbx_port
            // 
            this.cbx_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_port.FormattingEnabled = true;
            this.cbx_port.Location = new System.Drawing.Point(66, 18);
            this.cbx_port.Name = "cbx_port";
            this.cbx_port.Size = new System.Drawing.Size(80, 24);
            this.cbx_port.TabIndex = 6;
            this.cbx_port.SelectedIndexChanged += new System.EventHandler(this.cbx_port_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.btn_start);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(760, 670);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "通信";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txt_log1);
            this.groupBox6.Controls.Add(this.btn_clear);
            this.groupBox6.Location = new System.Drawing.Point(8, 322);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(746, 346);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "日志";
            // 
            // txt_log1
            // 
            this.txt_log1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_log1.Location = new System.Drawing.Point(4, 21);
            this.txt_log1.Name = "txt_log1";
            this.txt_log1.ReadOnly = true;
            this.txt_log1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txt_log1.Size = new System.Drawing.Size(736, 293);
            this.txt_log1.TabIndex = 1;
            this.txt_log1.Text = "";
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(665, 320);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 23);
            this.btn_clear.TabIndex = 0;
            this.btn_clear.Text = "清除";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_start
            // 
            this.btn_start.Enabled = false;
            this.btn_start.Location = new System.Drawing.Point(673, 3);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "开始";
            this.btn_start.Click += new System.EventHandler(this.btn_start_click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Location = new System.Drawing.Point(8, 34);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(740, 282);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "图片";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl.Location = new System.Drawing.Point(16, 89);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(768, 700);
            this.tabControl.TabIndex = 1;
            // 
            // ImageTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ImageTest";
            this.Text = "串口数据解析图片测试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageTest_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbx_data;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_rate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbx_port;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.ComboBox cbx_parity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RichTextBox txt_log1;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button btn_start;
    }
}