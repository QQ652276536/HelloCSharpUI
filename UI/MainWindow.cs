using System;
using System.Drawing;
using System.Windows.Forms;
using TestIMEI;
using TestTools;

namespace HelloCSharp.UI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            PanelsRegisterEvents();
        }

        private void PanelsRegisterEvents()
        {
            foreach (Control panControl in flowLayoutPanel1.Controls)
            {
                panControl.MouseHover += new EventHandler(panel_MouseHover);
                panControl.MouseLeave += new EventHandler(panel_MouseLeave);
                panControl.Click += new EventHandler(panel_Click);
                foreach (Control labControl in panControl.Controls)
                {
                    labControl.MouseHover += new EventHandler(panel_MouseHover);
                    labControl.Click += new EventHandler(panel_Click);
                }
            }
        }

        private void panel_MouseHover(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Type type = control.GetType();
            if (type.Name.Equals("Label"))
            {
                control.BackColor = Color.Transparent;//屏蔽Label的背景色
                control.Parent.BackColor = Color.FromArgb(50, 255, 144, 0);
            }
            else
            {
                control.BackColor = Color.FromArgb(50, 255, 144, 0);
            }
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            panel.BackColor = Color.Transparent;
        }

        private void panel_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            string panelName = control.Name;
            string typeName = control.GetType().Name;
            if (typeName.Equals("Label"))
            {
                panelName = control.Parent.Name;
                Console.WriteLine("啊~~~~~~" + panelName);
            }
            switch (panelName)
            {
                case "panel1":
                    FileAWindow fileA = new FileAWindow();
                    this.StartPosition = FormStartPosition.CenterParent;
                    fileA.ShowDialog();
                    break;
                case "panel2":
                    this.StartPosition = FormStartPosition.CenterParent;
                    FileBWindow fileB = new FileBWindow();
                    fileB.ShowDialog();
                    break;
                case "panel3":
                    this.StartPosition = FormStartPosition.CenterParent;
                    TestCOMWindow testCOMWindow = new TestCOMWindow();
                    testCOMWindow.ShowDialog();
                    break;
                case "panel4":
                    this.StartPosition = FormStartPosition.CenterParent;
                    TestIMEIWindow testIMEIWindow = new TestIMEIWindow();
                    testIMEIWindow.ShowDialog();
                    break;
            }
        }

    }
}
