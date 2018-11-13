using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Data;
using GaiaGenerateSQL;

namespace OnlyEatNotWash
{
    /// <summary>
    /// 上级指示窗体
    /// </summary>
    public class EmailWindow
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public EmailWindow()
        {
            ResolutionStaticHtml resolution = new ResolutionStaticHtml();
            resolution._filePath = @"C:\Users\GAIA\Desktop\email";
            String[] files1 = resolution.GetFiles("*.html");
            String[] files2 = resolution.GetRetFile(files1);
            int result = resolution.RemoveRetFile(files2);
            String[] files3 = resolution.GetFiles("*.html");
            BindGrid(files3, true);
        }

        /// <summary>
        /// 表格
        /// </summary>
        private DataTable m_gridEmail = new DataTable("datas");

        private EmailInfo m_emailInfo;

        /// <summary>
        /// 获取或设置邮箱信息
        /// </summary>
        public EmailInfo EmailInfo
        {
            get { return m_emailInfo; }
            set { m_emailInfo = value; }
        }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public void BindGrid(String[] files, bool isNew)
        {
            int filesSize = files.Length;
            m_gridEmail.Columns.Add("产品名称", Type.GetType("System.String"));
            m_gridEmail.Columns.Add("公司", Type.GetType("System.String"));
            m_gridEmail.Columns.Add("产品简介", Type.GetType("System.String"));
            for (int i = 0; i < filesSize; i++)
            {
                String filePath = files[i];
                String content = "";
                content = File.ReadAllText(filePath, Encoding.Default);
                if (content != null && content.Length > 100)
                {
                    String productNameStr = "<td width=\"424\" style=\"padding-left:14px; font-weight:bold\"><span style=\"display:inline-block;margin:0 2px\">";
                    int productCount = content.Split(new String[] { productNameStr }, StringSplitOptions.None).Length - 1;
                    int pdx = 0;
                    int npdx = 0;
                    int indx = 0;
                    int indx2 = 0;
                    for (int tempI = 0; tempI < productCount; tempI++)
                    {
                        DataRow row = m_gridEmail.NewRow();
                        if (tempI == 0)
                        {
                            pdx = content.IndexOf(productNameStr, 0);
                            npdx = content.IndexOf("</span>", pdx + 1);
                        }
                        else
                        {
                            pdx = content.IndexOf(productNameStr, pdx + tempI);
                            npdx = content.IndexOf("</span>", pdx + 1);
                        }
                        String productName = content.Substring(pdx + productNameStr.Length, npdx - pdx - productNameStr.Length);
                        row["产品名称"] = productName;
                        String companyNameStr = "<td width=\"424\" style=\"padding-left:14px; font-weight:bold;word-break: break-all; word-wrap:break-word;\"><span style=\"display:inline-block;margin:0 2px\">";
                        int idx = content.IndexOf(companyNameStr);
                        int nidx = content.IndexOf("</span>", idx + 1);
                        if (idx > -1 && nidx > -1)
                        {
                            String companyName = content.Substring(idx + companyNameStr.Length, nidx - idx - companyNameStr.Length);
                            if (companyName.Length < 4)
                            {
                                continue;
                            }
                            row["公司"] = companyName;
                        }
                        String productInfoStr = "<span>项目简介：</span>";
                        int infoCount = content.Split(new String[] { productInfoStr }, StringSplitOptions.None).Length - 1;
                        if (tempI == 0)
                        {
                            indx = content.IndexOf(productInfoStr, 0);
                            indx2 = content.IndexOf("</p>", indx + 1);
                        }
                        else
                        {
                            indx = content.IndexOf(productInfoStr, indx + tempI);
                            indx2 = content.IndexOf("</p>", indx + 1);
                        }
                        row["产品简介"] = content.Substring(indx + productInfoStr.Length, indx2 - indx - productInfoStr.Length);
                        m_gridEmail.Rows.Add(row);
                    }
                }
            }
            ExportService m_exportService = new ExportService();
            m_exportService.ExportDataTableToExcel(m_gridEmail, "C://Users//GAIA//Desktop//1");
        }

    }

    public class EmailInfo
    {
        /// <summary>
        /// 端口
        /// </summary>
        public String m_port;

        /// <summary>
        /// 密码
        /// </summary>
        public String m_pwd;

        /// <summary>
        /// 服务器
        /// </summary>
        public String m_server;

        /// <summary>
        /// 用户名
        /// </summary>
        public String m_userName;
    }

    /// <summary>
    /// 邮件的条件
    /// </summary>
    public class EmailCondition
    {
        /// <summary>
        /// 过滤年龄
        /// </summary>
        public int m_filterAge = 40;

        /// <summary>
        /// 过滤日期
        /// </summary>
        public int m_filterDate = 10;

        /// <summary>
        /// 过滤户口
        /// </summary>
        public String m_filterHuKou = "";

        /// <summary>
        /// 过滤关键字
        /// </summary>
        public String m_filterKey = "";

        /// <summary>
        /// 几次工作
        /// </summary>
        public String m_filterJingYan = "";

        /// <summary>
        /// 过滤婚姻
        /// </summary>
        public String m_filterMarry = "全部";

        /// <summary>
        /// 过滤性别
        /// </summary>
        public String m_filterSex = "全部";

        /// <summary>
        /// 过滤状态
        /// </summary>
        public String m_filterStatus = "全部";

        /// <summary>
        /// 过滤学历
        /// </summary>
        public String m_filterXueli = "全部";

        /// <summary>
        /// 标识ID
        /// </summary>
        public String m_id = "";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "请取名";

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public EmailCondition Copy()
        {
            EmailCondition copyCondition = new EmailCondition();
            copyCondition.m_filterAge = m_filterAge;
            copyCondition.m_filterDate = m_filterDate;
            copyCondition.m_filterHuKou = m_filterHuKou;
            copyCondition.m_filterJingYan = m_filterJingYan;
            copyCondition.m_filterMarry = m_filterMarry;
            copyCondition.m_filterSex = m_filterSex;
            copyCondition.m_filterStatus = m_filterStatus;
            copyCondition.m_filterXueli = m_filterXueli;
            copyCondition.m_filterKey = m_filterKey;
            return copyCondition;
        }

        /// <summary>
        /// 转换为String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("最近" + m_filterDate + "天 ");
            sb.Append(m_filterAge + "岁以下 ");
            sb.Append(m_filterHuKou.Length > 0 ? m_filterHuKou + " " : "");
            sb.Append(m_filterMarry != "全部" ? m_filterMarry + " " : "");
            sb.Append(m_filterSex != "全部" ? m_filterSex + " " : "");
            sb.Append(m_filterStatus != "全部" ? m_filterStatus + " " : "");
            sb.Append(m_filterXueli != "全部" ? m_filterXueli + " " : "");
            sb.Append(m_filterJingYan.Length > 0 ? "上过" + m_filterJingYan + "次班 " : "");
            sb.Append(m_filterKey.Length > 0 ? m_filterKey : "");
            return sb.ToString();
        }
    }
}
