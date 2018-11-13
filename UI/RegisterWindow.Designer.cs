using System.Drawing;
using System.Windows.Forms;

namespace OnlyEatNotWash
{
    partial class RegisterWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterWindow));
            this.panel1 = new Panel();
            this.panel2 = new Panel();
            this.label8 = new Label();
            this.label7 = new Label();
            this.label6 = new Label();
            this.label5 = new Label();
            this.linkLabel1 = new LinkLabel();
            this.checkBox1 = new CheckBox();
            this.button1 = new Button();
            this.textBox4 = new TextBox();
            this.label4 = new Label();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.textBox1 = new TextBox();
            this.textBox2 = new TextBox();
            this.textBox3 = new TextBox();
            this.button4 = new Button();
            this.panel3 = new Panel();
            this.panel4 = new Panel();
            this.button3 = new Button();
            this.button2 = new Button();
            this.panel2.SuspendLayout();
            this.textBox3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.LightGoldenrodYellow;
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(710, 60);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = Color.Transparent;
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.linkLabel1);
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.textBox4);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Dock = DockStyle.Fill;
            this.panel2.Location = new Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(710, 350);
            this.panel2.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.BackColor = Color.Transparent;
            this.label8.Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label8.Location = new Point(300, 200);
            this.label8.Name = "label8";
            this.label8.Size = new Size(110, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "短信激活码错误";
            this.label8.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = Color.Transparent;
            this.label7.Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = SystemColors.AppWorkspace;
            this.label7.Location = new Point(300, 137);
            this.label7.Name = "label7";
            this.label7.Size = new Size(188, 51);
            this.label7.TabIndex = 11;
            this.label7.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
            this.label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.BackColor = Color.Transparent;
            this.label6.Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = SystemColors.AppWorkspace;
            this.label6.Location = new Point(300, 88);
            this.label6.Name = "label6";
            this.label6.Size = new Size(228, 21);
            this.label6.TabIndex = 12;
            this.label6.Text = "请输入中国大陆手机号，其他用户不可见";
            this.label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BackColor = Color.Transparent;
            this.label5.Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = SystemColors.AppWorkspace;
            this.label5.Location = new Point(300, 26);
            this.label5.Name = "label5";
            this.label5.Size = new Size(219, 34);
            this.label5.TabIndex = 13;
            this.label5.Text = "设置后不可更改\r\n中英文均可，最长14个英文或7个汉字";
            this.label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.linkLabel1.BackColor = Color.Transparent;
            this.linkLabel1.Font = new Font("Microsoft YaHei", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.LinkArea = new LinkArea(7, 15);
            this.linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = Color.DeepSkyBlue;
            this.linkLabel1.Location = new Point(107, 250);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new Size(230, 17);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "我已阅读并接受《百度用户协议》";
            this.linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
            this.linkLabel1.UseCompatibleTextRendering = true;
            // 
            // checkBox1
            // 
            this.checkBox1.BackColor = Color.Transparent;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = CheckState.Checked;
            this.checkBox1.Font = new Font("SimSun", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.checkBox1.Location = new Point(90, 248);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(20, 20);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Font = new Font("Microsoft YaHei", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button1.Location = new Point(186, 195);
            this.button1.Name = "button1";
            this.button1.Size = new Size(104, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "获取短信验证码";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox4
            // 
            this.textBox4.BackColor = SystemColors.Control;
            this.textBox4.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.ForeColor = Color.DeepSkyBlue;
            this.textBox4.Location = new Point(90, 195);
            this.textBox4.MaxLength = 4;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Size(90, 29);
            this.textBox4.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.BackColor = Color.Transparent;
            this.label4.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label4.Location = new Point(26, 198);
            this.label4.Name = "label4";
            this.label4.Size = new Size(60, 21);
            this.label4.TabIndex = 10;
            this.label4.Text = "验证码";
            this.label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = Color.Transparent;
            this.label1.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label1.Location = new Point(26, 31);
            this.label1.Name = "label1";
            this.label1.Size = new Size(60, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名";
            this.label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = Color.Transparent;
            this.label2.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label2.Location = new Point(26, 88);
            this.label2.Name = "label2";
            this.label2.Size = new Size(60, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "手机号";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = Color.Transparent;
            this.label3.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label3.Location = new Point(26, 144);
            this.label3.Name = "label3";
            this.label3.Size = new Size(60, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "密   码";
            this.label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = SystemColors.Control;
            this.textBox1.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = Color.DeepSkyBlue;
            this.textBox1.Location = new Point(90, 28);
            this.textBox1.MaxLength = 14;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(200, 29);
            this.textBox1.TabIndex = 1;
            this.textBox1.Click += new System.EventHandler(this.textBox1_Click);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = SystemColors.Control;
            this.textBox2.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = Color.DeepSkyBlue;
            this.textBox2.Location = new Point(90, 84);
            this.textBox2.MaxLength = 11;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Size(200, 29);
            this.textBox2.TabIndex = 2;
            this.textBox2.Click += new System.EventHandler(this.textBox2_Click);
            this.textBox2.Leave += new System.EventHandler(this.textBox2_Leave);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = SystemColors.Control;
            this.textBox3.Controls.Add(this.button4);
            this.textBox3.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.ForeColor = Color.DeepSkyBlue;
            this.textBox3.Location = new Point(90, 140);
            this.textBox3.MaxLength = 14;
            this.textBox3.Name = "textBox3";
            this.textBox3.PasswordChar = '●';
            this.textBox3.Size = new Size(200, 29);
            this.textBox3.TabIndex = 3;
            this.textBox3.Click += new System.EventHandler(this.textBox3_Click);
            this.textBox3.Leave += new System.EventHandler(this.textBox3_Leave);
            // 
            // button4
            // 
            this.button4.BackgroundImage = ((Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = ImageLayout.Stretch;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = FlatStyle.Flat;
            this.button4.Location = new Point(158, 3);
            this.button4.Name = "button4";
            this.button4.Size = new Size(30, 20);
            this.button4.TabIndex = 15;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.MouseDown += new MouseEventHandler(this.button4_MouseDown);
            this.button4.MouseUp += new MouseEventHandler(this.button4_MouseUp);
            // 
            // panel3
            // 
            this.panel3.BackColor = Color.MediumSeaGreen;
            this.panel3.Dock = DockStyle.Right;
            this.panel3.Location = new Point(521, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(189, 350);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.BackColor = Color.MediumPurple;
            this.panel4.Controls.Add(this.button3);
            this.panel4.Controls.Add(this.button2);
            this.panel4.Dock = DockStyle.Bottom;
            this.panel4.Location = new Point(0, 350);
            this.panel4.Name = "panel4";
            this.panel4.Size = new Size(521, 60);
            this.panel4.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.BackColor = Color.Transparent;
            this.button3.FlatAppearance.MouseDownBackColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(144)))), ((int)(((byte)(0)))));
            this.button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(144)))), ((int)(((byte)(0)))));
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button3.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.button3.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button3.Location = new Point(200, 0);
            this.button3.Name = "button3";
            this.button3.Size = new Size(90, 30);
            this.button3.TabIndex = 8;
            this.button3.Text = "注册";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = Color.Transparent;
            this.button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(144)))), ((int)(((byte)(0)))));
            this.button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(144)))), ((int)(((byte)(0)))));
            this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.Font = new Font("Microsoft YaHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button2.Location = new Point(90, 0);
            this.button2.Name = "button2";
            this.button2.Size = new Size(90, 30);
            this.button2.TabIndex = 7;
            this.button2.Text = "登录";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // A
            // 
            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackgroundImage = ((Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.ClientSize = new Size(710, 410);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "A";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.textBox3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private TextBox textBox1;
        private Label label1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox3;
        private Label label3;
        private Button button1;
        private TextBox textBox4;
        private Label label4;
        private CheckBox checkBox1;
        private LinkLabel linkLabel1;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Button button3;
        private Button button2;
        private Button button4;
    }
}