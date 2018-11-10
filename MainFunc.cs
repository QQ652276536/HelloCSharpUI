using System;
using System.Windows.Forms;

namespace OnlyEatNotWash
{
    class MainFunc
    {
        public void Test()
        {
            FileUtils.IP = "127.0.0.1";
            FileUtils.Port = 60000;
            if (FileUtils.TestConnection("test message"))
            {
                int result = FileUtils.StartSend("D://WorkSpace//Image//Photo//QQ头像.JPG", "QQ头像.JPG");
                if (result == 1)
                    MessageBox.Show("文件发送成功！");
                else if (result == -1)
                    MessageBox.Show("文件不存在！");
                else if (result == -2)
                    MessageBox.Show("连接失败！");
                else if (result == -3)
                    MessageBox.Show("IO异常！");
                else if (result == -4)
                    MessageBox.Show("未知异常！");
            }
            else
            {
                MessageBox.Show("无法连接服务器！");
            }
            //String fileContent = FileUtils.fileContent;
            //String content = "中华人民共和国";
            //FileUtils.Send(content+ "◎" + fileContent);

        }

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
            new MainFunc().Test();
        }

    }
}
