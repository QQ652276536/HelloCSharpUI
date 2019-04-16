using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HelloCSharp.MyControl
{
    public partial class HintTextBox : TextBox
    {
        private Label hintLabel = null;
        //唉...只能用Picture代替Button了（黑线问题）
        private PictureBox clear = null;

        public HintTextBox()
        {
            InitializeComponent();
            InitHintLabel();
            InitClearBtn();
        }

        public HintTextBox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void InitHintLabel()
        {
            hintLabel = new Label();
            hintLabel.BorderStyle = BorderStyle.None;
            hintLabel.Enabled = false;
            hintLabel.BackColor = Color.Transparent;
            hintLabel.AutoSize = true;
            hintLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            base.Controls.Add(hintLabel);
        }

        private void InitClearBtn()
        {
            clear = new PictureBox();
            clear.Size = new Size(12, 12);
            clear.BackColor = Color.Transparent;
            clear.BackgroundImage = Image.FromFile("../../Image/关闭3.png");
            clear.BackgroundImageLayout = ImageLayout.Stretch;
            clear.Cursor = Cursors.Hand;
            clear.Click += new EventHandler(clearBtn_Click);
            clear.MouseHover += new EventHandler(clearBtn_MouseHover);
            clear.MouseLeave += new EventHandler(clearBtn_MouseLeave);
            clear.Visible = false;
            base.Controls.Add(clear);
        }

        private void clearBtn_Click(Object sender, EventArgs e)
        {
            base.Text = "";
            hintLabel.Visible = true;
            base.Focus();
        }

        private void clearBtn_MouseHover(Object sender, EventArgs e)
        {
            clear.BackgroundImage = Image.FromFile("../../Image/关闭4.png");
        }

        private void clearBtn_MouseLeave(Object sender, EventArgs e)
        {
            clear.BackgroundImage = Image.FromFile("../../Image/关闭3.png");
        }

        public string HintText
        {
            get
            {
                return hintLabel.Text;
            }
            set
            {
                hintLabel.Text = value;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text.Trim() != string.Empty)
                {
                    hintLabel.Visible = false;
                    clear.Visible = true;
                }
                else
                {
                    hintLabel.Visible = true;
                    clear.Visible = false;
                }
                base.Text = value;
            }
        }

        /// <summary>
        /// 控件大小改变时重新计算其它控件的坐标
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            int tempHeight = (int)((base.Height - hintLabel.Height) / 2) - 2;
            hintLabel.Location = new Point(2, tempHeight);
            tempHeight = (int)((base.Height - clear.Height) / 2) - 2;
            //注意,如果TextBox太小那么清除按钮的横向位置是负的
            int tempWidth = base.Width - clear.Width - 10;
            clear.Location = new Point(tempWidth, tempHeight);
            base.OnSizeChanged(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            if (base.Text.Trim() != string.Empty)
            {
                hintLabel.Visible = false;
                clear.Visible = true;
            }
            else
            {
                hintLabel.Visible = true;
                clear.Visible = false;
            }
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (base.Text.Trim() != string.Empty)
            {
                hintLabel.Visible = false;
                clear.Visible = true;
            }
            else
            {
                hintLabel.Visible = true;
                clear.Visible = false;
            }
            base.Text = base.Text.Trim();
            base.OnLeave(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (base.Text != string.Empty)
            {
                hintLabel.Visible = false;
                clear.Visible = true;
            }
            else
            {
                hintLabel.Visible = true;
                clear.Visible = false;
            }
            base.OnTextChanged(e);
        }

    }
}
