using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            TextDele txtDele = new TextDele(AnalysisJson);
            this.BeginInvoke(txtDele);
        }

        /// <summary>
        /// 解析Html文件
        /// </summary>
        private void AnalysisJson()
        {
            textBox1.Text = "";
            string inputPath = label1.Text;
            string outputPath = label2.Text;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
                string fileName = directoryInfo.Name;
                //读取文件内容
                StreamReader streanReader = new StreamReader(directoryInfo.ToString(), Encoding.UTF8);
                string content = streanReader.ReadToEnd();
                //反序列化Json
                MyJsonClass myJsonClass = JsonConvert.DeserializeObject<MyJsonClass>(content);
                List<HttpEntity> httpEntityList = myJsonClass.item;
                int index = 0;
                string tempFileName = Path.GetFileNameWithoutExtension(inputPath);
                index = tempFileName.IndexOf(".postman_collection");
                tempFileName = tempFileName.Substring(0, index);
                foreach (HttpEntity tempHttpEntity in httpEntityList)
                {
                    List<EventEntity> eventEntityList = new List<EventEntity>();
                    EventEntity tempEventEntity = new EventEntity();
                    tempEventEntity.listen = "test";
                    ScriptEntity scriptEntity = new ScriptEntity();
                    List<string> execList = new List<string>();
                    execList.Add("tests[\"状态200\"] = responseCode.code === 200;");
                    scriptEntity.exec = execList;
                    tempEventEntity.script = scriptEntity;
                    eventEntityList.Add(tempEventEntity);
                    tempHttpEntity.eventEntity = eventEntityList;
                    RequestEntity requestEntity = tempHttpEntity.request;
                    List<HeaderEntity> headerEntityList = requestEntity.header;
                    int headerEntityListLength = headerEntityList.Count;
                    for (int i = headerEntityListLength - 1; i >= 0; i--)
                    {
                        //header只保留authorization和content-type两个内容
                        if (headerEntityList[i].key.Equals("authorization"))
                        {
                            headerEntityList[i].value = "Bearer {{Bearer}}";
                        }
                        else if (headerEntityList[i].key.Equals("content-type"))
                        {
                            continue;
                        }
                        else
                        {
                            headerEntityList.Remove(headerEntityList[i]);
                        }
                    }
                    //环境参数与请求参BodyEntity bodyEntity = requestEntity.body;
                    //数对应暂时没实现string oldBodyStr = bodyEntity.raw;
                    URLEntity urlEntity = requestEntity.url;
                    string oldRaw = urlEntity.raw;
                    index = oldRaw.IndexOf("/api");
                    urlEntity.raw = "{{url}}" + oldRaw.Substring(index);
                    List<string> hostList = new List<string>();
                    hostList.Add("{{url}}");
                    urlEntity.host = hostList;
                }
                myJsonClass.item = httpEntityList;
                //序列化Json
                string jsonStr = JsonConvert.SerializeObject(myJsonClass);
                //写入文件
                byte[] dataArray = Encoding.UTF8.GetBytes(jsonStr);
                FileStream fileStream = new FileStream(outputPath + "\\" + tempFileName + "_AutoTest.json", FileMode.Create);
                fileStream.Write(dataArray, 0, dataArray.Length);
                fileStream.Flush();
                fileStream.Close();
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
            string inputPath = label1.Text;
            string outputPath = label2.Text;
            DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles();
            Regex regex = new Regex("[a-zA-Z][1]");
            try
            {
                for (int i = 0; i < fileInfoArray.Length; i++)
                {
                    string tempPath = fileInfoArray[i].FullName;
                    tempPath += "\r\n";
                    textBox1.Text += tempPath;
                    string tempNameA = fileInfoArray[i].Name;
                    if (regex.IsMatch(tempNameA))
                    {
                        int tempIndexA = tempNameA.IndexOf("1");
                        tempNameA = tempNameA.Remove(tempIndexA, 1);
                        tempNameA = tempNameA.Insert(tempIndexA, "");
                    }
                    string tempPathA = outputPath + "\\" + tempNameA;
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
