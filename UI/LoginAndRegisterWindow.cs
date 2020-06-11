using QRCoder;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class LoginAndRegisterWindow : Form
    {
        public int _nowPage = 0;
        public ControlRegister _registerControl = null;
        public ControlLogin _loginControl = null;

        public LoginAndRegisterWindow()
        {
            InitializeComponent();
            _loginControl = new ControlLogin();
            _registerControl = new ControlRegister();
            this.panel2.Controls.Add(_loginControl);
            this.panel2.Controls.Add(_registerControl);
            CreateQRCode();
        }

        /// <summary>
        /// 点击登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //当前在登录页面，点击登录则是登录
            if (_nowPage == 0)
            {
                //TODO:登录
            }
            //当前在注册页面，点击登录则是转到登录页面
            else if (_nowPage == 1)
            {
                ChangeNowPage();
            }
        }

        /// <summary>
        /// 点击注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //当前在登录页面，点击注册则是转到注册页面
            if (_nowPage == 0)
            {
                ChangeNowPage();
            }
            //当前在注册页面，点击注册则是注册
            else if (_nowPage == 1)
            {
                _registerControl.SetVerifyUserNameColor(0);
                _registerControl.SetVerifyUserPhoneColor(0);
                _registerControl.SetVerifyUserPassword(0);
                _registerControl.SetVerifySMS(0);
                _registerControl.SetVerifyReadChecked();
                if (_registerControl._userNameFlag && _registerControl._userPhoneFlag && _registerControl._userPasswordFlag
                    && _registerControl._smsFlag && _registerControl._chkReadFlag)
                {
                    Register register = new Register(_registerControl.GetRegisterParam("json"));
                }
            }
        }

        /// <summary>
        /// 切换登录、注册页面
        /// </summary>
        private void ChangeNowPage()
        {
            //在登录页面点击注册跳转到注册页面
            if (_nowPage == 0)
            {
                _registerControl.Visible = true;
                _loginControl.Visible = false;
                _nowPage = 1;
            }
            //在注册页面点击登录跳转到登录页面
            else if (_nowPage == 1)
            {
                _loginControl.Visible = true;
                _registerControl.Visible = false;
                _nowPage = 0;
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        private void CreateQRCode()
        {
            QRCodeGenerator codeGenerator = new QRCodeGenerator();
            QRCodeData codeData = codeGenerator.CreateQrCode("www.nullpointer.vip", QRCodeGenerator.ECCLevel.M, true, true, QRCodeGenerator.EciMode.Utf8, 1);
            QRCode code = new QRCode(codeData);
            //图标路径
            Bitmap icon = new Bitmap("../../Image/QQ.JPG");
            Bitmap bitmap = code.GetGraphic(7, Color.FromArgb(255, 128, 0), Color.White, icon, 15, 1, false);
            Image image = Image.FromHbitmap(bitmap.GetHbitmap());
            this.pictureBox1.Image = image;
        }

        /// <summary>
        /// 跳转至我的网站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("chrome.exe", "http://www.nullpointer.vip");
        }

    }
}
