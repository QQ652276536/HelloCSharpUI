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

        private void button1_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new Zm301Test().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new WriteIMEI().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new WriteSN().ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new LoginAndRegister().ShowDialog();
        }
    }
}
