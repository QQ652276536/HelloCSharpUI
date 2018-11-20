using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
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
            String panelName = control.Name;
            String typeName = control.GetType().Name;
            if (typeName.Equals("Label"))
            {
                panelName = control.Parent.Name;
                Console.WriteLine("-------------------------");
            }
            switch (panelName)
            {
                case "panFileA":
                    break;
                case "panFileB":
                    break;
                case "panFileC":
                    break;
            }
        }
    }
}
