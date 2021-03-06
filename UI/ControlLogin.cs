﻿using System;
using System.Drawing;
using System.Windows.Forms;
using HelloCSharp.MyControl;

namespace HelloCSharp.UI
{
    /// <summary>
    /// 登录控件
    /// </summary>
    public partial class ControlLogin : UserControl
    {
        #region 自定义控件

        public HintTextBox textBox1;
        public HintTextBox textBox2;
        public HintTextBox textBox3;

        #endregion

        VerifyImage _verifyImage;
        string _verifyCode;

        public ControlLogin()
        {
            InitializeComponent();
            InitializeMyComponent();
        }

        private void InitializeMyComponent()
        {
            // 
            // textBox1
            // 
            this.textBox1 = new HintTextBox();
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox1.Location = new System.Drawing.Point(90, 28);
            this.textBox1.MaxLength = 14;
            this.textBox1.Name = "textBox1";
            this.textBox1.HintText = "手机/邮箱/用户名";
            this.textBox1.Size = new System.Drawing.Size(200, 29);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2 = new HintTextBox();
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox2.Location = new System.Drawing.Point(90, 84);
            this.textBox2.MaxLength = 14;
            this.textBox2.Name = "textBox2";
            this.textBox2.HintText = "密码";
            this.textBox2.Size = new System.Drawing.Size(200, 29);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3 = new HintTextBox();
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox3.Location = new System.Drawing.Point(90, 138);
            this.textBox3.MaxLength = 6;
            this.textBox3.Name = "textBox3";
            this.textBox3.HintText = "验证码";
            this.textBox3.Size = new System.Drawing.Size(80, 29);
            this.textBox3.TabIndex = 3;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox3);
            CreateVerifyImage();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            CreateVerifyImage();
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        private string CreateVerifyImage()
        {
            _verifyImage = new VerifyImage();
            _verifyImage.ImageSize = pictureBox1.Size;
            _verifyImage.CreateImage();
            _verifyCode = _verifyImage.Text;
            Console.WriteLine(_verifyCode);
            System.Drawing.Bitmap bitmap = _verifyImage.Image;
            Image image = Image.FromHbitmap(bitmap.GetHbitmap());
            this.pictureBox1.Image = image;
            return _verifyCode;
        }

    }
}
