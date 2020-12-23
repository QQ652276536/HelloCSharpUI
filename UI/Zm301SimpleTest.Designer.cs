namespace HelloCSharp.UI
{
    partial class Zm301SimpleTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Zm301SimpleTest));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbx_parity = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbx_data = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_rate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_open = new System.Windows.Forms.Button();
            this.cbx_port = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lab_elec = new System.Windows.Forms.Label();
            this.lab_sensor = new System.Windows.Forms.Label();
            this.lab_gps = new System.Windows.Forms.Label();
            this.lab_lock = new System.Windows.Forms.Label();
            this.lab_door = new System.Windows.Forms.Label();
            this.lab_ver = new System.Windows.Forms.Label();
            this.chk_cycle = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chk_cycle);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 108);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "串口配置";
            // 
            // cbx_parity
            // 
            this.cbx_parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_parity.Enabled = false;
            this.cbx_parity.FormattingEnabled = true;
            this.cbx_parity.Location = new System.Drawing.Point(267, 65);
            this.cbx_parity.Name = "cbx_parity";
            this.cbx_parity.Size = new System.Drawing.Size(80, 24);
            this.cbx_parity.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "校验位：";
            // 
            // cbx_data
            // 
            this.cbx_data.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_data.Enabled = false;
            this.cbx_data.FormattingEnabled = true;
            this.cbx_data.Location = new System.Drawing.Point(267, 23);
            this.cbx_data.Name = "cbx_data";
            this.cbx_data.Size = new System.Drawing.Size(80, 24);
            this.cbx_data.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(195, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "数据位：";
            // 
            // cbx_rate
            // 
            this.cbx_rate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_rate.Enabled = false;
            this.cbx_rate.FormattingEnabled = true;
            this.cbx_rate.Location = new System.Drawing.Point(83, 65);
            this.cbx_rate.Name = "cbx_rate";
            this.cbx_rate.Size = new System.Drawing.Size(80, 24);
            this.cbx_rate.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "波特率：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "串口：";
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(390, 22);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(100, 24);
            this.btn_open.TabIndex = 5;
            this.btn_open.Text = "打开串口";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // cbx_port
            // 
            this.cbx_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_port.FormattingEnabled = true;
            this.cbx_port.Location = new System.Drawing.Point(83, 23);
            this.cbx_port.Name = "cbx_port";
            this.cbx_port.Size = new System.Drawing.Size(80, 24);
            this.cbx_port.TabIndex = 6;
            this.cbx_port.SelectedIndexChanged += new System.EventHandler(this.cbx_port_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.lab_elec);
            this.groupBox3.Controls.Add(this.lab_sensor);
            this.groupBox3.Controls.Add(this.lab_gps);
            this.groupBox3.Controls.Add(this.lab_lock);
            this.groupBox3.Controls.Add(this.lab_door);
            this.groupBox3.Controls.Add(this.lab_ver);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(12, 149);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(513, 153);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "测试结果";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "版本：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(243, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 16);
            this.label9.TabIndex = 14;
            this.label9.Text = "传感器：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "门状态：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(243, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "电池电压：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "GPS状态：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(243, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "开锁：";
            // 
            // lab_elec
            // 
            this.lab_elec.AutoSize = true;
            this.lab_elec.Location = new System.Drawing.Point(337, 112);
            this.lab_elec.Name = "lab_elec";
            this.lab_elec.Size = new System.Drawing.Size(40, 16);
            this.lab_elec.TabIndex = 9;
            this.lab_elec.Text = "null";
            // 
            // lab_sensor
            // 
            this.lab_sensor.AutoSize = true;
            this.lab_sensor.Location = new System.Drawing.Point(337, 32);
            this.lab_sensor.Name = "lab_sensor";
            this.lab_sensor.Size = new System.Drawing.Size(40, 16);
            this.lab_sensor.TabIndex = 8;
            this.lab_sensor.Text = "null";
            // 
            // lab_gps
            // 
            this.lab_gps.AutoSize = true;
            this.lab_gps.Location = new System.Drawing.Point(89, 112);
            this.lab_gps.Name = "lab_gps";
            this.lab_gps.Size = new System.Drawing.Size(40, 16);
            this.lab_gps.TabIndex = 7;
            this.lab_gps.Text = "null";
            // 
            // lab_lock
            // 
            this.lab_lock.AutoSize = true;
            this.lab_lock.Location = new System.Drawing.Point(337, 72);
            this.lab_lock.Name = "lab_lock";
            this.lab_lock.Size = new System.Drawing.Size(40, 16);
            this.lab_lock.TabIndex = 6;
            this.lab_lock.Text = "null";
            // 
            // lab_door
            // 
            this.lab_door.AutoSize = true;
            this.lab_door.Location = new System.Drawing.Point(89, 72);
            this.lab_door.Name = "lab_door";
            this.lab_door.Size = new System.Drawing.Size(40, 16);
            this.lab_door.TabIndex = 5;
            this.lab_door.Text = "null";
            // 
            // lab_ver
            // 
            this.lab_ver.AutoSize = true;
            this.lab_ver.Location = new System.Drawing.Point(89, 32);
            this.lab_ver.Name = "lab_ver";
            this.lab_ver.Size = new System.Drawing.Size(40, 16);
            this.lab_ver.TabIndex = 4;
            this.lab_ver.Text = "null";
            // 
            // chk_cycle
            // 
            this.chk_cycle.AutoSize = true;
            this.chk_cycle.Location = new System.Drawing.Point(392, 67);
            this.chk_cycle.Name = "chk_cycle";
            this.chk_cycle.Size = new System.Drawing.Size(91, 20);
            this.chk_cycle.TabIndex = 9;
            this.chk_cycle.Text = "循环测试";
            this.chk_cycle.UseVisualStyleBackColor = true;
            this.chk_cycle.CheckedChanged += new System.EventHandler(this.chk_cycle_CheckedChanged);
            // 
            // Zm301SimpleTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 319);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Zm301SimpleTest";
            this.Text = "Zm301SimpleTest";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbx_parity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbx_data;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_rate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.ComboBox cbx_port;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lab_ver;
        private System.Windows.Forms.Label lab_sensor;
        private System.Windows.Forms.Label lab_gps;
        private System.Windows.Forms.Label lab_lock;
        private System.Windows.Forms.Label lab_door;
        private System.Windows.Forms.Label lab_elec;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chk_cycle;
    }
}