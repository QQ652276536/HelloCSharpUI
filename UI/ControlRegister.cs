using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using HelloCSharp.MyControl;

namespace HelloCSharp.UI
{
    /// <summary>
    /// 注册控件
    /// </summary>
    public partial class ControlRegister : UserControl
    {
        #region 自定义控件

        public HintTextBox textBox1;
        public HintTextBox textBox2;
        public HintTextBox textBox3;
        public HintTextBox textBox4;

        #endregion
        
        #region 变量

        public string _userName = "";
        public string _userPhone = "";
        public string _userPwd = "";
        public string _verifyCode = "";
        public bool _userNameFlag = true;
        public bool _userPhoneFlag = true;
        public bool _userPasswordFlag = true;
        public bool _chkReadFlag = true;
        public bool _smsFlag = true;
        public bool _smsBtnClickFlag = true;
        public int smsTime = 0;

        #endregion

        public ControlRegister()
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
            this.textBox1.HintText = "请设置用户名";
            this.textBox1.Size = new System.Drawing.Size(200, 29);
            this.textBox1.TabIndex = 1;
            this.textBox1.Click += new System.EventHandler(this.textBox1_Click);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // textBox2
            // 
            this.textBox2 = new HintTextBox();
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox2.Location = new System.Drawing.Point(90, 84);
            this.textBox2.MaxLength = 11;
            this.textBox2.Name = "textBox2";
            this.textBox2.HintText = "可用于登录和找回密码";
            this.textBox2.Size = new System.Drawing.Size(200, 29);
            this.textBox2.TabIndex = 2;
            this.textBox2.Click += new System.EventHandler(this.textBox2_Click);
            this.textBox2.Leave += new System.EventHandler(this.textBox2_Leave);
            // 
            // textBox3
            // 
            this.textBox3 = new HintTextBox();
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox3.Location = new System.Drawing.Point(90, 140);
            this.textBox3.MaxLength = 14;
            this.textBox3.Name = "textBox3";
            this.textBox3.HintText = "请设置登录密码";
            this.textBox3.PasswordChar = '●';
            this.textBox3.Size = new System.Drawing.Size(164, 29);
            this.textBox3.TabIndex = 3;
            this.textBox3.Click += new System.EventHandler(this.textBox3_Click);
            this.textBox3.Leave += new System.EventHandler(this.textBox3_Leave);
            // 
            // textBox4
            // 
            this.textBox4 = new HintTextBox();
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox4.Location = new System.Drawing.Point(90, 195);
            this.textBox4.MaxLength = 4;
            this.textBox4.Name = "textBox4";
            this.textBox4.HintText = "请输入验证码";
            this.textBox4.Size = new System.Drawing.Size(90, 29);
            this.textBox4.TabIndex = 4;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.BackgroundImage = Image.FromFile("../../Image/睁眼1.png");
            this.textBox3.PasswordChar = new char();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this.pictureBox1.BackgroundImage = Image.FromFile("../../Image/闭眼1.png");
            this.textBox3.PasswordChar = '●';
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            smsTime--;
            if (smsTime == 0)
            {
                timer1.Stop();
                _smsBtnClickFlag = true;
            }
            else
            {
                _smsBtnClickFlag = false;
            }
            ChangeSmsBtnState();

        }

