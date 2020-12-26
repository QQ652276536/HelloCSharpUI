using System;
using System.Windows.Forms;
namespace HelloCSharp.UI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btn_zm301_developer_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new Zm301Test().ShowDialog();
        }

        private void btn_sn_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new WriteSN().ShowDialog();
        }

        private void btn_imei_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new WriteIMEI().ShowDialog();
        }

        private void btn_zm301_factory_Click_1(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new Zm301SimpleTest().ShowDialog();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new LoginAndRegister().ShowDialog();
        }
    }
}
