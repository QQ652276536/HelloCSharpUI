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
            foreach (Control tempControl in flowLayoutPanel1.Controls)
            {
                tempControl.MouseHover += new EventHandler(panel_MouseHover);
                tempControl.MouseLeave += new EventHandler(panel_MouseLeave);
                tempControl.Click += new EventHandler(panel_Click);
            }
        }

        private void panel_MouseHover(object sender, EventArgs e)
        {
            Control panel = sender as Control;
            panel.BackColor = Color.FromArgb(50,255,144,0);
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            Control panel = sender as Control;
            panel.BackColor = Color.Transparent;
        }

        private void panel_Click(object sender, EventArgs e)
        {
            String panelName = ((Control)sender).Name;
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
