using System;
using System.Windows.Forms;

namespace OnlyEatNotWash
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new LoginForm());
            //Application.Run(new RegisterForm());
            //Application.Run(new A());

            //注意此处的测试内容需和服务端保持一致  否则无法判断是否成功
            if (FileUtils.TestConnection("test message"))
            {
                int result = FileUtils.StartSend("D://WorkSpace//Image//Photo//QQ头像.JPG", "QQ头像.JPG");
                if (result == 0)
                {
                    MessageBox.Show("文件发送成功！");
                }
                else if (result == -1)
                {
                    MessageBox.Show("文件不存在！");

                }
                else if (result == -2)
                {
                    MessageBox.Show("连接失败！");

                }
                else if (result == -3)
                {
                    MessageBox.Show("IO异常！");

                }
                else if (result == -4)
                {
                    MessageBox.Show("未知异常！");

                }
            }
            else
            {
                MessageBox.Show("无法连接服务器！");
            }

        }
    }
}
