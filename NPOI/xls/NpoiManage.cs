using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;


namespace GaiaGenerateSQL
{
    /// <summary>
    /// 数据应用类型
    /// </summary>
    public enum ExportSourceType
    {
        /// <summary>
        /// 其他
        /// </summary>
        General,

        /// <summary>
        /// edb数据
        /// </summary>
        Edb
    }

    public enum columnName
    {
        Nono,
        ColumnName,
        Caption
    }
    public enum excelType
    {
        xls,
        xlsx
    }
    public class DataTableData
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="imageLocation">图片坐标</param>
        public DataTableData(DataTable dt, Point dtLocation)
            : this(dt, dtLocation, ExportSourceType.General)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">表格数据/param>
        /// <param name="dtLocation">表格位置</param>
        /// <param name="exportType">导出类型</param>
        public DataTableData(DataTable dt, Point dtLocation, ExportSourceType exportType)
        {
            this.dt = dt;
            this.dtLocation = dtLocation;
            _exportDataType = exportType;
        }

        private DataTable dt;

        public DataTable Dt
        {
            get { return dt; }
            set { dt = value; }
        }
        private Point dtLocation;

        public Point DtLocation
        {
            get { return dtLocation; }
            set { dtLocation = value; }
        }

        private ExportSourceType _exportDataType;

