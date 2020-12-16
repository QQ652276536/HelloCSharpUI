using System;
using System.Windows.Forms;
namespace HelloCSharp.UI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            new Zm301TestWidnow().ShowDialog();
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
            new LoginAndRegisterWindow().ShowDialog();
        }
    }
}
