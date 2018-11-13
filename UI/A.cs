using MyDelegate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDelegate
{
    public delegate void MyDel();
}

namespace OnlyEatNotWash.UI
{
    public partial class A : Form
    {

        public A()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyDel myDel = new MyDel(ChangeColor);
            myDel += ChangeTitle;
            B b = new B(myDel);
            b.ShowDialog();
        }

        private void ChangeColor()
        {
            this.BackColor = Color.FromArgb(255, 0, 0);
        }

        private void ChangeTitle()
        {
            this.button1.Text = "窗体已经改变颜色了";
        }
    }
}
