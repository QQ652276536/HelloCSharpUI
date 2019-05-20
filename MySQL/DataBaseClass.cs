using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;
using System.IO;

namespace CodingMouse
{
    public class DataBaseClass
    {
        private static ClassIniFile INI = new ClassIniFile(System.Windows.Forms.Application.StartupPath + "\\setup.ini");
        private static string server = INI.GetString("SETTING", "SERVER", "192.168.0.124");
        private static string uid = INI.GetString("SETTING", "UID", "sa");
        private static string pwd = INI.GetString("SETTING", "PWD", "asdf1234");
        private static string database = INI.GetString("SETTING", "DATABASE", "wande");


        public static string str = "server=" + server + ";uid=" + uid + ";pwd=" + pwd + ";database=" + database;


        public DataSet GetDataSetDatas(string sql)
        {

            DataSet ds = new DataSet();
            try
            {

                SqlConnection conn = new SqlConnection(str);
                conn.Open();


                SqlDataAdapter odbcda = new SqlDataAdapter(sql, conn);
                odbcda.Fill(ds);
                conn.Close();
                odbcda.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;

        }
        public DataSet GetDataSetDatas(string commandString, string tableName)
        {

            DataSet dataSet = new DataSet();
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter(commandString, conn);

                SqlCommandBuilder ss = new SqlCommandBuilder(sqlDA);

                if (tableName != "")
                {
                    sqlDA.Fill(dataSet, tableName);

                }
                else
                {
                    sqlDA.Fill(dataSet);
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dataSet;
            //MessageBox.Show("8");
        }
        /* 执行查询数据库操作是否有记录　*/
        /// <summary>
        /// 功能：执行查询数据库操作是否有记录
        /// </summary>
        /// <param name=”strSql”></param>
        public static bool ExecuteRead(string strSql)
        {
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cm = new SqlCommand();
            cm.CommandText = strSql;
            cm.Connection = conn;
            try
            {
                SqlDataReader dr = cm.ExecuteReader();
                if (dr.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cm.Dispose();
                conn.Close();
            }

        }
        /* 执行更新删除插入数据库操作,成功则返回true　*/
        /// <summary>   
        /// 功能：执行更新删除插入数据库操作,成功则返回true   
        /// </summary>   
        /// <param name="strSql"></param>   
        /// <returns></returns>   
        public static bool ExecuteSql(string strSql)
        {
            bool flag = false;
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            //操作   
            SqlCommand cm = new SqlCommand();
            cm.CommandText = strSql;
            try
            {
                cm.Connection = conn;
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                conn.Close();
                flag = true;
            }
            return flag;

        }
        // 返回Sql语句执行结果的第一行第一列
        public static string readData(string commandString)
        {
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            string result;
            SqlDataReader Dr;
            try
            {
                SqlCommand cmd = new SqlCommand(commandString, conn);
                Dr = cmd.ExecuteReader();
                if (Dr.Read())
                {
                    result = Dr[0].ToString();
                    Dr.Close();
                }
                else
                {
                    result = "";
                    Dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
            conn.Close();
            return result;
        }
        public int RunCommand(string commandString)
        {
            int runCount = 0;
            try
            {



                // 执行 SQL 命令
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(commandString, conn);

                    runCount = cmd.ExecuteNonQuery();

                    conn.Close();
                    conn.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "操作数据失败 RunCommand",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            return runCount;

        }
        public void ExportDateToExcel(DataGridView dataGridView1, SaveFileDialog saveFileDialog1, string fileName)
        {
            //MessageBox.Show("3");
            // DGV中行号和列号
            int RCount = dataGridView1.Rows.Count;
            int CCount = dataGridView1.Columns.Count;

            // 从工具箱中添加一个“保存”对话框

            saveFileDialog1.DefaultExt = "xls";
            saveFileDialog1.Filter = "EXCEL文件(*.xls)|*.xls ";

            // EXcel文件名称
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();

            // 取消
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string fileNameString = saveFileDialog1.FileName;
            if (fileNameString.Trim() == "")
            {
                return;
            }
            //MessageBox.Show("4");
            FileInfo file = new FileInfo(fileNameString);
            if (file.Exists)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 注意引用的添加
            Microsoft.Office.Interop.Excel.Application objExcel = null;
            Microsoft.Office.Interop.Excel.Workbook objWorkbook = null;
            Microsoft.Office.Interop.Excel.Worksheet objsheet = null;
            //MessageBox.Show("5");
            try
            {
                objExcel = new Microsoft.Office.Interop.Excel.Application();
                objWorkbook = objExcel.Workbooks.Add(Type.Missing);
                objsheet = (Microsoft.Office.Interop.Excel.Worksheet)objWorkbook.ActiveSheet;

                objExcel.Visible = false;
                Microsoft.Office.Interop.Excel.Range range = null;
                //向Excel中写入表格的表头 
                int displayColumnsCount = 1;
                for (int i = 0; i <= dataGridView1.ColumnCount - 1; i++)
                {
                    if (dataGridView1.Columns[i].Visible == true)
                    {




                        objExcel.Cells[1, displayColumnsCount] = "'" + dataGridView1.Columns[i].HeaderText.Trim();


                        range = (Microsoft.Office.Interop.Excel.Range)objExcel.Cells[1, displayColumnsCount];
                        //range.Interior.ColorIndex = 15;//背景颜色 
                        range.Font.Bold = true;//粗体 
                        //range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;//居中 
                        //加边框 
                        //range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                        //range.ColumnWidth = 4.63;//设置列宽 
                        //range.EntireColumn.AutoFit();//自动调整列宽 
                        //range.EntireRow.AutoFit();//自动调整行高 

                        //range.EntireColumn.AutoFit();//自动调整宽

                        // ((OWC.Range)objExcel.Cells[1, displayColumnsCount]).EntireColumn.AutoFit(); 

                        displayColumnsCount++;
                    }
                }
                //向Excel中写入数据
                for (int row = 0; row <= dataGridView1.RowCount - 1; row++)
                {
                    displayColumnsCount = 1;
                    for (int col = 0; col < CCount; col++)
                    {
                        if (dataGridView1.Columns[col].Visible == true)
                        {
                            try
                            {
                                objExcel.Cells[row + 2, displayColumnsCount] = "'" + dataGridView1.Rows[row].Cells[col].Value.ToString().Trim();

                                range = (Microsoft.Office.Interop.Excel.Range)objExcel.Cells[row + 2, displayColumnsCount];
                                range.Font.Size = 9;//字体大小 
                                //加边框 
                                //range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                                //range.EntireColumn.AutoFit();//自动调整列宽 

                                displayColumnsCount++;
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }
                //合并格子加粗
                //range = objsheet.get_Range("B2", "D4");
                //range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);


                objWorkbook.SaveAs(fileNameString, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                //关闭Excel应用 
                if (objWorkbook != null) objWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);
                if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
                if (objExcel != null) objExcel.Quit();

                objsheet = null;
                objWorkbook = null;
                objExcel = null;
            }
            MessageBox.Show(fileNameString + "\n\n导出完毕！ ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        /// <summary>
        /// 清除DetaGridView
        /// </summary>
        /// <param name="dgv"></param>
        public static void CreateExcel(System.Data.DataTable dt, string fileName)
        {
            System.Diagnostics.Process[] arrProcesses;
            arrProcesses = System.Diagnostics.Process.GetProcessesByName("Excel");
            foreach (System.Diagnostics.Process myProcess in arrProcesses)
            {
                myProcess.Kill();
            }

            Object missing = Missing.Value;
            Microsoft.Office.Interop.Excel.Application m_objExcel =

             new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks m_objWorkBooks = m_objExcel.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook m_objWorkBook = m_objWorkBooks.Add(true);
            Microsoft.Office.Interop.Excel.Sheets m_objWorkSheets = m_objWorkBook.Sheets; ;
            Microsoft.Office.Interop.Excel.Worksheet m_objWorkSheet =

             (Microsoft.Office.Interop.Excel.Worksheet)m_objWorkSheets[1];
            int intFeildCount = dt.Columns.Count;
            for (int col = 0; col < intFeildCount; col++)
            {
                m_objWorkSheet.Cells[1, col + 1] = dt.Columns[col].ToString();
            }
            for (int intRowCount = 0; intRowCount < dt.Rows.Count; intRowCount++)
            {
                for (int intCol = 0; intCol < dt.Columns.Count; intCol++)
                {
                    m_objWorkSheet.Cells[intRowCount + 2, intCol + 1] = "'" + dt.Rows[intRowCount][intCol].ToString();
                }
            }


            m_objWorkBook.SaveAs(fileName, missing, missing, missing, missing,

             missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
             missing, missing, missing, missing, missing);

            m_objExcel = null;

        }
        public static void CreateExcel(DataGridView dgv, SaveFileDialog saveFileDialog1, string fileName)
        {
            saveFileDialog1.DefaultExt = ".xls";
            saveFileDialog1.Filter = "EXCEL文件(*.xls)|*.xls ";

            // EXcel文件名称
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();

            // 取消
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string fileNameString = saveFileDialog1.FileName;
            if (fileNameString.Trim() == "")
            {
                return;
            }
            //MessageBox.Show("4");
            FileInfo file = new FileInfo(fileNameString);
            if (file.Exists)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            System.Diagnostics.Process[] arrProcesses;
            arrProcesses = System.Diagnostics.Process.GetProcessesByName("Excel");
            foreach (System.Diagnostics.Process myProcess in arrProcesses)
            {
                myProcess.Kill();
            }

            Object missing = Missing.Value;
            Microsoft.Office.Interop.Excel.Application m_objExcel =

             new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks m_objWorkBooks = m_objExcel.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook m_objWorkBook = m_objWorkBooks.Add(true);
            Microsoft.Office.Interop.Excel.Sheets m_objWorkSheets = m_objWorkBook.Sheets; ;
            Microsoft.Office.Interop.Excel.Worksheet m_objWorkSheet =

             (Microsoft.Office.Interop.Excel.Worksheet)m_objWorkSheets[1];
            int intFeildCount = dgv.Columns.Count;
            for (int col = 0; col < intFeildCount; col++)
            {
                m_objWorkSheet.Cells[1, col + 1] = dgv.Columns[col].Name.ToString();
            }
            for (int intRowCount = 0; intRowCount < dgv.Rows.Count - 1; intRowCount++)
            {
                for (int intCol = 0; intCol < dgv.Columns.Count; intCol++)
                {
                    if (dgv.Rows[intRowCount].Cells[intCol].Value == null)
                    {
                        dgv.Rows[intRowCount].Cells[intCol].Value = "";
                    }
                    m_objWorkSheet.Cells[intRowCount + 2, intCol + 1] = "'" + dgv.Rows[intRowCount].Cells[intCol].Value.ToString();
                }
            }


            m_objWorkBook.SaveAs(fileNameString, missing, missing, missing, missing,

             missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
             missing, missing, missing, missing, missing);

            if (m_objWorkBook != null) m_objWorkBook.Close(Type.Missing, Type.Missing, Type.Missing);
            if (m_objExcel.Workbooks != null) m_objExcel.Workbooks.Close();
            if (m_objExcel != null) m_objExcel.Quit();

            //missing  = null;
            m_objWorkBook = null;
            m_objExcel = null;

            MessageBox.Show("导出成功！");
            //m_objExcel = null;

        }
        public void AutoSizeColumn(DataGridView dgViewFiles)
        {
            int width = 0;
            //使列自使用宽度
            //对于DataGridView的每一个列都调整
            for (int i = 0; i < dgViewFiles.Columns.Count; i++)
            {
                //将每一列都调整为自动适应模式
                dgViewFiles.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgViewFiles.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，
            //如果是小于原来设定的宽度，将模式改为填充。
            if (width > dgViewFiles.Size.Width)
            {
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                //dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            //冻结某列 从左开始 0，1，2
            dgViewFiles.Columns[1].Frozen = true;
        }
    }
}
