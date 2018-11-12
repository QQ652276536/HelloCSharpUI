using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Windows;

namespace OnlyEatNotWash
{
    /// <summary>
    /// 功能：客户端Socket传送文件工具类
    /// </summary>
    class FileUtils
    {
        /// <summary>
        ///IP地址
        /// </summary>
        public static String IP = "";

        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port = 0;

        public static String fileContent = "";

        public static void Send(String str)
        {
            try
            {
                TcpClient tcpClient = new TcpClient(IP, Port);
                NetworkStream netWorkStream = tcpClient.GetStream();
                byte[] contents = Encoding.UTF8.GetBytes(str);
                netWorkStream.Write(contents, 0, contents.Length);
                tcpClient.Close();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="path">文件全路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns>1成功，-1文件不存在，-2连接失败，-3IO异常，-4未知异常</returns>
        public static int StartSend(String path, String fileName)
        {
            if (!File.Exists(path))
            {
                return -1;
            }
            NetworkStream netWorkStream = null;
            BinaryWriter binaryWriter = null;
            FileStream fileStream = null;
            BinaryReader binaryReader = null;
            try
            {
                //FileInfo file = new FileInfo(path);
                //fileStream = file.OpenRead();
                //byte[] fileContents = new byte[file.Length];
                //fileStream.Read(fileContents, 0, Convert.ToInt32(file.Length));
                //fileContent = Convert.ToBase64String(fileContents);

                TcpClient client = new TcpClient(IP, Port);
                netWorkStream = client.GetStream();
                binaryWriter = new BinaryWriter(netWorkStream);
                //TODO:随便写条信息过去
                binaryWriter.Write("中华人民共和国");
                //获取文件名的字节
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                //拷贝文件名字节
                byte[] fileNameBytesArray = new byte[1024];
                Array.Copy(fileNameBytes, fileNameBytesArray, fileNameBytes.Length);
                //写入流
                binaryWriter.Write(fileNameBytesArray, 0, fileNameBytesArray.Length);
                //获取文件内容
                fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //写入流
                binaryReader = new BinaryReader(fileStream);
                //文件内容的字节
                byte[] fileContentArray = new byte[1024];
                int count = 0;
                while ((count = fileStream.Read(fileContentArray, 0, 1024)) > 0)
                {
                    binaryWriter.Write(fileContentArray, 0, count);
                    fileContentArray = new byte[1024];
                }
                binaryWriter.Flush();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.StackTrace);
                return -2;
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.StackTrace);
                return -3;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return -4;
            }
            finally
            {
                if (binaryWriter != null)
                    binaryWriter.Close();
                if (binaryReader != null)
                    binaryReader.Close();
                if (fileStream != null)
                    fileStream.Close();
                if (netWorkStream != null)
                    netWorkStream.Close();
            }
            return 1;
        }

        /// <summary>
        /// 测试通讯
        /// </summary>
        /// <param name="name">测试内容需和服务端校验内容保持一致</param>
        /// <returns></returns>
        public static bool TestConnection(String str)
        {
            NetworkStream netWorkStream = null;
            BinaryWriter binaryWriter = null;
            try
            {
                TcpClient tcpClient = new TcpClient(IP, Port);
                netWorkStream = tcpClient.GetStream();
                binaryWriter = new BinaryWriter(netWorkStream);
                byte[] byteArray = Encoding.UTF8.GetBytes(str);
                binaryWriter.Write(byteArray, 0, byteArray.Length);
                binaryWriter.Flush();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}