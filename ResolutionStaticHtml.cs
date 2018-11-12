using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyEatNotWash
{
    class ResolutionStaticHtml
    {
        public DataTable _dataTable;
        public String _filePath;

        public ResolutionStaticHtml()
        {
            _dataTable = new DataTable("Test");
        }

        public String[] GetFiles(String type)
        {
            if (_filePath != null && !_filePath.Equals("") && type != null && !type.Equals(""))
            {
                return Directory.GetFiles(_filePath, type);
            }
            else
            {
                return null;
            }
        }

        public String[] GetRetFile(String[] files)
        {
            if (files == null)
            {
                return null;
            }
            List<String> listFile = new List<String>();
            int fileSize = files.Length;
            String[] fileNameArray = new String[fileSize];
            for (int i = 0; i < fileSize; i++)
            {
                String baseFile1 = files[i];//文件全名
                String[] tmepStrArray = baseFile1.Split(new String[] { "智联求职者 " }, StringSplitOptions.None);
                if (tmepStrArray.Length < 2)
                {
                    continue;
                }
                String baseFile2 = tmepStrArray[1];//截取后的文件名
                for (int j = i + 1; j < fileSize - j; j++)
                {
                    String andFile1 = files[j];
                    tmepStrArray = andFile1.Split(new String[] { "智联求职者 " }, StringSplitOptions.None);
                    if (tmepStrArray.Length < 2)
                    {
                        continue;
                    }
                    String andFile2 = tmepStrArray[1];
                    if (baseFile2.Equals(andFile2))
                    {
                        listFile.Add(andFile1);
                    }
                }
            }
            int listLength = listFile.Count;
            if (listLength <= 0)
            {
                return null;
            }
            String[] fileArray = new String[listLength];
            for (int i = 0; i < listLength; i++)
            {
                fileArray[i] = listFile[i];
            }
            return fileArray;
        }

        public int RemoveRetFile(String[] files)
        {
            if (files == null)
            {
                return -1;
            }
            try
            {
                int fileCount = files.Count();
                for (int i = 0; i < fileCount; i++)
                {
                    if (File.Exists(files[i]))
                    {
                        File.Delete(files[i]);
                    }
                }
                return 1;
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }


    }
}
