using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            //DialogResult diaologResult = folderBrowserDialog2.ShowDialog();
            //if (diaologResult == DialogResult.OK)
            //{
            //    label1.Text = folderBrowserDialog2.SelectedPath;
            //}
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                label1.Text = openFileDialog.FileName;
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
            //Thread thread = new Thread(new ThreadStart(CreateFileA));
            Thread thread = new Thread(new ThreadStart(AnalysisHtml));
            thread.Name = "thread_1";
            _threadList.Add(thread);
            thread.Start();
        }

        private void AnalysisHtml()
        {
            TextDele txtDele = new TextDele(AnalysisAndCreate);
            this.BeginInvoke(txtDele);
        }

        /// <summary>
        /// 解析Html文件
        /// </summary>
        private void AnalysisAndCreate()
        {
            textBox1.Text = "";
            String inputPath = label1.Text;
            String outputPath = label2.Text;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
                String fileName = directoryInfo.Name;
                //读取文件内容
                StreamReader streanReader = new StreamReader(directoryInfo.ToString(), Encoding.Default);
                String content = streanReader.ReadToEnd();
                int startIndex1 = content.IndexOf("<ul id=\"resources\">");
                content = content.Substring(startIndex1);
                int startIndex2 = content.IndexOf("<div class=\"footer\">");
                content = content.Substring(0, startIndex2);
                AnalysisError(startIndex1, "A1");
                AnalysisError(startIndex2, "A2");
                String[] ulNodeArray = content.Split(new String[] { "<ul class=\"operations\">" }, StringSplitOptions.None);
                for (int i = 0; i < ulNodeArray.Length; i++)
                {
                    //第一个元素并不包含我想要的信息
                    if (i == 0)
                    {
                        continue;
                    }
                    String tempStr = ulNodeArray[i];
                    //获取请求方式
                    int tempStartIndex = tempStr.IndexOf("class=\"toggleOperation\">");
                    AnalysisError(tempStartIndex, "B1");
                    String tempSubContent1 = tempStr.Substring(tempStartIndex + 24);
                    int a = 1;
                }

                textBox1.Text += "完成!";
                textBox1.Select(textBox1.Text.Length, 0);
                textBox1.ScrollToCaret();
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void AnalysisError(int param1, String param2 = "X")
        {
            if (param1 <= 0)
            {
                MessageBox.Show("解析" + param2 + "处失败！");
                Application.Exit();
            }
        }

        private void CreateFileA()
        {
            TextDele txtDele = new TextDele(CreateFileB);
            this.BeginInvoke(txtDele);
        }

        /// <summary>
        /// 重命名文件并修改文件内容
        /// </summary>
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
