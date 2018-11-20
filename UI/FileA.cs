using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class FileA : Form
    {
        private delegate void TextDele();
        private List<Thread> _threadList;

        public FileA()
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
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Text = "执行中……";
            textBox1.Select(textBox1.Text.Length, 0);
            textBox1.ScrollToCaret();
            Thread thread = new Thread(new ThreadStart(CreateFileA));
            _threadList.Add(thread);
            thread.Start();
        }

        private void CreateFileA()
        {
            TextDele textDele = new TextDele(CreateFileB);
            this.BeginInvoke(textDele);
        }

        private void CreateFileB()
        {
            textBox1.Text = "";
            String inputPath = label1.Text;
            String outputPath = label2.Text;
            DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles();
            FileStream fileStream = null;
            StreamReader streamReader = null;
            StreamWriter streamWriter = null;
            try
            {
                for (int i = 0; i < fileInfoArray.Length; i++)
                {
                    String tempPath = fileInfoArray[i].FullName;
                    textBox1.Text += tempPath + "\r\n";
                    streamReader = new StreamReader(tempPath);
                    streamWriter = new StreamWriter(outputPath + "\\" + fileInfoArray[i].Name);
                    int lineCount = GetLineCount(tempPath);
                    if (lineCount <= 0)
                    {
                        continue;
                    }
                    int lineIndex = 0;
                    String nextLine;
                    while ((nextLine = streamReader.ReadLine()) != null)
                    {
                        if (lineIndex > 1 && lineIndex < lineCount - 1)
                        {
                            String[] contentArray = nextLine.Split(new String[] { "," }, StringSplitOptions.None);
                            int arrayLength = contentArray.Length;
                            String dateStr1 = contentArray[0];
                            String dateStr2 = dateStr1.Replace('/', '-');
                            String lineStr = "";
                            if (arrayLength == 7)
                            {
                                lineStr = dateStr2 + "," + contentArray[1] + "," + contentArray[2] + "," + contentArray[3] + ","
                                        + contentArray[4] + "," + contentArray[5] + "," + contentArray[6] + ",0,0";
                            }
                            else if (arrayLength == 8)
                            {
                                lineStr = dateStr2 + "," + contentArray[1] + "," + contentArray[2] + "," + contentArray[3] + ","
                                        + contentArray[4] + "," + contentArray[5] + "," + contentArray[6] + "," + contentArray[7]
                                        + ",0";
                            }
                            else if (arrayLength == 9)
                            {
                                String str1 = contentArray[1];
                                StringBuilder stringBuilder = new StringBuilder(str1);
                                stringBuilder.Insert(2, ":");
                                stringBuilder.Insert(5, ":00");
                                lineStr = dateStr2 + "," + stringBuilder.ToString() + "," + contentArray[2] + "," + contentArray[3] + ","
                                        + contentArray[4] + "," + contentArray[5] + "," + contentArray[6] + "," + contentArray[7] + ","
                                        + contentArray[8] + ",0,0";
                            }
                            streamWriter.WriteLine(lineStr);
                            streamWriter.Flush();
                        }
                        lineIndex++;
                    }
                }
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                textBox1.Text += "完成!";
                textBox1.Select(textBox1.Text.Length, 0);
                textBox1.ScrollToCaret();
            }
            catch
            {
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (streamReader != null)
                    streamReader.Close();
                if (streamWriter != null)
                    streamWriter.Close();
            }
        }

        private int GetLineCount(String path)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            int lineCount = 0;
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream, Encoding.Default);
                String lineContent;
                while ((lineContent = streamReader.ReadLine()) != null)
                {
                    lineCount++;
                }
            }
            catch
            {
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (streamReader != null)
                    streamReader.Close();
            }
            return lineCount;
        }

        private void FileA_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Thread tempTrhead in _threadList)
            {
                tempTrhead.Abort();
            }
        }
    }
}
