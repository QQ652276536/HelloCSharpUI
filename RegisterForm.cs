using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Register
{
    public partial class RegisterForm : Form
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
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setVerifyUserNameColor();
            setVerifyUserPhoneColor();
            setVerifyUserPassword();
            setVerifySMS();
            setVerifyReadChecked();
            if (_userNameFlag && _userPhoneFlag && _userPasswordFlag && _smsFlag && _chkReadFlag)
            {
                //TODO:注册
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            setVerifyUserNameColor();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            setVerifyUserPhoneColor();
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            setVerifyUserPassword();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http:www.baidu.com");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
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

        private void setVerifyUserNameColor()
        {
            _userName = textBox1.Text;
            int result = verifyUserName(_userName);
            switch (result)
            {
                case -1:
                    label4.Text = "请您输入用户名";
                    label4.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label4.Text = "用户名不能超过7个汉字或14个字符";
                    label4.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -3:
                    label4.Text = "此用户名太受欢迎,请更换一个";
                    label4.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -4:
                    label4.Text = "用户名仅支持中英文、数字和下划线、且不能为纯数字";
                    label4.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    this.label4.Text = "设置后不可更改\r\n中英文均可，最长14个英文或7个汉字";
                    label4.ForeColor = System.Drawing.SystemColors.AppWorkspace;
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

        private void setVerifyUserPhoneColor()
        {
            _userPhone = textBox2.Text;
            int result = verifyUserPhone(_userPhone);
            switch (result)
            {
                case -1:
                    label5.Text = "请您输入手机号";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label5.Text = "手机号码格式不正确";
                    label5.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label5.Text = "请输入中国大陆手机号，其他用户不可见";
                    label5.ForeColor = System.Drawing.SystemColors.AppWorkspace;
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

        private void setVerifyUserPassword()
        {
            _userPassword = textBox3.Text;
            int result = verifyUserPassword(_userPassword);
            switch (result)
            {
                case -1:
                    label6.Text = "请输入密码";
                    label6.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case -2:
                    label6.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label6.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    break;
                case 1:
                    label6.Text = "长度为6~14个字符\r\n支持字数、大小写字母和标点符号\r\n不允许有空格";
                    label6.ForeColor = System.Drawing.SystemColors.AppWorkspace;
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

        private void setVerifySMS()
        {
            _verifyCode = textBox4.Text;
            int result = verifySMS(_verifyCode);
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

        private bool verifyReadChecked()
        {
            if (checkBox1.Checked)
            {
                _chkReadFlag = true;
            }
            return _chkReadFlag;
        }

        private void setVerifyReadChecked()
        {
            verifyReadChecked();
        }
    }
}
