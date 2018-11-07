using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Login
{
    public partial class A : Form
    {
        String _userName = "";
        String _userPhone = "";
        String _userPassword = "";
        String _verifyCode = "";
        bool _userNameFlag = false;
        bool _userPhoneFlag = false;
        bool _userPasswordFlag = false;
        bool _chkReadFlag = false;
        bool _smsFlag = false;
        int _nowPage = 0;

        public A()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO:发起验证码请求
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////登录
            //if (_nowPage == 0)
            //{
            //}
            ////切换至登录页面
            //else if (_nowPage == 1)
            //{
            //    changeNowPage();
            //    _nowPage = 0;
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ////切换至注册页面
            //if (_nowPage == 0)
            //{
            //    changeNowPage();
            //    _nowPage = 1;
            //}
            ////注册
            //else if (_nowPage == 1)
            //{
                setVerifyUserNameColor(0);
                setVerifyUserPhoneColor(0);
                setVerifyUserPassword(0);
                setVerifySMS(0);
                setVerifyReadChecked();
                if (_userNameFlag && _userPhoneFlag && _userPasswordFlag && _smsFlag && _chkReadFlag)
                {
                    //TODO:注册
                }
            //}
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            this.button4.BackgroundImage = Image.FromFile("../../Image/眼睛_显示_o.png");
            this.textBox3.PasswordChar = new char();
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            this.button4.BackgroundImage = Image.FromFile("../../Image/眼睛_隐藏_o.png");
            this.textBox3.PasswordChar = '●';
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            setVerifyUserNameColor(1);
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            setVerifyUserPhoneColor(1);
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            setVerifyUserPassword(1);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            setVerifyUserNameColor(0);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            setVerifyUserPhoneColor(0);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            setVerifyUserPassword(0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            setVerifyReadChecked();
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
        private int verifyUserName(String str)
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
        private void setVerifyUserNameColor(int result)
        {
            _userName = textBox1.Text;
            if (result == 0)
            {
                result = verifyUserName(_userName);
            }
            switch (result)
            {
                case -1:
                    label5.Text = "请您输入用户名";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label5.Text = "用户名不能超过7个汉字或14个字符";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -3:
                    label5.Text = "此用户名太受欢迎,请更换一个";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -4:
                    label5.Text = "用户名仅支持中英文、数字和下划线、且不能为纯数字";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    this.label5.Text = "设置后不可更改\r\n中英文均可，最长14个英文或7个汉字";
                    label5.ForeColor = System.Drawing.SystemColors.AppWorkspace;
                    break;
            }
            label4.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2提示超长、1验证通过</returns>
        private int verifyUserPhone(String str)
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
        private void setVerifyUserPhoneColor(int result)
        {
            _userPhone = textBox2.Text;
            if (result == 0)
            {
                result = verifyUserPhone(_userPhone);
            }
            switch (result)
            {
                case -1:
                    label6.Text = "请您输入手机号";
                    label6.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label6.Text = "手机号码格式不正确";
                    label6.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label6.Text = "请输入中国大陆手机号，其他用户不可见";
                    label6.ForeColor = System.Drawing.SystemColors.AppWorkspace;
                    break;
            }
            label5.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2提示非法、1验证通过</returns>
        private int verifyUserPassword(String str)
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
        private void setVerifyUserPassword(int result)
        {
            _userPassword = textBox3.Text;
            if (result == 0)
            {
                result = verifyUserPassword(_userPassword);
            }
            switch (result)
            {
                case -1:
                    label7.Text = "请输入密码";
                    label7.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label7.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label7.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label7.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label7.ForeColor = System.Drawing.SystemColors.AppWorkspace;
                    break;
            }
            label6.Invalidate();
        }

        /// <summary>
        /// 这个方法只在点击注册的时候触发
        /// </summary>
        /// <param name="str"></param>
        /// <returns>-1提示为空，-2验证码错误、1验证通过</returns>
        private int verifySMS(String str)
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
        private void setVerifySMS(int result)
        {
            _verifyCode = textBox4.Text;
            if(result == 0)
            {
                result = verifySMS(_verifyCode);
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
        private bool verifyReadChecked()
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
        private void setVerifyReadChecked()
        {
            if (verifyReadChecked())
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// 切换登录、注册页面
        /// </summary>
        private void changeNowPage()
        {
            if (_nowPage == 0)
            {
                panel2.Show();
            }
            else if (_nowPage == 1)
            {
                panel2.Hide();
            }
        }

    }
}