        public ExportSourceType ExportType
        {
            get { return _exportDataType; }
            set { _exportDataType = value; }
        }
    }
    public class ImageData
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="imageIndex">图片所在的sheet序号</param>
        /// <param name="imageLocation">图片坐标</param>
        public ImageData(Image image, int imageIndex, Point imageLocation)
        {
            this.image = image;
            this.imageIndex = imageIndex;
            this.imageLocation = imageLocation;
        }
        private Image image;

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        private int imageIndex;

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }
        private Point imageLocation;

        public Point ImageLocation
        {
            get { return imageLocation; }
            set { imageLocation = value; }
        }
    }
    public class NpoiManage
    {
        #region 汤文 2013\5\6

        #region 导出模式
        private static excelType type = excelType.xls;
        public static excelType Type
        {
            get { return type; }
            set { type = value; }
        }

        #endregion

        #region 公用数据结构
        private class Determinant
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="tdPos">TD索引</param>
            /// <param name="trPos">TR索引</param>
            /// <param name="row">单元格行号</param>
            /// <param name="col">单元格列号</param>
            public Determinant(int tdPos, int trPos, int row, int col)
            {
                this.tdPos = tdPos;
                this.trPos = trPos;
                this.row = row;
                this.col = col;
            }

            private int tdPos;

            /// <summary>
            /// 获取或设置TD索引
            /// </summary>
            public int TdPos
            {
                get { return tdPos; }
                set { tdPos = value; }
            }

            private int trPos;

            /// <summary>
            /// 获取或设置TR索引
            /// </summary>
            public int TrPos
            {
                get { return trPos; }
                set { trPos = value; }
            }

            private int row;

            /// <summary>
            /// 获取或设置单元格行号
            /// </summary>
            public int Row
            {
                get { return row; }
                set { row = value; }
            }

            private int col;

            /// <summary>
            /// 获取或设置单元格列号
            /// </summary>
            public int Col
            {
                get { return col; }
                set { col = value; }
            }
            /// <summary>
            /// 转换为字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "tr:" + trPos.ToString() + "td:" + tdPos.ToString();
            }
        }
        private class CellsMerge
        {
            public CellsMerge()
            {
            }
            public CellsMerge(int rowX, int rowY, int colX, int colY, string text)
            {
                row = new Point(rowX, rowY);
                col = new Point(colX, colY);
                this.text = text;
                this.dataType = "s";
                backColor = -1;
            }

            private Point row;

            public Point Row
            {
                get { return row; }
                set { row = value; }
            }
            private Point col;

            public Point Col
            {
                get { return col; }
                set { col = value; }
            }
            private string text;

            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            private string dataType;

            public string DataType
            {
                get { return dataType; }
                set { dataType = value; }
            }

            private short backColor;

            public short BackColor
            {
                get { return backColor; }
                set { backColor = value; }
            }
        }
        #endregion

        #region 公用方法
        private static void SetCellValue(IRow row, List<int> widths, int col, ICellStyle style, string value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            ICell ic = row.CreateCell(col);
            if (style != null)
            {
                ic.CellStyle = style;
            }
            ic.SetCellValue(value);
            SetCellWith(row.Sheet, widths, col, value);
            SetCellHeight(row, value);
        }
        private static void SetCellValue(IRow row, List<int> widths, int col, ICellStyle style, bool value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            ICell ic = row.CreateCell(col);
            if (style != null)
            {
                ic.CellStyle = style;
            }
            ic.SetCellValue(value);
            SetCellWith(row.Sheet, widths, col, value.ToString());
            SetCellHeight(row, value.ToString());
        }
        private static void SetCellValue(IRow row, List<int> widths, int col, ICellStyle style, DateTime value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            ICell ic = row.CreateCell(col);
            if (style != null)
            {
                ic.CellStyle = style;
            }
            ic.SetCellValue(value);
            SetCellWith(row.Sheet, widths, col, value.ToString());
            SetCellHeight(row, value.ToString());
        }
        private static void SetCellValue(IRow row, List<int> widths, int col, ICellStyle style, double value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            ICell ic = row.CreateCell(col);
            if (style != null)
            {
                ic.CellStyle = style;
            }
            ic.SetCellValue(value);
            SetCellWith(row.Sheet, widths, col, value.ToString());
            SetCellHeight(row, value.ToString());
        }
        private static void SetCellValue(IRow row, List<int> widths, int col, ICellStyle style, IRichTextString value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            ICell ic = row.CreateCell(col);
            if (style != null)
            {
                ic.CellStyle = style;
            }
            ic.SetCellValue(value);
            SetCellWith(row.Sheet, widths, col, value.ToString());
            SetCellHeight(row, value.ToString());
        }

        private static void SetCellWith(ISheet sheet, List<int> widths, int col, string value)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            //if (widths != null && widths[col] != 0)
            //{
            //    sheet.SetColumnWidth(col, widths[col] * 50);
            //}
            if (value == null)
            {
                return;
            }
            int i = 6;
            if (value.Contains("\r\n"))
            {
                string[] array = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in array)
                {
                    if (i < str.Trim().Length)
                    {
                        i = str.Trim().Length;
                    }
                }
            }
            else
            {

                if (i < value.Length)
                {
                    i = value.Length;
                }
            }
            if (i > 30)
            {
                i = 30;
            }
            //sheet.SetColumnWidth(col, i * 4 * 200);
            sheet.SetColumnWidth(col, i * 4 * 150);
        }
        private static void SetCellHeight(IRow row, string value)
        {
            if (value == null)
            {
                return;
            }
            //设置列高
            if (value.Contains("\r\n"))
            {
                string[] array = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                row.HeightInPoints = array.Length * row.Sheet.DefaultRowHeight / 12;
            }
        }

        private static IFont CellFont(IWorkbook workbook, short wordSize, short boldweight, short fontColor)
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = wordSize;//字号
            font.Boldweight = boldweight;//加粗
            font.Color = fontColor;//颜色
            return font;
        }
        /// <summary>
        /// 保留2位小数以及千分符,百分符
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        private static ICellStyle CellStyle(IWorkbook workbook, int type, int decimalDigits)
        {
            StringBuilder digits = new StringBuilder();
            for (int i = 0; i < decimalDigits; i++)
            {
                digits.Append("0");
            }
            string digit = string.Format(".{0}", digits.ToString());
            ICellStyle style = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            switch (type)
            {
                //保留小数位数
                case 1: style.DataFormat = format.GetFormat(string.Format("0{0}", digit)); break;
                //千分符号
                case 2: style.DataFormat = format.GetFormat("#,##0"); break;
                //千分符号+保留小数位数
                case 3: style.DataFormat = format.GetFormat(string.Format("#,##0{0}", digit)); break;
                //百分符
                case 4: style.DataFormat = format.GetFormat("0%"); break;
                //百分符+保留小数位数
                case 5: style.DataFormat = format.GetFormat(string.Format("0{0}%", digit)); break;
                default: break;
            }
            return style;
        }
        private static ICellStyle CellStyle(IWorkbook workbook, IFont font, NPOI.SS.UserModel.HorizontalAlignment alignment)
        {
            //样式
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = alignment;
            style.WrapText = true;
            style.SetFont(font);
            return style;
        }
        private static ICellStyle CellStyle(IWorkbook workbook, IFont font, NPOI.SS.UserModel.HorizontalAlignment alignment, VerticalAlignment valignment)
        {
            //样式
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = alignment;
            style.VerticalAlignment = valignment;
            style.WrapText = true;
            style.SetFont(font);
            return style;
        }
        private static ICellStyle CellStyle(IWorkbook workbook, NPOI.SS.UserModel.HorizontalAlignment alignment, IDataFormat id, int cout)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0.");
            for (int i = 0; i < cout; i++)
            {
                sb.Append("0");
            }
            //样式
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = alignment;
            style.DataFormat = id.GetFormat(sb.ToString());
            style.WrapText = true;
            return style;
        }

        private static int CellImag(int rowIndex, Image img)
        {
            rowIndex += img.Height / 15 + 2;
            return rowIndex;
        }

        private static void CellImag(IWorkbook workbook, ISheet sheet, Image img, int x, int y)
        {
            try
            {
                string imagPath = Path.Combine(Application.StartupPath, "temp");
                string strTempFilePath = System.IO.Path.Combine(imagPath, @"temppic.jpeg");

                img.Save(strTempFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image img2 = Image.FromFile(strTempFilePath);

                byte[] bytes = convertByte(img2);
                int pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                IDrawing patriarch = sheet.CreateDrawingPatriarch();
                IClientAnchor anchor;
                if (type == excelType.xls)
                {
                    anchor = new HSSFClientAnchor(0, 0, 0, 0, y, x, y, x);
                }
                else
                {
                    anchor = new XSSFClientAnchor(0, 0, 0, 0, y, x, y, x);
                }
                IPicture pict = patriarch.CreatePicture(anchor, pictureIdx);
                pict.Resize();
            }
            catch { }
        }

        private static int CellMerge(IWorkbook workbook, ISheet sheet, ICellStyle style, List<CellsMerge> headMerge)
        {
            if (headMerge.Count == 0)
            {
                return 0;
            }
            int rowIndex = 0;
            int maxRowIndex = 0;
            int listCout = headMerge[headMerge.Count - 1].Row.X;
            IRow[] rows = new IRow[listCout + 1];
            //合并后的单元格
            if (headMerge != null && headMerge.Count > 0)
            {
                maxRowIndex = headMerge[0].Row.X;
                rows[maxRowIndex] = sheet.GetRow(maxRowIndex);
                if (rows[maxRowIndex] == null)
                {
                    rows[maxRowIndex] = sheet.CreateRow(maxRowIndex);
                }

                for (int i = 0; i < headMerge.Count; i++)
                {
                    try
                    {
                        if (maxRowIndex < (headMerge[i].Row.X))
                        {
                            maxRowIndex = headMerge[i].Row.X;
                            if (sheet.GetRow(maxRowIndex) != null)
                            {
                                rows[maxRowIndex] = sheet.GetRow(maxRowIndex);
                            }
                            else
                            {
                                rows[maxRowIndex] = sheet.CreateRow(maxRowIndex);
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                //赋值
                for (int j = 0; j < headMerge.Count; j++)
                {
                    if (headMerge[j].DataType.ToLower() == "s")
                    {
                        SetCellValue(rows[headMerge[j].Row.X], null, headMerge[j].Col.X, style, headMerge[j].Text);
                    }
                    else
                    {
                        double d;
                        double.TryParse(headMerge[j].Text, out d);
                        SetCellValue(rows[headMerge[j].Row.X], null, headMerge[j].Col.X, style, d);
                    }
                }
                for (int j = 0; j < headMerge.Count; j++)
                {
                    //单元格合并
                    try
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(headMerge[j].Row.X, headMerge[j].Row.Y, headMerge[j].Col.X, headMerge[j].Col.Y));

                    }
                    catch (System.Exception ex)
                    {

                    }
                }

                rowIndex = maxRowIndex + 1;
            }
            return rowIndex;
        }
        private static int CellMerge(IWorkbook workbook, ISheet sheet, List<CellsMerge> bodyMerge)
        {
            return CellMerge(workbook, sheet, bodyMerge, null);
        }
        private static int CellMerge(IWorkbook workbook, ISheet sheet, List<CellsMerge> bodyMerge, ICellStyle style)
        {
            try
            {
                if (bodyMerge.Count == 0)
                {
                    return 0;
                }
                Dictionary<string, ICellStyle> CellStyleDY = new Dictionary<string, ICellStyle>();
                int rowIndex = 0;
                int maxRowIndex = 0;
                //List<IRow> rows=new List<IRow>();
                int listCout = bodyMerge[bodyMerge.Count - 1].Row.X;
                IRow[] rows = new IRow[listCout + 1];
                //合并后的单元格
                if (bodyMerge != null && bodyMerge.Count > 0)
                {
                    maxRowIndex = bodyMerge[0].Row.X;
                    rows[maxRowIndex] = sheet.GetRow(maxRowIndex);
                    if (rows[maxRowIndex] == null)
                    {
                        rows[maxRowIndex] = sheet.CreateRow(maxRowIndex);
                    }

                    for (int i = 0; i < bodyMerge.Count; i++)
                    {
                        if (maxRowIndex < (bodyMerge[i].Row.X))
                        {
                            maxRowIndex = bodyMerge[i].Row.X;
                            if (sheet.GetRow(maxRowIndex) != null)
                            {
                                rows[maxRowIndex] = sheet.GetRow(maxRowIndex);
                            }
                            else
                            {
                                rows[maxRowIndex] = sheet.CreateRow(maxRowIndex);
                            }
                        }
                    }
                    for (int j = 0; j < bodyMerge.Count; j++)
                    {
                        string[] array = bodyMerge[j].DataType.Trim().ToUpper().Split(new string[] { "|" }, StringSplitOptions.None);
                        if (array == null)
                        {
                            array = new string[3];
                        }
                        try
                        {
                            switch (array.Length)
                            {
                                //兼容以前的代码
                                case 1:
                                    {
                                        if (array[0] != null && array[0].Trim() == "N")
                                        {
                                            double d = Convert.ToDouble(bodyMerge[j].Text);
                                            SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, d);
                                        }
                                        else
                                        {
                                            SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, bodyMerge[j].Text);
                                        }
                                    }
                                    break;
                                case 3:
                                    {
                                        StringBuilder type = new StringBuilder();
                                        int decimalDigits = 0;
                                        if (array[0] != null && array[0].Trim() == "N")
                                        {
                                            type.Append("N");
                                        }
                                        type.Append("|");
                                        if (array[1] != null && array[1].Length > 0)
                                        {
                                            int.TryParse(array[1], out decimalDigits);
                                            type.Append("D");
                                        }
                                        type.Append("|");
                                        if (array[2] != null && array[2].Trim() == "T")
                                        {
                                            type.Append("T");
                                        }
                                        else if (array[2] != null && array[2].Trim() == "H")
                                        {
                                            type.Append("H");
                                        }

                                        switch (type.ToString())
                                        {
                                            case "N|D|T":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text);
                                                    string key = string.Format("{0}_{1}", 3, decimalDigits);
                                                    if (!CellStyleDY.ContainsKey(key))
                                                    {
                                                        CellStyleDY.Add(key, CellStyle(workbook, 3, decimalDigits));
                                                    }
                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, CellStyleDY[key], d);
                                                }
                                                break;
                                            case "N||T":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text);
                                                    string key = string.Format("{0}_{1}", 2, decimalDigits);
                                                    if (!CellStyleDY.ContainsKey(key))
                                                    {
                                                        CellStyleDY.Add(key, CellStyle(workbook, 2, decimalDigits));
                                                    }
                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, CellStyleDY[key], d);
                                                }
                                                break;
                                            case "N|D|H":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text.Replace("%", "")) / 100;
                                                    string key = string.Format("{0}_{1}", 5, decimalDigits);
                                                    if (!CellStyleDY.ContainsKey(key))
                                                    {
                                                        CellStyleDY.Add(key, CellStyle(workbook, 5, decimalDigits));
                                                    }
                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, CellStyleDY[key], d);
                                                } break;
                                            case "N||H":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text.Replace("%", "")) / 100;
                                                    string key = string.Format("{0}_{1}", 4, decimalDigits);
                                                    if (!CellStyleDY.ContainsKey(key))
                                                    {
                                                        CellStyleDY.Add(key, CellStyle(workbook, 4, decimalDigits));
                                                    }

                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, CellStyleDY[key], d);

                                                }
                                                break;
                                            case "N|D|":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text);
                                                    string key = string.Format("{0}_{1}", 1, decimalDigits);
                                                    if (!CellStyleDY.ContainsKey(key))
                                                    {
                                                        CellStyleDY.Add(key, CellStyle(workbook, 1, decimalDigits));
                                                    }

                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, CellStyleDY[key], d);

                                                } break;
                                            case "N||":
                                                {
                                                    double d = Convert.ToDouble(bodyMerge[j].Text);

                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, d);

                                                }
                                                break;
                                            default:
                                                {

                                                    SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, bodyMerge[j].Text);

                                                } break;
                                        }
                                    }
                                    break;
                                default:
                                    {

                                        if (bodyMerge[j].Text.Trim() != "-")
                                        {
                                            SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, bodyMerge[j].Text);
                                        }
                                        else
                                        {
                                            SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, "");
                                        }

                                    } break;
                            }
                        }
                        catch
                        {

                            if (bodyMerge[j].Text.Trim() != "-")
                            {
                                SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, bodyMerge[j].Text);
                            }
                            else
                            {
                                SetCellValue(rows[bodyMerge[j].Row.X], null, bodyMerge[j].Col.X, style, "");

                            }

                        }

                        SetCellWith(sheet, null, bodyMerge[j].Col.X, bodyMerge[j].Text);
                    }
                    for (int j = 0; j < bodyMerge.Count; j++)
                    {
                        //单元格合并
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(bodyMerge[j].Row.X, bodyMerge[j].Row.Y, bodyMerge[j].Col.X, bodyMerge[j].Col.Y));
                    }

                    rowIndex = maxRowIndex + 1;
                }
                return rowIndex;
            }
            catch
            {
                return 0;
            }
        }
        private static void CellEnd(ISheet sheet, int rowIndex)
        {
            rowIndex += 5;
            IFont font = CellFont(sheet.Workbook, 11, (short)FontBoldWeight.Bold, HSSFColor.Red.Index);
            font.IsItalic = true;
            ICellStyle style = sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            style.SetFont(font);

            IRow row = sheet.CreateRow(rowIndex);
            ICell ic = row.CreateCell(0);
            ic.CellStyle = style;
            ic.SetCellValue("数据来源：坤仪");
            row.HeightInPoints = row.Sheet.DefaultRowHeight / 12;
        }
        private static void CellFreezePane(ISheet sheet, int row, int col)
        {
            if (type == excelType.xls)
            {
                if (col > 255)
                {
                    return;
                }
            }
            sheet.CreateFreezePane(col, row, col, row);
        }
        private static void CheckExt(string fileName)
        {
            if (Path.GetExtension(fileName).Contains("xlsx"))
            {
                type = excelType.xlsx;
            }
            else
            {
                type = excelType.xls;
            }
        }
        #endregion

        #region 拼接
        private static IWorkbook ToExcel(List<DataTableData> tables, List<List<string[]>> headers, columnName cn, List<List<int>> wds, List<List<CellsMerge>> headMerges, List<List<CellsMerge>> bodyMerges, int FreezeRow, int FreezeCol, int decimalDigits, List<ImageData> imageDatas, string nullString, bool colour)
        {
            short color1 = HSSFColor.Teal.Index;
            short color2 = HSSFColor.Lime.Index;
            short color3 = HSSFColor.Gold.Index;

            //Workbook 
            IWorkbook workbook = null;
            if (type == excelType.xls)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = new XSSFWorkbook();
            }
            List<CellsMerge> headMerge = null;
            List<CellsMerge> bodyMerge = null;
            //字体1
            IFont font1 = CellFont(workbook, 12, (short)FontBoldWeight.Bold, HSSFColor.Black.Index);
            //样式1-列头第二列开始
            ICellStyle style1 = CellStyle(workbook, font1, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center);

            // 列头第一列开始
            ICellStyle style1_1 = CellStyle(workbook, font1, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center);

            //表头字体
            IFont font3 = CellFont(workbook, 12, (short)FontBoldWeight.Bold, HSSFColor.Black.Index);
            //表头样式
            ICellStyle headerStyle = CellStyle(workbook, font3, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center);
            ICellStyle headerStyle1 = CellStyle(workbook, font3, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center);

            //字体4
            IFont font4 = CellFont(workbook, 10, (short)FontBoldWeight.Normal, HSSFColor.Black.Index);
            //样式4
            ICellStyle style4 = CellStyle(workbook, font4, NPOI.SS.UserModel.HorizontalAlignment.Left);

            //样式5-第一列datetime
            ICellStyle style5 = workbook.CreateCellStyle();
            //第二列起datetime
            ICellStyle style5_5 = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            style5.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            style5.DataFormat = format.GetFormat("yyyy-mm-dd");
            style5_5.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            style5_5.DataFormat = format.GetFormat("yyyy-mm-dd");

            ICellStyle style5_51 = workbook.CreateCellStyle(); 
            style5_51.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            style5_51.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

            //第一列double,decimal
            ICellStyle style6 = null;
            //第二列起 double,decimal
            ICellStyle style6_6 = null;

            //整数类型保留千分符
            ICellStyle styleInt = CellStyle(workbook, 2, -1);

            //保留小数位数
            if (decimalDigits > 0)
            {
                //样式6
                //style6 = CellStyle(workbook, 1, decimalDigits);
                //style6_6 = CellStyle(workbook, 1, decimalDigits);
                //保留千分位和小数点
                style6 = CellStyle(workbook, 3, decimalDigits);
                style6_6 = CellStyle(workbook, 3, decimalDigits);
            }
            //样式7-数据第一列
            ICellStyle style7 = workbook.CreateCellStyle();

            //edb默认样式
            IFont fontEdbDefault = CellFont(workbook, 9, (short)FontBoldWeight.Normal, HSSFColor.Black.Index);
            fontEdbDefault.FontName = "Simsun";
            ICellStyle styleDefault = workbook.CreateCellStyle();
            styleDefault.SetFont(fontEdbDefault);
            styleDefault.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            styleDefault.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            styleDefault.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            styleDefault.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            if (colour)
            {
                if (style1 != null)
                {
                    style1.FillPattern = FillPattern.Squares;
                    style1.FillForegroundColor = style1.FillBackgroundColor = color2;
                }
                if (style1_1 != null)
                {
                    style1_1.FillPattern = FillPattern.Squares;
                    style1_1.FillForegroundColor = style1_1.FillBackgroundColor = color1;
                }
                if (headerStyle != null)
                {
                    headerStyle.FillPattern = FillPattern.Squares;
                    headerStyle.FillForegroundColor = headerStyle.FillBackgroundColor = color2;
                }
                if (headerStyle1 != null)
                {
                    headerStyle1.FillPattern = FillPattern.Squares;
                    headerStyle1.FillForegroundColor = headerStyle1.FillBackgroundColor = color1;
                }
                if (style5_5 != null)
                {
                    style5_5.FillPattern = FillPattern.Squares;
                    style5_5.FillForegroundColor = style5_5.FillBackgroundColor = color3;
                }
                if (style6_6 != null)
                {
                    style6_6.FillPattern = FillPattern.Squares;
                    style6_6.FillForegroundColor = style6_6.FillBackgroundColor = color3;
                }
                if (style7 != null)
                {
                    style7.FillPattern = FillPattern.Squares;
                    style7.FillForegroundColor = style7.FillBackgroundColor = color3;
                }
            }

            //工作表
            ISheet sheet;

            bool no = true;
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i] == null)
                {
                    continue;
                }
                no = false;
                DataTableData tableData = tables[i];
                DataTable table = tableData.Dt;

                #region EDB数据格式设置
                if (tableData.ExportType == ExportSourceType.Edb)
                {
                    IFont fontEdb1 = CellFont(workbook, 9, (short)FontBoldWeight.Bold, HSSFColor.White.Index);
                    fontEdb1.FontName = "Simsun";
                    //列头-第一列
                    style1_1.SetFont(fontEdb1);
                    style1_1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1_1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1_1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1_1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1_1.FillPattern = FillPattern.SolidForeground;
                    style1_1.FillForegroundColor = HSSFColor.SeaGreen.Index;
                    style1.Alignment = HorizontalAlignment.Right;

                    IFont fontEdb2 = CellFont(workbook, 9, (short)FontBoldWeight.Normal, HSSFColor.Black.Index);
                    fontEdb2.FontName = "Simsun";
                    style1.SetFont(fontEdb2);
                    //列头-其他列
                    style1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style1.FillPattern = FillPattern.SolidForeground;
                    style1.FillForegroundColor = HSSFColor.LightGreen.Index;
                    style1.Alignment = HorizontalAlignment.Left;

                    style7.SetFont(fontEdb2);
                    style7.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style7.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style7.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style7.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style7.FillPattern = FillPattern.SolidForeground;
                    style7.FillForegroundColor = HSSFColor.Gold.Index;
                    style7.Alignment = HorizontalAlignment.Right;

                    style5.SetFont(fontEdb2);
                    style5.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                    style5_5.SetFont(fontEdb2);
                    style5_5.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5_5.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5_5.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style5_5.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                    if (style6 != null)
                    {
                        style6.SetFont(fontEdb2);
                        style6.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    }

                    if (style6_6 != null)
                    {
                        style6_6.SetFont(fontEdb2);
                        style6_6.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6_6.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6_6.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        style6_6.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    }


                }
                #endregion

                if (type == excelType.xls)
                {
                    if (table.Rows.Count > 65501)
                    {
                        int rowCount = table.Rows.Count;
                        for (int j = 65500; j < rowCount; j++)
                        {
                            table.Rows.RemoveAt(65500);
                        }
                        //throw new Exception("超出Office2003的最大行数65535行！");
                    }
                    if (table.Columns.Count > 256)
                    {
                        int colCount = table.Columns.Count;
                        for (int j = 256; j < colCount; j++)
                        {
                            table.Columns.RemoveAt(256);
                        }
                        //throw new Exception("超出Office2003的最大列数255列！");
                    }
                }
                List<string[]> head = null;
                List<int> widths = null;
                try
                {
                    if (headers != null)
                    {
                        head = headers[i];
                    }
                }
                catch { }
                try
                {
                    if (wds != null)
                    {
                        widths = wds[i];
                    }
                }
                catch { }
                //创建工作表
                if (table.TableName == "")
                {
                    sheet = workbook.CreateSheet();
                }
                else
                {
                    try
                    {
                        sheet = workbook.CreateSheet(table.TableName);
                    }
                    catch
                    {
                        sheet = workbook.CreateSheet();
                    }
                }

                int rowIndex = tableData.DtLocation.X;
                //合并后的单元格
                if (headMerges != null && headMerges.Count > 0 && headMerges[i] != null && headMerges[i].Count > 0 && headMerges[i][0] != null)
                {
                    headMerge = headMerges[i];
                    List<CellsMerge> c1 = new List<CellsMerge>();
                    List<CellsMerge> c2 = new List<CellsMerge>();
                    for (int ii = 0; ii < headMerge.Count; ii++)
                    {
                        if (headMerge[ii].Col.X != 0)
                        {
                            c2.Add(headMerge[ii]);
                        }
                        else
                        {
                            c1.Add(headMerge[ii]);
                        }
                    }
                    int a = CellMerge(workbook, sheet, style1_1, c1);
                    int b = CellMerge(workbook, sheet, style1, c2);
                    rowIndex = (a > b) ? a : b;
                }
                else
                {
                    //列头
                    if (head == null)
                    {
                        //行对象
                        IRow headerRow = sheet.GetRow(rowIndex);
                        if (headerRow == null)
                        {
                            headerRow = sheet.CreateRow(rowIndex);
                        }
                        //列头
                        foreach (DataColumn column in table.Columns)
                        {
                            string headName = null;
                            switch (cn)
                            {
                                case columnName.Nono:
                                    ; break;
                                case columnName.Caption:
                                    {
                                        headName = column.Caption.Trim();
                                    }
                                    ; break;
                                default:
                                    {
                                        headName = column.ColumnName.Trim();
                                    }
                                    ; break;
                            }
                            if (column.Ordinal != 0)
                            {
                                SetCellValue(headerRow, widths, column.Ordinal, headerStyle, headName);
                            }
                            else
                            {
                                SetCellValue(headerRow, widths, column.Ordinal, headerStyle1, headName);
                            }
                        }
                        if (cn != columnName.Nono)
                            rowIndex++;
                    }
                    else
                    {
                        foreach (string[] strs in head)
                        {
                            if (strs == null)
                            {
                                break;
                            }
                            //行对象
                            IRow headerRow = sheet.GetRow(rowIndex);
                            if (headerRow == null)
                            {
                                headerRow = sheet.CreateRow(rowIndex);
                            }
                            //列头
                            for (int j = 0, jj = 0; j < strs.Length; j++)
                            {
                                if (strs[j] == null) continue;
                                if (j != 0)
                                {
                                    SetCellValue(headerRow, widths, jj, headerStyle, strs[j]);
                                }
                                else
                                {
                                    SetCellValue(headerRow, widths, jj, headerStyle1, strs[j]);
                                }
                                jj++;
                            }
                            rowIndex++;
                        }
                    }
                }
                int headerCount = rowIndex;
                //添加数据
                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in table.Columns)
                    {
                        ICell ic = dataRow.CreateCell(column.Ordinal);
                        if (column.Ordinal == 0)
                        {
                            ic.CellStyle = style7;
                        }
                        else if (tableData.ExportType == ExportSourceType.Edb) ic.CellStyle = styleDefault;
                        if (row[column] == null)
                        {
                            ic.SetCellValue(nullString);
                            continue;
                        }
                        if (row[column].ToString() == "3.402823E+38" || row[column].ToString() == "1.79769313486232E+308")
                        {
                            ic.SetCellValue(nullString);
                            continue;
                        }
                        if (String.IsNullOrEmpty(row[column].ToString()))
                        {
                            ic.SetCellValue(nullString);
                            continue;
                        }
                        try
                        {
                            switch (column.DataType.Name.ToLower())
                            {
                                case "bool":
                                    {
                                        ic.SetCellValue(Convert.ToBoolean(row[column]));
                                    } break;
                                case "int16":
                                case "int32":
                                case "int64":
                                    {
                                        ic.CellStyle = styleInt;
                                        ic.SetCellValue(Convert.ToInt64(row[column]));
                                    }
                                    ; break;
                                case "single":
                                    {
                                        if (column.Ordinal != 0)
                                        {
                                            if (style6 != null)
                                            {
                                                ic.CellStyle = style6;
                                            }
                                        }
                                        else
                                        {
                                            if (style6_6 != null)
                                            {
                                                ic.CellStyle = style6_6;
                                            }

                                        }
                                        decimal d = Convert.ToDecimal(row[column]);
                                        ic.SetCellValue(Convert.ToDouble(d));
                                    } break;
                                case "double":
                                case "decimal":
                                    {
                                        if (column.Ordinal != 0)
                                        {
                                            if (style6 != null)
                                            {
                                                ic.CellStyle = style6;
                                            }
                                        }
                                        else
                                        {
                                            if (style6_6 != null)
                                            {
                                                ic.CellStyle = style6_6;
                                            }
                                        }
                                        ic.SetCellValue(Convert.ToDouble(row[column]));
                                    } break;
                                case "datetime":
                                    {
                                        if (column.Ordinal != 0)
                                        {
                                            if (column.ExtendedProperties.ContainsKey("datefmt"))
                                            {
                                                ic.CellStyle = style5_51;
                                            }
                                            else
                                            {
                                                ic.CellStyle = style5;                                                
                                            }
                                        }
                                        else
                                        {
                                            ic.CellStyle = style5_5;
                                        }
                                        ic.SetCellValue(Convert.ToDateTime(row[column]));

                                    }; break;
                                default:
                                    {
                                        ic.SetCellValue(row[column].ToString());

                                    }
                                    ; break;
                            }
                        }
                        catch
                        {
                            ic.SetCellValue(row[column].ToString());
                        }
                    }
                    rowIndex++;
                }
                //添加图片
                if (imageDatas != null)
                {
                    for (int j = 0; j < imageDatas.Count; j++)
                    {
                        if (i == imageDatas[j].ImageIndex)
                        {
                            //插入图片
                            CellImag(workbook, sheet, imageDatas[j].Image, imageDatas[j].ImageLocation.X, imageDatas[j].ImageLocation.Y);
                        }
                    }
                }
                //合并后的单元格
                if (bodyMerges != null)
                {
                    bodyMerge = bodyMerges[i];
                    int a = CellMerge(workbook, sheet, bodyMerge);
                    if (rowIndex < a)
                    {
                        rowIndex = a;
                    }
                }
                if (FreezeRow != 0 || FreezeCol != 0)
                {
                    CellFreezePane(sheet, FreezeRow, FreezeCol);
                }
                CellEnd(sheet, rowIndex);
                sheet.CreateFreezePane(0, headerCount, 0, headerCount);

                //columnWidth
                foreach (DataColumn column in table.Columns)
                {
                    if(!column.ExtendedProperties.ContainsKey("width")) continue;
                    int width;
                    int.TryParse(column.ExtendedProperties["width"].ToString(), out width);
                    if (width== 0) continue;
                    sheet.SetColumnWidth(column.Ordinal, width * 300);
                }
            }
            if (no)
            {
                sheet = workbook.CreateSheet();
            }
            return workbook;
        }
        private static IWorkbook ToExcel(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, List<List<int>> wds, List<List<CellsMerge>> headMerges, List<List<CellsMerge>> bodyMerges, int FreezeRow, int FreezeCol, int decimalDigits, string nullString, bool colour)
        {
            List<DataTableData> tablesData = new List<DataTableData>();
            List<ImageData> imgData = null;
            if (img != null)
            {
                imgData = new List<ImageData>();
                imgData.Add(new ImageData(img, 0, new Point(0, 0)));
            }
            if (tables != null)
            {
                for (int i = 0; i < tables.Count; i++)
                {
                    if (i != 0 || img == null)
                    {
                        tablesData.Add(new DataTableData(tables[i], new Point(0, 0)));
                    }
                    else
                    {
                        tablesData.Add(new DataTableData(tables[i], new Point(CellImag(0, img), 0)));
                    }
                }
            }
            return ToExcel(tablesData, headers, cn, wds, headMerges, bodyMerges, FreezeRow, FreezeCol, decimalDigits, imgData, nullString, colour);
        }
        private static IWorkbook ToExcel(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, List<List<int>> wds, List<List<CellsMerge>> headMerges, List<List<CellsMerge>> bodyMerges, int FreezeRow, int FreezeCol, int decimalDigits)
        {
            return ToExcel(tables, headers, cn, img, wds, headMerges, bodyMerges, FreezeRow, FreezeCol, decimalDigits, "——", false);

        }
        private static IWorkbook ToExcel(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, List<List<int>> wds, List<List<CellsMerge>> headMerges, List<List<CellsMerge>> bodyMerges, int FreezeRow, int FreezeCol)
        {
            //return ToExcel(tables, headers, cn, img, wds, headMerges, bodyMerges, FreezeRow, FreezeCol,-1);
            return ToExcel(tables, headers, cn, img, wds, headMerges, bodyMerges, FreezeRow, FreezeCol, 4);
        }
        private static IWorkbook ToExcel(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, List<List<int>> wds, List<List<CellsMerge>> headMerges, List<List<CellsMerge>> bodyMerges)
        {
            return ToExcel(tables, headers, cn, img, wds, headMerges, bodyMerges, 0, 0);
        }
        private static IWorkbook ToExcel(List<DataTable> tables, columnName cn, bool combineTables)
        {
            //Workbook 
            IWorkbook workbook = null;
            if (type == excelType.xls)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = new XSSFWorkbook();
            }
            if (tables == null || tables.Count == 0 || tables[0] == null)
            {
                workbook.CreateSheet();
                return workbook;
            }
            if (!combineTables)
            {
                return ToExcel(tables, null, cn, null, null, null, null);
            }

            //创建工作表
            ISheet sheet;

            if (tables[0].TableName != "")
            {
                sheet = workbook.CreateSheet(tables[0].TableName);
            }
            else
            {
                sheet = workbook.CreateSheet();
            }
            //字体2
            IFont font2 = CellFont(workbook, 12, (short)FontBoldWeight.Bold, HSSFColor.Red.Index);
            //样式2
            ICellStyle style2 = CellStyle(workbook, font2, NPOI.SS.UserModel.HorizontalAlignment.Left);
            //表头字体
            IFont font3 = CellFont(workbook, 12, (short)FontBoldWeight.Bold, HSSFColor.Black.Index);
            //表头样式
            ICellStyle headerStyle = CellStyle(workbook, font3, NPOI.SS.UserModel.HorizontalAlignment.Center);

            //表头样式
            IDataFormat id = workbook.CreateDataFormat();
            ICellStyle style4 = CellStyle(workbook, NPOI.SS.UserModel.HorizontalAlignment.Right, id, 4);

            //样式5
            ICellStyle style5 = workbook.CreateCellStyle();
            style5.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            style5.DataFormat = id.GetFormat("yyyy-mm-dd");

            int rowIndex = 0;
            //行对象
            IRow headerRow = sheet.CreateRow(rowIndex);

            //列头
            foreach (DataColumn column in tables[0].Columns)
            {
                string headName = null;
                switch (cn)
                {
                    case columnName.Nono:
                        ; break;
                    case columnName.Caption:
                        {
                            headName = column.Caption.Trim();
                        }
                        ; break;
                    default:
                        {
                            headName = column.ColumnName.Trim();
                        }
                        ; break;
                }
                if (headName != null)
                {
                    SetCellValue(headerRow, null, column.Ordinal, headerStyle, headName);
                }
            }
            rowIndex++;

            foreach (DataTable table in tables)
            {
                if (table == null)
                {
                    continue;
                }
                if (type == excelType.xls)
                {
                    if (table.Rows.Count > 65501)
                    {
                        int rowCount = table.Rows.Count;
                        for (int j = 65500; j < rowCount; j++)
                        {
                            table.Rows.RemoveAt(65500);
                        }
                        //throw new Exception("超出Office2003的最大行数65535行！");
                    }
                    if (table.Columns.Count > 256)
                    {
                        int colCount = table.Columns.Count;
                        for (int j = 256; j < colCount; j++)
                        {
                            table.Columns.RemoveAt(256);
                        }
                        //throw new Exception("超出Office2003的最大列数255列！");
                    }
                }
                //添加数据
                foreach (DataRow row in table.Rows)
                {
                    //IRow dataRow =sheet.GetRow(rowIndex);
                    //if (dataRow == null)
                    //{
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    //}
                    foreach (DataColumn column in table.Columns)
                    {
                        ICell ic = dataRow.CreateCell(column.Ordinal);
                        //ic.CellStyle = headerStyle;
                        if (row[column] == null)
                        {
                            ic.SetCellValue("－－");
                        }
                        try
                        {
                            switch (column.DataType.Name.ToLower())
                            {
                                case "bool":
                                    {
                                        ic.SetCellValue(Convert.ToBoolean(row[column]));
                                    } break;
                                case "int16":
                                case "int32":
                                case "int64":
                                    {
                                        ic.SetCellValue(Convert.ToInt64(row[column]));
                                    }
                                    ; break;
                                case "single":
                                    {
                                        decimal d = Convert.ToDecimal(row[column]);
                                        ic.CellStyle = style4;
                                        ic.SetCellValue(Convert.ToDouble(d));
                                    } break;
                                case "double":
                                case "decimal":
                                    {
                                        ic.CellStyle = style4;
                                        ic.SetCellValue(Convert.ToDouble(row[column]));
                                    } break;
                                case "datetime":
                                    {
                                        ic.CellStyle = style5;
                                        ic.SetCellValue(Convert.ToDateTime(row[column]));
                                    }; break;
                                default:
                                    {
                                        ic.SetCellValue(row[column].ToString());

                                    }
                                    ; break;
                            }
                        }
                        catch
                        {
                            ic.SetCellValue(row[column].ToString());
                        }
                    }
                    rowIndex++;
                }
            }
            CellEnd(sheet, rowIndex);

            return workbook;
        }
        private static IWorkbook ToExcel(Image img)
        {
            //Workbook 
            IWorkbook workbook = null;
            if (type == excelType.xls)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = new XSSFWorkbook();
            }

            //工作表
            ISheet sheet = workbook.CreateSheet();

            int rowIndex = 0;
            //添加图片
            if (img != null)
            {
                rowIndex += CellImag(rowIndex, img);
                CellImag(workbook, sheet, img, 0, 0);
            }

            CellEnd(sheet, rowIndex);

            return workbook;
        }
        #endregion

        #region 转换
        private static byte[] convertByte(Image img)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                img.Save(ms, img.RawFormat);
                byte[] bytes = ms.ToArray();
                return bytes;
            }
            catch
            {

            }
            finally
            {
                ms.Close();
            }
            return null;

        }
        private static Image convertImg(byte[] datas)
        {
            MemoryStream ms = new MemoryStream(datas);
            Image img = Image.FromStream(ms, true);
            ms.Close();
            return img;
        }
        private static DataTable converDatatable(DataView data)
        {
            return data.Table;
        }

        private static DataTable converDatatable(HtmlElement tableElement, out List<int> widths, out string[] header, out List<CellsMerge> headMerge, out List<CellsMerge> bodyMerge)
        {
            //File.WriteAllText("d:\\a.txt", tableElement.OuterHtml);
            List<int> reMove = new List<int>();
            headMerge = new List<CellsMerge>();
            bodyMerge = new List<CellsMerge>();
            DataTable dt = new DataTable();
            DataRow dr = null;
            DataColumn dc = null;
            header = null;
            List<string> head = new List<string>();
            widths = new List<int>();
            List<HtmlElement> trElements = new List<HtmlElement>();

            //收集Tr元素
            CollectTrElements(trElements, tableElement);

            //创建行列式描述
            Dictionary<string, Determinant> determinants = new Dictionary<string, Determinant>();

            //初始化第一个单元格
            //Log.WriteLog("Excel导出日志：初始化第一个单元格");
            Determinant firstDeterminant = new Determinant(0, 0, 1, 1);
            determinants[firstDeterminant.ToString()] = firstDeterminant;
            int colCount = 1;

            //循环遍历Tr
            int rowCount = trElements.Count;
            int nextColStart = 1;
            int rCount = 0;
            int rowSpanNum = 0;
            int colSpanNum = 0;
            bool merger = false;
            int rowSpanCout = 0;
            DateTime dt2 = DateTime.Now;

            //header = new string[trElements[trElements.Count-1].Children.Count];
            for (int k = 0; k < trElements[trElements.Count - 1].Children.Count; k++)
            {
                if (trElements[trElements.Count - 1].Children[k].GetAttribute("exporthidden") == "true")
                {
                    reMove.Add(k);
                }
                dc = new DataColumn();
                dt.Columns.Add(dc);
            }

            for (int i = 0; i < trElements.Count; i++)
            {
                rowSpanNum = i;
                dr = dt.NewRow();

                //获取TR元素
                HtmlElement trElement = trElements[i];
                if (trElement.Children.Count > colCount)
                {
                    colCount = trElement.Children.Count;
                }
                if (rCount > 0)
                {
                    rCount--;
                    if (rCount == 0) nextColStart = 1;
                }
                if (trElement.Children.Count > 0)
                {
                    //循环遍历TR子元素
                    bool addflag = true;

                    for (int j = 0; j < trElement.Children.Count; j++)
                    {
                        //获取TD元素
                        HtmlElement tdElement = trElement.Children[j];

                        try
                        {
                            if (tdElement.GetAttribute("title") != null && tdElement.GetAttribute("title") != "")
                            {
                                tdElement.InnerText = tdElement.GetAttribute("title");
                            }
                        }
                        catch
                        {
#if DEBUG
                            MessageBox.Show("导出网页到Excel错误!");
#endif
                        }
                        if (tdElement.OuterHtml.Replace(" ", "").IndexOf("display:none", StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            continue;
                        }
                        //获取ColSpan
                        int colSpan = 1;
                        int rowSpan = 1;
                        string title = string.Empty;
                        try
                        {
                            //获取colSpan和rowSpan
                            colSpan = Convert.ToInt32(tdElement.GetAttribute("colSpan"));
                            rowSpan = Convert.ToInt32(tdElement.GetAttribute("rowSpan"));
                            //获取标题
                            title = tdElement.GetAttribute("sheetname");
                            if (title == null || title.Length == 0)
                            {
                                title = tdElement.GetAttribute("paraname");
                            }
                        }
                        catch (Exception err)
                        {

                        }

                        if (colSpan == 1 && rowSpan == 1)
                        {
                            addflag = false;
                            //merger = false;
                        }
                        if (colSpan > 1)
                        {
                            merger = true;
                        }
                        if (rowSpan > 1)
                        {
                            merger = true;
                            rowSpanCout = rowSpan;
                        }

                        //获取rowspan
                        //获取行列式
                        string identifier = "tr:" + i.ToString() + "td:" + j.ToString();
                        Determinant thisTd = null;
                        if (determinants.ContainsKey(identifier))
                        {
                            thisTd = determinants[identifier];
                        }
                        else
                        {
                            if (addflag)
                            {
                                thisTd = new Determinant(j, i, i + 1, nextColStart);
                            }
                            else
                            {
                                nextColStart = j + 1;
                                thisTd = new Determinant(j, i, i + 1, j + 1);
                            }
                            determinants[identifier] = thisTd;
                        }
                        int row = thisTd.Row;
                        int col = thisTd.Col;
                        if (j == 0)
                        {
                            col = nextColStart;
                            thisTd.Col = col;
                        }
                        //生成本行接下来一格的行列信息
                        Determinant thisColNext = new Determinant(j + 1, i, row, col + colSpan);
                        determinants[thisColNext.ToString()] = thisColNext;

                        for (int r = 1; r < rowSpan; r++)
                        {
                            Determinant rowNext = new Determinant(j, i + r, row + r, col);
                            if (j > 0)
                            {
                                if (trElement.Children[j - 1].GetAttribute("rowSpan") != "1")
                                {
                                    rowNext.Col = col + colSpan;
                                    rowNext.TdPos -= colSpan;
                                }
                            }
                            determinants[rowNext.ToString()] = rowNext;
                        }
                        //根据描述生成Excel
                        if (tdElement.TagName == "TH")
                        {
                            string headTitle = null;
                            string dataType = null;
                            if (title != null && title.Length > 0)
                            {
                                headTitle = title;

                            }
                            else
                            {
                                if (tdElement.InnerText != null)
                                {
                                    string tempStr = GetValue(tdElement.OuterHtml, "exportvalue");
                                    dataType = GetValue(tdElement.OuterHtml, "exportdatatype");
                                    if (tempStr == null)
                                    {
                                        headTitle = tdElement.InnerText.Trim();
                                    }
                                    else
                                    {
                                        headTitle = tempStr;
                                    }
                                }
                            }
                            //if (!merger)
                            //{
                            //    try
                            //    {
                            //        int temp = Convert.ToInt32(Convert.ToInt32(tdElement.DomElement.GetType().GetProperty("clientWidth").GetValue(tdElement.DomElement, null)) / 8.5 + 2);
                            //        widths.Add(temp * 11);
                            //    }
                            //    catch
                            //    {

                            //    }
                            //    //header[j] = headTitle;
                            //    head.Add(headTitle);
                            //}
                            //else
                            {
                                CellsMerge cm = new CellsMerge(row - 1, row + rowSpan - 2, col - 1, col + colSpan - 2, headTitle);
                                if (dataType != null)
                                {
                                    cm.DataType = dataType;
                                }
                                headMerge.Add(cm);
                            }
                            dr = null;
                            rowSpanCout = 0;
                        }
                        else
                        {
                            string bodyTitle = null;
                            string dataType = null;
                            if (title != null && title.Length > 0)
                            {
                                bodyTitle = title;
                            }
                            else
                            {
                                if (tdElement.InnerText != null)
                                {
                                    string tempStr = GetValue(tdElement.OuterHtml, "exportvalue");
                                    dataType = GetValue(tdElement.OuterHtml, "exportdatatype");
                                    if (tempStr == null)
                                    {
                                        bodyTitle = tdElement.InnerText.Trim();
                                    }
                                    else
                                    {
                                        bodyTitle = tempStr;
                                    }
                                }
                            }
                            //if (!merger)
                            //{
                            //    if (dr == null)
                            //    {
                            //        dr = dt.NewRow();
                            //    }
                            //    dr[j] = bodyTitle;
                            //}
                            //else
                            {
                                CellsMerge cm = new CellsMerge(row - 1, row + rowSpan - 2, col - 1, col + colSpan - 2, bodyTitle);
                                if (dataType != null)
                                {
                                    cm.DataType = dataType;
                                }
                                bodyMerge.Add(cm);
                            }
                        }
                        if (addflag)
                        {
                            if (rowSpan > 1)
                            {
                                nextColStart += colSpan;
                                rCount = rowSpan;
                            }
                        }
                        if (rowSpan <= 1)
                        {
                            addflag = false;
                        }
                        if (j == 0 && !addflag && rCount <= 0)
                        {
                            nextColStart = 1;
                        }
                    }
                    if (dr != null)
                    {
                        //dt.Rows.Add(dr);
                        if (rowSpanCout == 0)
                        {
                            merger = false;
                        }
                        else
                        {
                            rowSpanCout--;
                        }
                    }
                }
                colSpanNum = 0;
                rowSpanNum++;
            }
            header = head.ToArray();
            TimeSpan ts = DateTime.Now - dt2;
            double sec = ts.TotalMilliseconds;
            determinants.Clear();
            trElements.Clear();
            //去掉隐藏列
            if (reMove.Count > 0)
            {
                List<DataColumn> dtRemove = new List<DataColumn>();
                List<CellsMerge> headMergeRemove = new List<CellsMerge>();
                List<CellsMerge> bodyMergeRemove = new List<CellsMerge>();
                for (int i = 0; i < reMove.Count; i++)
                {
                    for (int j = 0; j < headMerge.Count; j++)
                    {
                        if ((headMerge[j].Col.X == headMerge[j].Col.Y) && (headMerge[j].Col.X == reMove[i]))
                        {
                            headMergeRemove.Add(headMerge[j]);
                        }
                    }
                    for (int j = 0; j < bodyMerge.Count; j++)
                    {
                        if ((bodyMerge[j].Col.X == bodyMerge[j].Col.Y) && (bodyMerge[j].Col.X == reMove[i]))
                        {
                            bodyMergeRemove.Add(bodyMerge[j]);
                        }
                    }
                }

                for (int i = 0; i < reMove.Count; i++)
                {
                    for (int j = 0; j < headMerge.Count; j++)
                    {
                        if (headMerge[j].Col.X >= reMove[i])
                        {
                            headMerge[j].Col = new Point(headMerge[j].Col.X - 1, headMerge[j].Col.Y - 1);
                        }
                    }
                    for (int j = 0; j < bodyMerge.Count; j++)
                    {
                        if (bodyMerge[j].Col.X >= reMove[i])
                        {
                            bodyMerge[j].Col = new Point(bodyMerge[j].Col.X - 1, bodyMerge[j].Col.Y - 1);
                        }
                    }
                    dtRemove.Add(dt.Columns[reMove[i]]);
                }
                for (int i = 0; i < headMergeRemove.Count; i++)
                {
                    headMerge.Remove(headMergeRemove[i]);
                }
                for (int i = 0; i < bodyMergeRemove.Count; i++)
                {
                    bodyMerge.Remove(bodyMergeRemove[i]);
                }
                for (int i = 0; i < dtRemove.Count; i++)
                {
                    dt.Columns.Remove(dtRemove[i]);
                }
            }
            return dt;
        }
        private static List<DataTable> converDatatable(string html, out List<List<int>> wds, out List<List<string[]>> headers, out List<List<CellsMerge>> headMerges, out List<List<CellsMerge>> bodyMerges)
        {
            headMerges = new List<List<CellsMerge>>();
            bodyMerges = new List<List<CellsMerge>>();
            List<CellsMerge> headMerge = new List<CellsMerge>();
            List<CellsMerge> bodyMerge = new List<CellsMerge>();
            List<DataTable> lt = new List<DataTable>();
            wds = new List<List<int>>();
            List<string[]> headTemp = new List<string[]>();
            headers = new List<List<string[]>>();
            List<int> wd = new List<int>();
            string[] header = null;
            try
            {
                //创建WebBrowser用于创建对象
                WebBrowser web = new WebBrowser();

                web.ScriptErrorsSuppressed = true;

                web.Navigate("about:blank");
                web.Document.Write(html);

                if (web.Document.Body != null && web.Document.Body.Children != null && web.Document.Body.Children.Count > 0)
                {
                    foreach (HtmlElement element in web.Document.Body.Children)
                    {
                        if (element.TagName == "TABLE")
                        {
                            //导出
                            string sheetName = element.GetAttribute("sheetname").ToString();
                            DataTable dt = null;
                            try
                            {
                                dt = converDatatable(element, out wd, out header, out headMerge, out bodyMerge);
                            }
                            catch
                            {
                                dt = new DataTable();
                            }
                            if (sheetName != "")
                            {
                                dt.TableName = sheetName;
                            }
                            headMerges.Add(headMerge);
                            bodyMerges.Add(bodyMerge);
                            headTemp.Add(header);
                            headers.Add(headTemp);
                            headTemp = new List<string[]>();
                            wds.Add(wd);
                            lt.Add(dt);
                            dt = null;
                        }
                    }
                }
                web.Dispose();
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show("ERROR MSG:" + ex.Message);
#endif
            }
            return lt;
        }
        private static void CollectTrElements(List<HtmlElement> trElements, HtmlElement element)
        {
            if (element.TagName == "TR")
            {
                trElements.Add(element);
            }
            else
            {
                //递归搜集
                if (element.Children.Count > 0)
                {
                    for (int i = 0; i < element.Children.Count; i++)
                    {
                        CollectTrElements(trElements, element.Children[i]);
                    }
                }
            }
        }
        private static List<CellsMerge> converHeadMerge(string headerHtml)
        {
            int headerRowCount;
            return converHeadMerge(headerHtml, out headerRowCount);
        }
        private static List<CellsMerge> converHeadMerge(string headerHtml, out int headerRowCount)
        {
            List<CellsMerge> headMerge = new List<CellsMerge>();
            string temp = headerHtml.Replace("<table>", "").Replace("</table>", "").Replace("<tr>", "Θ").Replace("</tr>", "Θ");
            int row = 0;
            int col = 0;
            //行
            string[] array = temp.Split(new char[] { 'Θ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in array)
            {
                //列
                temp = str.Replace("</td>", "Θ");
                string[] array2 = temp.Split(new char[] { 'Θ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str2 in array2)
                {
                    string[] array3 = str2.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                    string text = "";
                    int colNum = 1;
                    if (array3.Length == 2)
                    {
                        text = array3[1].Trim();
                    }
                    if (array3[0].Replace(" ", "").Contains("colspan="))
                    {
                        string[] arr = array3[0].Split(new char[] { '\'' }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            colNum = Convert.ToInt32(arr[1]);
                        }
                        catch { }
                    }
                    headMerge.Add(new CellsMerge(row, row, col, col + colNum - 1, text));
                    col = col + colNum;
                }
                col = 0;
                row++;
            }

            headerRowCount = row;

            //headMerge.Add(new HeadMerge(row - 1, row + rowSpan - 2, col - 1, col + colSpan - 2, headTitle));
            return headMerge;
        }
        private static string GetValue(string outPutHtml, string keyWords)
        {
            if (outPutHtml.Contains(keyWords))
            {
                //File.WriteAllText(@"d:\text.txt", outPutHtml);
                string[] arr = outPutHtml.Split(new char[] { '\"' });
                string keyWord = keyWords.Trim() + "=";
                bool output = false;
                foreach (string str in arr)
                {
                    if (output == true)
                    {
                        return str;
                    }
                    if (str.Replace(" ", "").Contains(keyWord))
                    {
                        output = true;
                    }
                }
            }
            return null;
        }
        #endregion

        #region DataTable
        public static void SaveToFile(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, string fileName, List<List<int>> widths, int FreezeRow, int FreezeCol)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(tables, headers, cn, img, widths, null, null, FreezeRow, FreezeCol).Write(fs);
            }
        }
        public static void SaveToFile(List<DataTable> tables, List<List<string[]>> headers, columnName cn, Image img, string fileName, List<List<int>> widths)
        {
            SaveToFile(tables, headers, cn, img, fileName, widths, 0, 0);
        }
        public static void SaveToFile(DataTable table, List<string[]> head, columnName cn, Image img, string fileName)
        {
            List<DataTable> lt = new List<DataTable>();
            List<List<string[]>> headers = new List<List<string[]>>();
            lt.Add(table);
            headers.Add(head);
            SaveToFile(lt, headers, cn, img, fileName, null);
        }
        public static void SaveToFile(List<DataTable> tables, columnName cn, bool combineTables, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(tables, cn, combineTables).Write(fs);
            }
        }
        public static void SaveToFile(DataTable table, string fileName)
        {
            List<DataTable> lt = new List<DataTable>();
            lt.Add(table);
            SaveToFile(lt, null, columnName.ColumnName, null, fileName, null);
        }
        public static void SaveToFile(List<DataTableData> tables, List<ImageData> images, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(tables, null, columnName.Nono, null, null, null, 0, 0, -1, images, "－－", false).Write(fs);
            }
        }
        public static void SaveToFile(List<DataTableData> tables, List<ImageData> images, string nullString, string fileName, bool colour)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(tables, null, columnName.ColumnName, null, null, null, 0, 0, 4, images, nullString, colour).Write(fs);
            }
        }
        public static void SaveToFile(DataTable table, string headerHtml, string nullString, string fileName, bool colour)
        {
            CheckExt(fileName);
            List<DataTable> lt = new List<DataTable>();
            List<CellsMerge> headMerge = converHeadMerge(headerHtml);
            List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
            headMerges.Add(headMerge);
            lt.Add(table);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(lt, null, columnName.Nono, null, null, headMerges, null, 0, 0, -1, nullString, colour).Write(fs);
            }

            //DataTableData tableData = new DataTableData(table, new Point(0, 0));
            //tableData.ExportType = ExportSourceType.Edb;
            //SaveToFile(tableData, headerHtml, nullString, fileName, colour);
        }

        /// <summary>
        /// Notes: 导出表格数据
        /// Author: jiejiep
        /// </summary>
        /// <param name="table"></param>
        /// <param name="headerHtml"></param>
        /// <param name="nullString"></param>
        /// <param name="fileName"></param>
        /// <param name="colour"></param>
        public static void SaveToFile(DataTableData table, string headerHtml, string nullString, string fileName, bool colour)
        {
            CheckExt(fileName);
            int headerRowCount;
            List<CellsMerge> headMerge = converHeadMerge(headerHtml, out headerRowCount);
            List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
            headMerges.Add(headMerge);
            List<DataTableData> tablesData = new List<DataTableData>();
            tablesData.Add(table);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(tablesData, null, columnName.Nono, null, headMerges, null, headerRowCount, 0, -1, null, nullString, colour).Write(fs);
            }
        }
        public static void SaveToFile(DataTableData table, string headerHtml, ImageData image, string fileName, bool colour)
        {
            CheckExt(fileName);
            List<DataTableData> lt = new List<DataTableData>();
            List<CellsMerge> headMerge = converHeadMerge(headerHtml);
            List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
            headMerges.Add(headMerge);
            //table.ExportType = ExportSourceType.Edb;
            lt.Add(table);
            List<ImageData> images = null;
            if (image != null)
            {
                images = new List<ImageData>();
                images.Add(image);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(lt, null, columnName.Nono, null, headMerges, null, 0, 0, -1, images, "--", colour).Write(fs);
            }
        }
        #endregion

        #region DataView
        public static void SaveToFile(List<DataView> tables, List<string> headers, string fileName)
        {
            CheckExt(fileName);
            List<DataTable> lt = new List<DataTable>();
            List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
            foreach (string header in headers)
            {
                List<CellsMerge> headMerge = converHeadMerge(header);
                headMerges.Add(headMerge);
            }
            foreach (DataView table in tables)
            {
                lt.Add(converDatatable(table));
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(lt, null, columnName.Nono, null, null, headMerges, null).Write(fs);
            }
        }
        public static void SaveToFile(List<DataView> tables, List<List<string[]>> headers, columnName cn, Image img, string fileName)
        {
            List<DataTable> lt = new List<DataTable>();
            foreach (DataView dv in tables)
            {
                lt.Add(converDatatable(dv));
            }
            SaveToFile(lt, headers, cn, img, fileName, null);
        }
        public static void SaveToFile(DataView table, List<string[]> head, columnName cn, Image img, string fileName)
        {
            List<DataTable> lt = new List<DataTable>();
            List<List<string[]>> headers = new List<List<string[]>>();
            lt.Add(converDatatable(table));
            headers.Add(head);
            SaveToFile(lt, headers, cn, img, fileName, null);
        }
        public static void SaveToFile(DataView table, string fileName)
        {
            List<DataTable> lt = new List<DataTable>();
            lt.Add(converDatatable(table));
            SaveToFile(lt, null, columnName.ColumnName, null, fileName, null);
        }
        public static void SaveToFile(DataView table, string headerHtml, string fileName)
        {
            CheckExt(fileName);
            List<DataTable> lt = new List<DataTable>();
            List<CellsMerge> headMerge = converHeadMerge(headerHtml);
            List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
            headMerges.Add(headMerge);
            lt.Add(converDatatable(table));
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(lt, null, columnName.Nono, null, null, headMerges, null).Write(fs);
            }
        }
        #endregion

        #region 网页以及图片
        public static void SaveToFile(Image img, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ToExcel(img).Write(fs);
            }
        }
        public static void SaveToFile(string html, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                List<List<int>> wds = new List<List<int>>();
                List<List<string[]>> headers = new List<List<string[]>>();
                //headMerges = new List<List<HeadMerge>>();
                List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
                List<List<CellsMerge>> bodyMerges = new List<List<CellsMerge>>();
                List<DataTable> dts = converDatatable(html, out wds, out headers, out headMerges, out bodyMerges);
                ToExcel(dts, headers, columnName.ColumnName, null, wds, headMerges, bodyMerges).Write(fs);
            }
        }
        public static void SaveToFile(HtmlElement tableElement, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                List<int> wd = new List<int>();
                List<List<int>> wds = new List<List<int>>();
                List<string[]> headerTemp = new List<string[]>();
                string[] header = null;
                List<List<string[]>> headers = new List<List<string[]>>();
                List<List<CellsMerge>> headMerges = new List<List<CellsMerge>>();
                List<List<CellsMerge>> bodyMerges = new List<List<CellsMerge>>();
                List<CellsMerge> headMerge = new List<CellsMerge>();
                List<CellsMerge> bodyMerge = new List<CellsMerge>();

                List<DataTable> dts = new List<DataTable>();
                dts.Add(converDatatable(tableElement, out wd, out header, out headMerge, out bodyMerge));
                wds.Add(wd);
                headerTemp.Add(header);
                headers.Add(headerTemp);
                headMerges.Add(headMerge);
                bodyMerges.Add(bodyMerge);
                headerTemp = new List<string[]>();
                ToExcel(dts, headers, columnName.ColumnName, null, wds, headMerges, bodyMerges).Write(fs);
            }
        }
        #endregion

        #region 注释和公式
        public static void SaveToFile(string formula, string note, int r, int c, int noteRow1, int noteCol1, int noteRow2, int noteCol2, bool noteVisible, string sheetName, string fileName)
        {
            CheckExt(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = null;
                if (type == excelType.xls)
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = new XSSFWorkbook();
                }
                ISheet sheet;
                if (sheetName == null)
                {
                    sheet = CreateSheet(workbook);
                }
                else
                {
                    sheet = CreateSheet(workbook, sheetName);
                }
                IRow row = CreateRow(sheet, r);
                ICell cell = CreateCell(row, c);
                cell.SetCellValue(formula);
                IDrawing patr = sheet.CreateDrawingPatriarch();
                IComment comment = null;
                if (type == excelType.xls)
                {
                    comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, noteCol1, noteRow1, noteCol2 + 2, noteRow2 + 2));
                    comment.String = new HSSFRichTextString(note);
                }
                else
                {
                    comment = patr.CreateCellComment(new XSSFClientAnchor(0, 0, 0, 0, noteCol1, noteRow1, noteCol2 + 2, noteRow2 + 2));
                    comment.String = new XSSFRichTextString(note);
                }
                comment.Author = "坤仪";
                comment.Visible = true;
                cell.CellComment = comment;
                workbook.Write(fs);
            }
        }
        #endregion

        #region 手动拼接
        public static IWorkbook CreateWorkbook()
        {
            return new HSSFWorkbook();
        }
        public static ISheet CreateSheet(IWorkbook workbook)
        {
            return workbook.CreateSheet();
        }
        public static ISheet CreateSheet(IWorkbook workbook, string sheetName)
        {
            return workbook.CreateSheet(sheetName);
        }
        public static IRow CreateRow(ISheet sheet, int row)
        {
            return sheet.CreateRow(row);
        }
        public static ICell CreateCell(IRow row, int col)
        {
            return row.CreateCell(col);
        }
        public static ICell CreateCell(IRow row, int col, CellType cellType)
        {
            return row.CreateCell(col, cellType);
        }

        #endregion
        #endregion

        #region 读取Excel到DataTable

        /// <summary>
        /// 读取Excel到DataTable
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataSet ReadExcelToDataSet(string filePath)
        {
            DataSet ds = new DataSet();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                // 构造 XSSFWorkbook 对象，strPath 传入文件路径  
                IWorkbook workBook = null;

                if (Path.GetExtension(filePath).ToLower().Equals(".xlsx"))
                {
                    workBook = new XSSFWorkbook(fs);
                }
                else if (Path.GetExtension(filePath).ToLower().Equals(".xls"))
                {
                    workBook = new HSSFWorkbook(fs);
                }
                int sheetCount = workBook.NumberOfSheets;
                for (int i = 0; i < sheetCount; i++)
                {
                    try
                    {
                        ISheet sheet = workBook.GetSheetAt(i);
                        DataTable table = _ReadSheetDataTable(sheet);
                        if (table == null) continue;
                        ds.Tables.Add(table);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 获取当前Sheet页的有效起始列、截至列
        /// </summary>
        /// <param name="sheet">当前Sheet页</param>
        /// <param name="startColIndex">起始列</param>
        /// <param name="endColIndex">截止列</param>
        /// <param name="cellCount">有效值单元格列数</param>
        private static void _GetSheetInfos(ISheet sheet, out int startColIndex, out int endColIndex, out int cellCount)
        {
            startColIndex = 0;
            endColIndex = 0;
            cellCount = 0;
            for (int i = sheet.FirstRowNum; i <= sheet.PhysicalNumberOfRows; ++i)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                bool isAllBlank = true;
                foreach ( ICell cell in row.Cells)
                {
                    if (cell.CellType != CellType.Blank)
                    {
                        isAllBlank = false;
                        break;
                    }
                }
                if (isAllBlank) continue;
                if (row.FirstCellNum > startColIndex) startColIndex = row.FirstCellNum;
                if (row.LastCellNum > endColIndex) endColIndex = row.LastCellNum;
                if (row.PhysicalNumberOfCells > cellCount) cellCount = row.PhysicalNumberOfCells;
            }
        }

        /// <summary>
        /// 读取Sheet页内容到DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static DataTable _ReadSheetDataTable(ISheet sheet)
        {
            DataTable dataTable = new DataTable();
            // 定义 row、cell  

            int startRowIndex = 0;
            IRow firstRow = sheet.GetRow(startRowIndex);
            int count = 0;
            while (firstRow == null)
            {
                //过滤Sheet页头的空行
                startRowIndex++;
                firstRow = sheet.GetRow(startRowIndex);
                count++;
                if (count > 200)
                {
                    return null;
                }
            }

            int startColIndex, endColIndex, cellCount;
            _GetSheetInfos(sheet, out startColIndex, out endColIndex, out cellCount);

            for (int i = 0; i < cellCount; i++)
            {
                //if (firstRow.GetCell(i) != null)
                //{
                //    dataTable.Columns.Add(firstRow.GetCell(i).StringCellValue ?? string.Format("F{0}", i + 1), typeof(string));
                //}
                //else
                //{
                //    dataTable.Columns.Add(string.Format("F{0}", i + 1), typeof(string));
                //}
                //列头固定F1,F2,F3....
                dataTable.Columns.Add(string.Format("F{0}", i + 1), typeof(string));
            }
            //Sheet页中所有内容作为数据记录写入DataTable
            for (int i = startRowIndex; i <= sheet.PhysicalNumberOfRows; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow tableRow = dataTable.NewRow();
                if (_FillDataRow(row, startColIndex, endColIndex, ref tableRow))
                    dataTable.Rows.Add(tableRow);
            }

            dataTable.TableName = sheet.SheetName;
            return dataTable;
        }

        /// <summary>
        /// Excel行转换为DataTable行
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private static bool _FillDataRow(IRow row, int startColIndex, int endColIndex, ref DataRow dataRow)
        {
            bool hasValue = false;
            if (row == null) return hasValue;
            for (int m = startColIndex; m < endColIndex; m++)
            {
                int i = m - startColIndex;
                ICell cell = row.GetCell(m);
                if (cell == null)
                {
                    dataRow[i] = DBNull.Value;
                    continue;
                }

                switch (cell.CellType)
                {
                    case CellType.Blank:
                        dataRow[i] = DBNull.Value;
                        break;
                    case CellType.Boolean:
                        dataRow[i] = cell.BooleanCellValue;
                        hasValue = true;
                        break;
                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            dataRow[i] = cell.DateCellValue;
                        }
                        else
                        {
                            dataRow[i] = cell.NumericCellValue;
                        }
                        hasValue = true;
                        break;
                    case CellType.Error:
                        dataRow[i] = cell.ErrorCellValue;
                        break;
                    case CellType.Formula:
                        if (cell.CachedFormulaResultType == CellType.Error)
                        {
                            dataRow[i] = DBNull.Value;
                        }
                        else if (cell.CachedFormulaResultType == CellType.Numeric)
                        {
                            dataRow[i] = cell.NumericCellValue;
                        }
                        else if (cell.CachedFormulaResultType == CellType.Boolean)
                        {
                            dataRow[i] = cell.BooleanCellValue;
                        }
                        else
                        {
                            dataRow[i] = cell.StringCellValue;
                        }
                        break;
                    case CellType.String:
                    default:
                        dataRow[i] = cell.StringCellValue;
                        hasValue = true;
                        break;
                }
            }
            return hasValue;
        }

        #endregion
    }
}
