using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using MySqlX.XDevAPI;

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
            String inputPath = label1.Text;
            String outputPath = label2.Text;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
                String fileName = directoryInfo.Name;
                //读取文件内容
                StreamReader streanReader = new StreamReader(directoryInfo.ToString(), Encoding.Default);
                String content = streanReader.ReadToEnd();
                //反序列化Json
                MyJsonClass myJsonClass = JsonConvert.DeserializeObject<MyJsonClass>(content);
                List<HttpEntity> httpEntityList = myJsonClass.item;
                int index = 0;
                UTF8Encoding utf8 = new UTF8Encoding();
                foreach (HttpEntity tempHttpEntity in httpEntityList)
                {
                    //String oldName = tempHttpEntity.name;
                    //index = oldName.IndexOf("/api");
                    //tempHttpEntity.name = "{{url}}" + oldName.Substring(index);
                    List<EventEntity> eventEntityList = tempHttpEntity.eventEntity == null ? new List<EventEntity>() : tempHttpEntity.eventEntity;
                    String tempFileName = Path.GetFileNameWithoutExtension(inputPath);
                    index = tempFileName.IndexOf(".postman_collection");
                    tempFileName = tempFileName.Substring(0, index);
                    foreach (EventEntity tempEventEntity in eventEntityList)
                    {
                        tempEventEntity.listen = tempFileName;
                        ScriptEntity scriptEntity = tempEventEntity.script == null ? new ScriptEntity() : tempEventEntity.script;
                        List<String> execList = new List<String>();
                        execList.Add("tests[\"State200\"] = responseCode.code === 200;");
                        scriptEntity.exec = execList;
                        tempEventEntity.script = scriptEntity;
                    }
                    tempHttpEntity.eventEntity = eventEntityList;
                    RequestEntity requestEntity = tempHttpEntity.request;
                    List<HeaderEntity> headerEntityList = requestEntity.header;
                    int headerEntityListLength = headerEntityList.Count;
                    for (int i = headerEntityListLength - 1; i >= 0; i--)
                    {
                        //header只保留authorization和content-type两个内容
                        if (headerEntityList[i].key.Equals("authorization"))
                        {
                            headerEntityList[i].value = "Bearer {{bearer}}";
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
                    //数对应暂时没实现String oldBodyStr = bodyEntity.raw;
                    URLEntity urlEntity = requestEntity.url;
                    String oldRaw = urlEntity.raw;
                    index = oldRaw.IndexOf("/api");
                    urlEntity.raw = "{{url}}" + oldRaw.Substring(index);
                    List<String> hostList = new List<String>();
                    hostList.Add("{{url}}");
                    urlEntity.host = hostList;
                }
                myJsonClass.item = httpEntityList;
                //序列化Json
                String jsonStr = JsonConvert.SerializeObject(myJsonClass);
                //写入文件
                byte[] dataArray = Encoding.Default.GetBytes(jsonStr);
                FileStream fileStream = new FileStream(outputPath + "/XXX.json", FileMode.Create);
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