        /// <summary>
        /// 改变短信验证按钮状态
        /// </summary>
        private void ChangeSmsBtnState()
        {
            if (_smsBtnClickFlag)
            {
                this.button2.Text = "获取短信验证";
                this.button2.Enabled = true;
                this.button2.BackColor = System.Drawing.Color.Transparent;
                this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            }
            else
            {
                this.button2.Enabled = false;
                this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
                this.button2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
                this.button2.Text = smsTime.ToString() + "秒后重新获取";
            }
        }

        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //间隔时间，秒
            smsTime = 9;
            //更改验证码按钮状态
            _smsBtnClickFlag = false;
            ChangeSmsBtnState();
            //启动定时
            timer1.Enabled = true;
            timer1.Start();
            //获取验证码
            GetSms getSMS = new GetSms(_userPhone);
            string sendSms = getSMS._code;
            //TODO:验证手机号是否已存在，如果已存在先提示他手机号已存在可直接登录，他点确认后跳转至登录页面
            if (sendSms.Equals("1"))
            {
            }
            else if (!sendSms.Equals("-1") && sendSms != null && !sendSms.Equals(""))
            {
                Console.WriteLine("收到验证码：" + sendSms);
            }
        }

        public Dictionary<string, string> GetRegisterParam()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("userName", _userName);
            dictionary.Add("userPhone", _userPhone);
            dictionary.Add("userPwd", _userPwd);
            dictionary.Add("verifyCode", _verifyCode);
            return dictionary;
        }

        public string GetRegisterParam(string param)
        {
            switch (param)
            {
                case "json":
                    UserInfo userInfo = new UserInfo();
                    userInfo._userName = _userName;
                    userInfo._userPhone = _userPhone;
                    userInfo._userPwd = _userPwd;
                    userInfo._verifyCode = Convert.ToInt32(_verifyCode);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string strJson = serializer.Serialize(userInfo);
                    return strJson;
                default:
                    return null;
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            SetVerifyUserNameColor(1);
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            SetVerifyUserPhoneColor(1);
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            SetVerifyUserPassword(1);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            SetVerifyUserNameColor(0);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            SetVerifyUserPhoneColor(0);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            SetVerifyUserPassword(0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetVerifyReadChecked();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http:www.baidu.com");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2提示超长、-3提示太受欢迎、-4提示不能为纯数字、1验证通过</returns>
        private int VerifyUserName(string str)
        {
            if (str.Trim().Equals(""))
            {
                _userNameFlag = false;
                return -1;
            }
            //ASCII码验证是否为纯数字
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = ascii.GetBytes(str);
            int numCount = 0;
            foreach (byte b in bytes)
            {
                //数字
                if (b >= 48 && b <= 57)
                {
                    numCount++;
                }
            }
            if (bytes.Length == numCount)
            {
                _userNameFlag = false;
                return -4;
            }
            int fontCNum = 0;
            int fontENum = 0;
            //英文范围0-127，汉字大于127
            for (int i = 0; i < str.Length; i++)
            {
                //是汉字
                if ((int)str[i] > 127)
                {
                    fontCNum++;
                }
                //英文
                if ((int)str[i] >= 0 && (int)str[i] <= 127)
                {
                    fontENum++;
                }
            }
            if (fontCNum > 7 || fontENum > 14)
            {
                _userNameFlag = false;
                return -2;
            }
            if (!Regex.IsMatch(str, @"^[\u4e00-\u9fa5]{2,7}$|^[\dA-Za-z]{4,14}"))
            {
                _userNameFlag = false;
                return -3;
            }
            return 1;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public void SetVerifyUserNameColor(int result)
        {
            _userName = textBox1.Text;
            if (result == 0)
            {
                result = VerifyUserName(_userName);
            }
            switch (result)
            {
                case -1:
                    label5.Text = "请您输入用户名";
                    label5.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label5.Text = "用户名不能超过7个汉字或14个字符";
                    label5.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case -3:
                    label5.Text = "此用户名太受欢迎,请更换一个";
                    label5.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case -4:
                    label5.Text = "用户名仅支持中英文、数字和下划线、且不能为纯数字";
                    label5.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    this.label5.Text = "设置后不可更改\r\n中英文均可，最长14个英文或7个汉字";
                    label5.ForeColor = SystemColors.AppWorkspace;
                    break;
            }
            label4.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2提示超长、1验证通过</returns>
        private int VerifyUserPhone(string str)
        {
            if (str.Trim().Equals(""))
            {
                _userPhoneFlag = false;
                return -1;
            }
            if (!Regex.IsMatch(str, @"^[1][3,4,5,7,8][0-9]{9}"))
            {
                _userPhoneFlag = false;
                return -2;
            }
            return 1;
        }

        /// <summary>
        /// 手机号
        /// </summary>
        public void SetVerifyUserPhoneColor(int result)
        {
            _userPhone = textBox2.Text;
            if (result == 0)
            {
                result = VerifyUserPhone(_userPhone);
            }
            switch (result)
            {
                case -1:
                    label6.Text = "请您输入手机号";
                    label6.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label6.Text = "手机号码格式不正确";
                    label6.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label6.Text = "请输入中国大陆手机号，其他用户不可见";
                    label6.ForeColor = SystemColors.AppWorkspace;
                    break;
            }
            label5.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2提示非法、1验证通过</returns>
        private int VerifyUserPassword(string str)
        {
            if (str.Trim().Equals(""))
            {
                _userPasswordFlag = false;
                return -1;
            }
            if (!Regex.IsMatch(str, @"^[~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[:;<,>\?\/""0-9a-zA-Z]{6,14}"))
            {
                _userPasswordFlag = false;
                return -2;
            }
            return 1;
        }

        /// <summary>
        /// 密码
        /// </summary>
        public void SetVerifyUserPassword(int result)
        {
            _userPwd = textBox3.Text;
            if (result == 0)
            {
                result = VerifyUserPassword(_userPwd);
            }
            switch (result)
            {
                case -1:
                    label7.Text = "请您输入密码";
                    label7.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label7.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label7.ForeColor = Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label7.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label7.ForeColor = SystemColors.AppWorkspace;
                    break;
            }
            label6.Invalidate();
        }

        /// <summary>
        /// 这个方法只在点击注册的时候触发
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2验证码错误、1验证通过</returns>
        private int VerifySMS(string str)
        {
            if (str.Trim().Equals(""))
            {
                _smsFlag = false;
                return -1;
            }
            //TODO:
            if (!str.Equals("正确的短信验证码"))
            {
                _smsFlag = false;
                return -2;
            }
            return 1;
        }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public void SetVerifySMS(int result)
        {
            _verifyCode = textBox4.Text;
            if (result == 0)
            {
                result = VerifySMS(_verifyCode);
            }
            switch (result)
            {
                case -1:
                    label8.Text = "请您输入验证码";
                    label8.Visible = true;
                    break;
                case -2:
                    label8.Text = "短信验证码错误";
                    label8.Visible = true;
                    break;
                case 1:
                    label8.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool VerifyReadChecked()
        {
            if (checkBox1.Checked)
            {
                _chkReadFlag = true;
            }
            else
            {
                _chkReadFlag = false;
            }
            return _chkReadFlag;
        }

        /// <summary>
        /// 协议
        /// </summary>
        public void SetVerifyReadChecked()
        {
            if (VerifyReadChecked())
            {
            }
            else
            {
            }
        }

    }
}
