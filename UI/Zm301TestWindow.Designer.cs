namespace HelloCSharp.UI
{
    partial class Zm301TestWidnow
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btn_name_write = new System.Windows.Forms.Button();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txt_log = new System.Windows.Forms.RichTextBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btn_gps = new System.Windows.Forms.Button();
            this.txt_gps = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_box_write = new System.Windows.Forms.Button();
            this.btn_box_read = new System.Windows.Forms.Button();
            this.txt_box_id = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_work_write = new System.Windows.Forms.Button();
            this.btn_work_read = new System.Windows.Forms.Button();
            this.txt_work_id = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_lock1 = new System.Windows.Forms.Button();
            this.btn_lock2 = new System.Windows.Forms.Button();
            this.btn_lock3 = new System.Windows.Forms.Button();
            this.btn_lock_all = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btn_clear2 = new System.Windows.Forms.Button();
            this.txt_log2 = new System.Windows.Forms.RichTextBox();
            this.btn_cycle_start = new System.Windows.Forms.Button();
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
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl.Location = new System.Drawing.Point(16, 89);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(768, 700);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(760, 670);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "单项测试";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.btn_name_write);
            this.groupBox9.Controls.Add(this.txt_name);
            this.groupBox9.Location = new System.Drawing.Point(6, 17);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(273, 53);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "蓝牙名称（5Hex）";
            // 
            // btn_name_write
            // 
            this.btn_name_write.Enabled = false;
            this.btn_name_write.Location = new System.Drawing.Point(143, 19);
            this.btn_name_write.Name = "btn_name_write";
            this.btn_name_write.Size = new System.Drawing.Size(75, 27);
            this.btn_name_write.TabIndex = 1;
            this.btn_name_write.Text = "设置";
            this.btn_name_write.Click += new System.EventHandler(this.btn_name_write_Click);
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(6, 20);
            this.txt_name.MaxLength = 5;
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(100, 26);
            this.txt_name.TabIndex = 2;
            this.txt_name.TextChanged += new System.EventHandler(this.txt_name_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txt_log);
            this.groupBox6.Controls.Add(this.btn_clear);
            this.groupBox6.Location = new System.Drawing.Point(8, 322);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(746, 346);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "日志";
            // 
            // txt_log
            // 
            this.txt_log.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_log.Location = new System.Drawing.Point(4, 21);
            this.txt_log.Name = "txt_log";
            this.txt_log.ReadOnly = true;
            this.txt_log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(736, 293);
            this.txt_log.TabIndex = 1;
            this.txt_log.Text = "";
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_gps);
            this.groupBox5.Controls.Add(this.txt_gps);
            this.groupBox5.Location = new System.Drawing.Point(8, 232);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(746, 74);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "GPS位置";
            // 
            // btn_gps
            // 
            this.btn_gps.Location = new System.Drawing.Point(652, 22);
            this.btn_gps.Name = "btn_gps";
            this.btn_gps.Size = new System.Drawing.Size(75, 43);
            this.btn_gps.TabIndex = 0;
            this.btn_gps.Text = "查询";
            this.btn_gps.UseVisualStyleBackColor = true;
            this.btn_gps.Click += new System.EventHandler(this.btn_gps_Click);
            // 
            // txt_gps
            // 
            this.txt_gps.BackColor = System.Drawing.Color.Silver;
            this.txt_gps.Enabled = false;
            this.txt_gps.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_gps.Location = new System.Drawing.Point(6, 22);
            this.txt_gps.Multiline = true;
            this.txt_gps.Name = "txt_gps";
            this.txt_gps.ReadOnly = true;
            this.txt_gps.Size = new System.Drawing.Size(622, 43);
            this.txt_gps.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_box_write);
            this.groupBox4.Controls.Add(this.btn_box_read);
            this.groupBox4.Controls.Add(this.txt_box_id);
            this.groupBox4.Location = new System.Drawing.Point(428, 163);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(320, 53);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "表箱号（6位）";
            // 
            // btn_box_write
            // 
            this.btn_box_write.Enabled = false;
            this.btn_box_write.Location = new System.Drawing.Point(133, 20);
            this.btn_box_write.Name = "btn_box_write";
            this.btn_box_write.Size = new System.Drawing.Size(75, 27);
            this.btn_box_write.TabIndex = 0;
            this.btn_box_write.Text = "设置";
            this.btn_box_write.UseVisualStyleBackColor = true;
            this.btn_box_write.Click += new System.EventHandler(this.btn_box_write_Click);
            // 
            // btn_box_read
            // 
            this.btn_box_read.Location = new System.Drawing.Point(232, 20);
            this.btn_box_read.Name = "btn_box_read";
            this.btn_box_read.Size = new System.Drawing.Size(75, 27);
            this.btn_box_read.TabIndex = 1;
            this.btn_box_read.Text = "读取";
            this.btn_box_read.UseVisualStyleBackColor = true;
            this.btn_box_read.Click += new System.EventHandler(this.btn_box_read_Click);
            // 
            // txt_box_id
            // 
            this.txt_box_id.Location = new System.Drawing.Point(6, 20);
            this.txt_box_id.MaxLength = 6;
            this.txt_box_id.Name = "txt_box_id";
            this.txt_box_id.Size = new System.Drawing.Size(100, 26);
            this.txt_box_id.TabIndex = 2;
            this.txt_box_id.TextChanged += new System.EventHandler(this.txt_box_id_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_work_write);
            this.groupBox3.Controls.Add(this.btn_work_read);
            this.groupBox3.Controls.Add(this.txt_work_id);
            this.groupBox3.Location = new System.Drawing.Point(6, 163);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(368, 53);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "工号（8位）";
            // 
            // btn_work_write
            // 
            this.btn_work_write.Enabled = false;
            this.btn_work_write.Location = new System.Drawing.Point(143, 20);
            this.btn_work_write.Name = "btn_work_write";
            this.btn_work_write.Size = new System.Drawing.Size(75, 27);
            this.btn_work_write.TabIndex = 0;
            this.btn_work_write.Text = "设置";
            this.btn_work_write.UseVisualStyleBackColor = true;
            this.btn_work_write.Click += new System.EventHandler(this.btn_work_write_Click);
            // 
            // btn_work_read
            // 
            this.btn_work_read.Location = new System.Drawing.Point(274, 20);
            this.btn_work_read.Name = "btn_work_read";
            this.btn_work_read.Size = new System.Drawing.Size(75, 27);
            this.btn_work_read.TabIndex = 1;
            this.btn_work_read.Text = "读取";
            this.btn_work_read.UseVisualStyleBackColor = true;
            this.btn_work_read.Click += new System.EventHandler(this.btn_work_read_Click);
            // 
            // txt_work_id
            // 
            this.txt_work_id.Location = new System.Drawing.Point(6, 20);
            this.txt_work_id.MaxLength = 8;
            this.txt_work_id.Name = "txt_work_id";
            this.txt_work_id.Size = new System.Drawing.Size(100, 26);
            this.txt_work_id.TabIndex = 2;
            this.txt_work_id.TextChanged += new System.EventHandler(this.txt_work_id_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.btn_lock1);
            this.groupBox2.Controls.Add(this.btn_lock2);
            this.groupBox2.Controls.Add(this.btn_lock3);
            this.groupBox2.Controls.Add(this.btn_lock_all);
            this.groupBox2.Location = new System.Drawing.Point(8, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 53);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "开锁";
            // 
            // btn_lock1
            // 
            this.btn_lock1.Location = new System.Drawing.Point(6, 20);
            this.btn_lock1.Name = "btn_lock1";
            this.btn_lock1.Size = new System.Drawing.Size(75, 27);
            this.btn_lock1.TabIndex = 0;
            this.btn_lock1.Text = "开锁1";
            this.btn_lock1.UseVisualStyleBackColor = true;
            this.btn_lock1.Click += new System.EventHandler(this.btn_lock1_Click);
            // 
            // btn_lock2
            // 
            this.btn_lock2.Location = new System.Drawing.Point(143, 20);
            this.btn_lock2.Name = "btn_lock2";
            this.btn_lock2.Size = new System.Drawing.Size(75, 27);
            this.btn_lock2.TabIndex = 1;
            this.btn_lock2.Text = "开锁2";
            this.btn_lock2.UseVisualStyleBackColor = true;
            this.btn_lock2.Click += new System.EventHandler(this.btn_lock2_Click);
            // 
            // btn_lock3
            // 
            this.btn_lock3.Location = new System.Drawing.Point(274, 20);
            this.btn_lock3.Name = "btn_lock3";
            this.btn_lock3.Size = new System.Drawing.Size(75, 27);
            this.btn_lock3.TabIndex = 2;
            this.btn_lock3.Text = "开锁3";
            this.btn_lock3.UseVisualStyleBackColor = true;
            this.btn_lock3.Click += new System.EventHandler(this.btn_lock3_Click);
            // 
            // btn_lock_all
            // 
            this.btn_lock_all.Location = new System.Drawing.Point(405, 20);
            this.btn_lock_all.Name = "btn_lock_all";
            this.btn_lock_all.Size = new System.Drawing.Size(75, 27);
            this.btn_lock_all.TabIndex = 3;
            this.btn_lock_all.Text = "开全部锁";
            this.btn_lock_all.UseVisualStyleBackColor = true;
            this.btn_lock_all.Click += new System.EventHandler(this.btn_lock_all_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.btn_cycle_start);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(760, 670);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "循环测试";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btn_clear2);
            this.groupBox7.Controls.Add(this.txt_log2);
            this.groupBox7.Location = new System.Drawing.Point(8, 44);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(746, 624);
            this.groupBox7.TabIndex = 1;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "日志";
            // 
            // btn_clear2
            // 
            this.btn_clear2.Location = new System.Drawing.Point(665, 597);
            this.btn_clear2.Name = "btn_clear2";
            this.btn_clear2.Size = new System.Drawing.Size(75, 23);
            this.btn_clear2.TabIndex = 0;
            this.btn_clear2.Text = "清除";
            this.btn_clear2.UseVisualStyleBackColor = true;
            this.btn_clear2.Click += new System.EventHandler(this.btn_clear2_Click);
            // 
            // txt_log2
            // 
            this.txt_log2.Location = new System.Drawing.Point(6, 25);
            this.txt_log2.Name = "txt_log2";
            this.txt_log2.ReadOnly = true;
            this.txt_log2.Size = new System.Drawing.Size(733, 566);
            this.txt_log2.TabIndex = 1;
            this.txt_log2.Text = "";
            // 
            // btn_cycle_start
            // 
            this.btn_cycle_start.Location = new System.Drawing.Point(328, 15);
            this.btn_cycle_start.Name = "btn_cycle_start";
            this.btn_cycle_start.Size = new System.Drawing.Size(75, 23);
            this.btn_cycle_start.TabIndex = 0;
            this.btn_cycle_start.Text = "开始";
            this.btn_cycle_start.UseVisualStyleBackColor = true;
            this.btn_cycle_start.Click += new System.EventHandler(this.btn_cycle_start_Click);
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(647, 17);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(100, 27);
            this.btn_open.TabIndex = 5;
            this.btn_open.Text = "打开串口";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
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
            // Zm301TestWidnow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Zm301TestWidnow";
            this.Text = "ZM301研发压力测试工具";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbx_data;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_rate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbx_port;
        private System.Windows.Forms.TextBox txt_work_id;
        private System.Windows.Forms.Button btn_lock_all;
        private System.Windows.Forms.Button btn_lock2;
        private System.Windows.Forms.Button btn_lock1;
        private System.Windows.Forms.Button btn_lock3;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_work_write;
        private System.Windows.Forms.Button btn_work_read;
        private System.Windows.Forms.Button btn_gps;
        private System.Windows.Forms.TextBox txt_gps;
        private System.Windows.Forms.Button btn_box_write;
        private System.Windows.Forms.Button btn_box_read;
        private System.Windows.Forms.TextBox txt_box_id;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.ComboBox cbx_parity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_cycle_start;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btn_clear2;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btn_name_write;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.RichTextBox txt_log;
        private System.Windows.Forms.RichTextBox txt_log2;
    }
}