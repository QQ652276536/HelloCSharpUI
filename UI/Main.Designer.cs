using System.Drawing;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.group = new System.Windows.Forms.GroupBox();
            this.btn_zm301_factory = new System.Windows.Forms.Button();
            this.btn_sn = new System.Windows.Forms.Button();
            this.btn_imei = new System.Windows.Forms.Button();
            this.btn_zm301_developer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_login = new System.Windows.Forms.Button();
            this.group.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // group
            // 
            this.group.BackColor = System.Drawing.Color.Transparent;
            this.group.Controls.Add(this.btn_zm301_factory);
            this.group.Controls.Add(this.btn_sn);
            this.group.Controls.Add(this.btn_imei);
            this.group.Controls.Add(this.btn_zm301_developer);
            this.group.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.group.ForeColor = System.Drawing.Color.White;
            this.group.Location = new System.Drawing.Point(12, 15);
            this.group.Name = "group";
            this.group.Size = new System.Drawing.Size(696, 130);
            this.group.TabIndex = 0;
            this.group.TabStop = false;
            this.group.Text = "串口";
            // 
            // btn_zm301_factory
            // 
            this.btn_zm301_factory.BackColor = System.Drawing.Color.Transparent;
            this.btn_zm301_factory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_zm301_factory.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_zm301_factory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_zm301_factory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_zm301_factory.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_zm301_factory.ForeColor = System.Drawing.Color.White;
            this.btn_zm301_factory.Location = new System.Drawing.Point(54, 78);
            this.btn_zm301_factory.Name = "btn_zm301_factory";
            this.btn_zm301_factory.Size = new System.Drawing.Size(159, 30);
            this.btn_zm301_factory.TabIndex = 3;
            this.btn_zm301_factory.Text = "ZM301工厂测试";
            this.btn_zm301_factory.UseVisualStyleBackColor = false;
            this.btn_zm301_factory.Click += new System.EventHandler(this.btn_zm301_factory_Click_1);
            // 
            // btn_sn
            // 
            this.btn_sn.BackColor = System.Drawing.Color.Transparent;
            this.btn_sn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_sn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_sn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_sn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_sn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_sn.ForeColor = System.Drawing.Color.White;
            this.btn_sn.Location = new System.Drawing.Point(54, 25);
            this.btn_sn.Name = "btn_sn";
            this.btn_sn.Size = new System.Drawing.Size(90, 30);
            this.btn_sn.TabIndex = 0;
            this.btn_sn.Text = "SN写入";
            this.btn_sn.UseVisualStyleBackColor = false;
            this.btn_sn.Click += new System.EventHandler(this.btn_sn_Click);
            // 
            // btn_imei
            // 
            this.btn_imei.BackColor = System.Drawing.Color.Transparent;
            this.btn_imei.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_imei.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_imei.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_imei.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_imei.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_imei.ForeColor = System.Drawing.Color.White;
            this.btn_imei.Location = new System.Drawing.Point(192, 25);
            this.btn_imei.Name = "btn_imei";
            this.btn_imei.Size = new System.Drawing.Size(90, 30);
            this.btn_imei.TabIndex = 1;
            this.btn_imei.Text = "IMEI写入";
            this.btn_imei.UseVisualStyleBackColor = false;
            this.btn_imei.Click += new System.EventHandler(this.btn_imei_Click);
            // 
            // btn_zm301_developer
            // 
            this.btn_zm301_developer.BackColor = System.Drawing.Color.Transparent;
            this.btn_zm301_developer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_zm301_developer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_zm301_developer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_zm301_developer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_zm301_developer.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_zm301_developer.ForeColor = System.Drawing.Color.White;
            this.btn_zm301_developer.Location = new System.Drawing.Point(342, 25);
            this.btn_zm301_developer.Name = "btn_zm301_developer";
            this.btn_zm301_developer.Size = new System.Drawing.Size(172, 30);
            this.btn_zm301_developer.TabIndex = 2;
            this.btn_zm301_developer.Text = "ZM301压力测试";
            this.btn_zm301_developer.UseVisualStyleBackColor = false;
            this.btn_zm301_developer.Click += new System.EventHandler(this.btn_zm301_developer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btn_login);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(695, 126);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务";
            // 
            // btn_login
            // 
            this.btn_login.BackColor = System.Drawing.Color.Transparent;
            this.btn_login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_login.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_login.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_login.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_login.ForeColor = System.Drawing.Color.White;
            this.btn_login.Location = new System.Drawing.Point(54, 34);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(90, 30);
            this.btn_login.TabIndex = 4;
            this.btn_login.Text = "登录";
            this.btn_login.UseVisualStyleBackColor = false;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(720, 321);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.group);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "工具箱";
            this.group.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox group;
        private Button btn_sn;
        private Button btn_imei;
        private Button btn_zm301_developer;
        private GroupBox groupBox1;
        private Button btn_login;
        private Button btn_zm301_factory;
    }
}