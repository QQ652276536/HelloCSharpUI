using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class FileB : Form
    {
        private delegate void TextDele();
        private List<Thread> _threadList;

        public FileB()
        {
            InitializeComponent();
            _threadList = new List<Thread>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult diaologResult = folderBrowserDialog1.ShowDialog();
            if (diaologResult == DialogResult.OK)
            {
                label1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult diaologResult = folderBrowserDialog2.ShowDialog();
            if (diaologResult == DialogResult.OK)
            {
                label2.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (label1.Text.Equals("") || label2.Text.Equals(""))
            {
                MessageBox.Show("请选择源路径！");
                return;
            }
            textBox1.Text = "执行中……";
            textBox1.Select(textBox1.Text.Length, 0);
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            Thread thread = new Thread(new ThreadStart(CreateFileA));
            thread.Name = "thread_1";
            _threadList.Add(thread);
            thread.Start();
        }

        private void CreateFileA()
        {
            TextDele txtDele = new TextDele(CreateFileB);
            this.BeginInvoke(txtDele);
        }

        private void CreateFileB()
        {
            textBox1.Text = "";
            String inputPath = label1.Text;
            String outputPath = label2.Text;
            DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles();
            Regex regex = new Regex("[a-zA-Z][1]");
            try
            {
                for (int i = 0; i < fileInfoArray.Length; i++)
                {
                    String tempPath = fileInfoArray[i].FullName;
                    tempPath += "\r\n";
                    textBox1.Text += tempPath;
                    String tempNameA = fileInfoArray[i].Name;
                    if (regex.IsMatch(tempNameA))
                    {
                        int tempIndexA = tempNameA.IndexOf("1");
                        tempNameA = tempNameA.Remove(tempIndexA, 1);
                        tempNameA = tempNameA.Insert(tempIndexA, "");
                    }
                    String tempPathA = outputPath + "\\" + tempNameA;
                    fileInfoArray[i].MoveTo(tempPathA);
                }
                textBox1.Text += "完成!";
                textBox1.Select(textBox1.Text.Length, 0);
                textBox1.ScrollToCaret();
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch
            {
            }
        }

        private void FileB_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Thread tempTrhaed in _threadList)
            {
                tempTrhaed.Abort();
            }
        }
    }
}
