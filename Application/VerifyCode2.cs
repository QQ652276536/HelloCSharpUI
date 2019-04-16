using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace HelloCSharp
{
    /// <summary>
    /// 验证码生成类
    /// </summary>
    public class VerifyCode2
    {
        Random random = new Random();
        //生成的验证码字符串
        public char[] charArray = null;
        //验证码
        private string validationCode = null;
        public string ValidationCode
        {
            get { return validationCode; }
        }

        //验证码字符串的长度
        private Int32 validationCodeCount = 4;
        public Int32 ValidationCodeCount
        {
            get { return validationCodeCount; }
            set { validationCodeCount = value; }
        }

        Graphics graphics = null;
        //验证码的宽度
        private int codeWidth = 80;
        public Int32 Width
        {
            get { return codeWidth; }
            set { codeWidth = value; }
        }

        //验证码的高度
        private int codeHeight = 27;
        public Int32 Height
        {
            get { return codeHeight; }
            set { codeHeight = value; }
        }

        // 验证码字体列表，默认为{ "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" }
        private string[] fontFace = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
        public string[] FontFace
        {
            get { return fontFace; }
            set { fontFace = value; }
        }

        // 验证码字体的最小值，默认为15,建议不小于15像素
        private int fontMinSize = 15;
        public Int32 FontMinSize
        {
            get { return fontMinSize; }
            set { fontMinSize = value; }
        }

        // 验证码字体的最大值
        private Int32 fontMaxSize = 15;
        public Int32 FontMaxSize
        {
            get { return fontMaxSize; }
            set { fontMaxSize = value; }
        }

        // 验证码字体的颜色，默认为系统自动生成字体颜色
        private Color[] fontColor = { };
        public Color[] FontColor
        {
            get { return fontColor; }
            set { fontColor = value; }
        }

        // 验证码的背景色，默认为Color.FromArgb(243, 251, 254)
        private Color backColor = Color.FromArgb(243, 255, 255);
        public Color BackgroundColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        // 贝塞尔曲线的条数,默认为3条
        private Int32 bezierCount = 3;
        public Int32 BezierCount
        {
            get { return bezierCount; }
            set { bezierCount = value; }
        }

        // 直线条数，默认为3条
        private Int32 lineCount = 3;
        public Int32 LineCount
        {
            get { return lineCount; }
            set { lineCount = value; }
        }

        /// <summary>
        /// 随机字符串列表，请使用英文状态下的逗号分隔。
        /// </summary>
        private string charCollection = "2,3,4,5,6,7,8,9,a,s,value,fontFamily,graphics,h,z,color,v,bitmap,n,m,k,q,w,e,r,t,y,u,p,A,S,D,F,G,H,Z,C,V,B,N,M,K,Q,W,E,R,T,Y,U,P";
        public string CharCollection
        {
            get { return charCollection; }
            set { charCollection = value; }
        }

        // 验证码字符串个数，默认为4个字符
        private Int32 intCount = 4;
        public Int32 IntCount
        {
            get { return intCount; }
            set { intCount = value; }
        }

        // 是否添加噪点，默认添加，噪点颜色为系统随机生成
        private Boolean isPixel = true;
        public Boolean IsPixel
        {
            get { return isPixel; }
            set { isPixel = value; }
        }

        // 是否添加随机噪点字符串，默认添加
        private Boolean isRandString = true;
        public Boolean IsRandString
        {
            get { return isRandString; }
            set { isRandString = value; }
        }

        /// <summary>
        /// 随机背景字符串的个数
        /// </summary>
        public Int32 RandomStringCount
        {
            get;
            set;
        }

        // 随机背景字符串的大小
        private Int32 randomStringFontSize = 9;
        public Int32 RandomStringFontSize
        {
            get { return randomStringFontSize; }
            set { randomStringFontSize = value; }
        }

        /// <summary>
        /// 边框样式
        /// </summary>
        public enum BorderStyle
        {
            //无边框
            None,
            //矩形边框
            Rectangle,
            //圆角边框
            RoundRectangle
        }

        // 验证码字符串随机转动的角度的最大值
        private Int32 rotationAngle = 40;
        public Int32 RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }

        /// <summary>
        /// 设置或获取边框样式
        /// </summary>
        public BorderStyle Border
        {
            get;
            set;
        }

        private Point[] strPoint = null;

        // 对验证码图片进行高斯模糊的阀值，如果设置为0，则不对图片进行高斯模糊，该设置可能会对图片处理的性能有较大影响
        private Double gaussianDeviation = 0;
        public Double GaussianDeviation
        {
            get { return gaussianDeviation; }
            set { gaussianDeviation = value; }
        }

        // 对图片进行暗度和亮度的调整，如果该值为0，则不调整。该设置会对图片处理性能有较大影响
        private Int32 brightnessValue = 0;
        public Int32 BrightnessValue
        {
            get { return brightnessValue; }
            set { brightnessValue = value; }
        }

        /// <summary>
        /// 构造函数，用于初始化常用变量
        /// </summary>
        public void DrawValidationCode()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            strPoint = new Point[validationCodeCount + 1];
            if (gaussianDeviation < 0)
            {
                gaussianDeviation = 0;
            }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="target">用于存储图片的一般字节序列</param>
        public byte[] CreateImage(string code)
        {
            MemoryStream target = new MemoryStream();
            Bitmap bitmap = new Bitmap(codeWidth + 1, codeHeight + 1);
            //写字符串
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit; ;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            try
            {
                graphics.Clear(Color.White);
                DrawValidationCode();
                graphics.DrawImageUnscaled(DrawBackground(), 0, 0);
                graphics.DrawImageUnscaled(DrawRandomString(code), 0, 0);
                //对图片文字进行扭曲
                bitmap = AddRippleEffect(bitmap, 5);
                //对图片进行高斯模糊
                if (gaussianDeviation > 0)
                {
                    GaussFunction gauss = new GaussFunction();
                    bitmap = gauss.FilterProcessImage(gaussianDeviation, bitmap);
                }
                //进行暗度和亮度处理
                if (brightnessValue != 0)
                {
                    //对图片进行调暗处理
                    bitmap = AdjustBrightness(bitmap, brightnessValue);
                }
                bitmap.Save(target, ImageFormat.Jpeg);
                //输出图片流
                return target.ToArray();
            }
            finally
            {
                //brush.Dispose();
                bitmap.Dispose();
                graphics.Dispose();
            }
        }

        /// <summary>
        /// 画验证码背景，例如，增加早点，添加曲线和直线等
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawBackground()
        {
            Bitmap bitmap = new Bitmap(codeWidth + 1, codeHeight + 1);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //清除整个绘图面并使用指定颜色填充
            graphics.Clear(Color.White);
            Rectangle rectangle = new Rectangle(0, 0, codeWidth, codeHeight);
            Brush brush = new SolidBrush(backColor);
            graphics.FillRectangle(brush, rectangle);
            //画噪点
            if (isPixel)
            {
                graphics.DrawImageUnscaled(DrawRandomPixel(30), 0, 0);
            }
            graphics.DrawImageUnscaled(DrawRandBgString(), 0, 0);
            //画曲线
            //graphics.DrawImageUnscaled(DrawRandomBezier(bezierCount), 0, 0);
            //画直线
            //graphics.DrawImageUnscaled(DrawRandomLine(lineCount), 0, 0);
            //graphics.DrawImageUnscaled(DrawStringline(), 0, 0);
            //绘制边框
            if (Border == BorderStyle.Rectangle)
            {
                graphics.DrawRectangle(new Pen(Color.FromArgb(90, 87, 46)), 0, 0, codeWidth, codeHeight);
            }
            //画圆角
            else if (Border == BorderStyle.RoundRectangle)
            {
                DrawRoundRectangle(graphics, rectangle, Color.FromArgb(90, 87, 46), 1, 3);
            }
            return bitmap;
        }

        private Bitmap DrawTwist(Bitmap bmp, Int32 tWidth, Int32 tHeight, float angle, Color color)
        {
            //为了方便查看效果，在这里我定义了一个常量,它在定义数组的长度和for循环中都要用到
            int size = codeWidth;
            double[] x = new double[size];
            Bitmap bitmap = new Bitmap(bmp.Width, bmp.Height);
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen pen = new Pen(color);
            //画正弦曲线的横轴间距参数。建议所用的值应该是正数,且是2的倍数
            //在这里采用2。
            int val = 2;
            float temp = 0.0f;
            //把画布下移100!为什么要这样做，只要你把这一句给注释掉，运行一下代码，你就会明白是为什么？
            graphics.TranslateTransform(0, 100);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (int i = 0; i < size; i++)
            {
                //改变tWidth，实现正弦曲线宽度的变化。
                //改tHeight，实现正弦曲线高度的变化。
                x[i] = Math.Sin(2 * Math.PI * i / tWidth) * tHeight;
                graphics.DrawLine(pen, i * val, temp, i * val + val / 2, (float)x[i]);
                temp = (float)x[i];
            }
            graphics.RotateTransform(60, MatrixOrder.Prepend);
            //旋转图片
            // bitmap = KiRotate(bitmap, angle, Color.Transparent);
            return bitmap;
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            double PI2 = 6.283185307179586476925286766559;
            // 将位图背景填充为白色
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }

        /// <summary>
        /// 图片任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics graphics = Graphics.FromImage(tmp);
            graphics.Clear(bkColor);
            graphics.DrawImageUnscaled(bmp, 1, 1);
            graphics.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            graphics = Graphics.FromImage(dst);
            graphics.Clear(bkColor);
            graphics.TranslateTransform(-rct.X, -rct.Y);
            graphics.RotateTransform(angle);
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.DrawImageUnscaled(tmp, 0, 0);
            graphics.Dispose();
            tmp.Dispose();

            return dst;
        }

        /// <summary>
        /// 随机生成贝塞尔曲线
        /// </summary>
        /// <param name="bmp">一个图片的实例</param>
        /// <param name="lineNum">线条数量</param>
        /// <returns></returns>
        public Bitmap DrawRandomBezier(Int32 lineNum)
        {
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            GraphicsPath gPath1 = new GraphicsPath();
            Int32 lineRandNum = random.Next(lineNum);

            for (int i = 0; i < (lineNum - lineRandNum); i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point[] point = {
                                        new Point(random.Next(1, (bitmap.Width / 10)), random.Next(1, (bitmap.Height))),
                                        new Point(random.Next((bitmap.Width / 10) * 2, (bitmap.Width / 10) * 4), random.Next(1, (bitmap.Height))),
                                        new Point(random.Next((bitmap.Width / 10) * 4, (bitmap.Width / 10) * 6), random.Next(1, (bitmap.Height))),
                                        new Point(random.Next((bitmap.Width / 10) * 8, bitmap.Width), random.Next(1, (bitmap.Height)))
                                    };

                gPath1.AddBeziers(point);
                graphics.DrawPath(p, gPath1);
                p.Dispose();
            }
            for (int i = 0; i < lineRandNum; i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point[] point = {
                                new Point(random.Next(1, bitmap.Width), random.Next(1, bitmap.Height)),
                                new Point(random.Next((bitmap.Width / 10) * 2, bitmap.Width), random.Next(1, bitmap.Height)),
                                new Point(random.Next((bitmap.Width / 10) * 4, bitmap.Width), random.Next(1, bitmap.Height)),
                                new Point(random.Next(1, bitmap.Width), random.Next(1, bitmap.Height))
                                    };
                gPath1.AddBeziers(point);
                graphics.DrawPath(p, gPath1);
                p.Dispose();
            }
            return bitmap;
        }

        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="bmp">一个bmp实例</param>
        /// <param name="lineNum">线条个数</param>
        /// <returns></returns>
        public Bitmap DrawRandomLine(Int32 lineNum)
        {
            if (lineNum < 0) throw new ArgumentNullException("参数bmp为空！");
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            for (int i = 0; i < lineNum; i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point pt1 = new Point(random.Next(1, (bitmap.Width / 5) * 2), random.Next(bitmap.Height));
                Point pt2 = new Point(random.Next((bitmap.Width / 5) * 3, bitmap.Width), random.Next(bitmap.Height));
                graphics.DrawLine(p, pt1, pt2);
                p.Dispose();
            }

            return bitmap;
        }

        /// <summary>
        /// 画随机噪点
        /// </summary>
        /// <param name="pixNum">噪点的百分比</param>
        /// <returns></returns>
        public Bitmap DrawRandomPixel(Int32 pixNum)
        {
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            bitmap.MakeTransparent();
            Graphics graph = Graphics.FromImage(bitmap);
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //画噪点 
            for (int i = 0; i < (codeHeight * codeWidth) / pixNum; i++)
            {
                int x = random.Next(bitmap.Width);
                int y = random.Next(bitmap.Height);
                bitmap.SetPixel(x, y, GetRandomDeepColor());
                //下移坐标重新画点
                if ((x + 1) < bitmap.Width && (y + 1) < bitmap.Height)
                {
                    //画图片的前景噪音点
                    graph.DrawRectangle(new Pen(Color.Silver), random.Next(bitmap.Width), random.Next(bitmap.Height), 1, 1);
                }

            }

            return bitmap;
        }

        /// <summary>
        /// 画随机字符串中间连线
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawStringline()
        {
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Point[] p = new Point[validationCodeCount];
            for (int i = 0; i < validationCodeCount; i++)
            {
                p[i] = strPoint[i];
                //throw new Exception(strPoint.Length.ToString());
            }
            // graphics.DrawBezier(new Pen(GetRandomDeepColor()), strPoint);
            //graphics.DrawClosedCurve(new Pen(GetRandomDeepColor()), strPoint);
            graphics.DrawCurve(new Pen(GetRandomDeepColor(), 1), strPoint);

            return bitmap;
        }

        /// <summary>
        /// 写入验证码的字符串
        /// </summary>
        private Bitmap DrawRandomString(string code)
        {
            if (fontMaxSize >= (codeHeight / 5) * 4)
            {
                throw new ArgumentException("字体最大值参数FontMaxSize与验证码高度相近，这会导致描绘验证码字符串时出错，请重新设置参数！");
            }
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.Transparent);
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            charArray = code.ToCharArray();//拆散字符串成单字符数组
            validationCode = charArray.ToString();

            //设置字体显示格式
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            FontFamily fontFamily = new FontFamily(GenericFontFamilies.Monospace);

            Int32 charNum = charArray.Length;

            Point sPoint = new Point();
            Int32 fontSize = 12;
            for (int i = 0; i < validationCodeCount; i++)
            {
                int findex = random.Next(5);
                //定义字体
                Font textFont = new Font(fontFamily, random.Next(fontMinSize, fontMaxSize), FontStyle.Bold);
                //定义画刷，用于写字符串
                //Brush brush = new SolidBrush(GetRandomDeepColor());
                Int32 textFontSize = Convert.ToInt32(textFont.Size);
                fontSize = textFontSize;
                Point point = new Point(random.Next((codeWidth / charNum) * i + 5, (codeWidth / charNum) * (i + 1)), random.Next(codeHeight / 5 + textFontSize / 2, codeHeight - textFontSize / 2));
                //如果当前字符X坐标小于字体的二分之一大小
                if (point.X < textFontSize / 2)
                {
                    point.X = point.X + textFontSize / 2;
                }
                //防止文字叠加
                if (i > 0 && (point.X - sPoint.X < (textFontSize / 2 + textFontSize / 2)))
                {
                    point.X = point.X + textFontSize;
                }
                //如果当前字符X坐标大于图片宽度，就减去字体的宽度
                if (point.X > (codeWidth - textFontSize / 2))
                {
                    point.X = codeWidth - textFontSize / 2;
                }

                sPoint = point;

                float angle = random.Next(-rotationAngle, rotationAngle);//转动的度数
                graphics.TranslateTransform(point.X, point.Y);//移动光标到指定位置
                graphics.RotateTransform(angle);

                //设置渐变画刷  
                Rectangle myretang = new Rectangle(0, 1, Convert.ToInt32(textFont.Size), Convert.ToInt32(textFont.Size));
                Color color = GetRandomDeepColor();
                LinearGradientBrush mybrush2 = new LinearGradientBrush(myretang, color, GetLightColor(color, 120), random.Next(180));

                graphics.DrawString(charArray[i].ToString(), textFont, mybrush2, 1, 1, format);

                graphics.RotateTransform(-angle);//转回去
                graphics.TranslateTransform(-point.X, -point.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别

                strPoint[i] = point;

                textFont.Dispose();
                mybrush2.Dispose();
            }
            return bitmap;
        }

        /// <summary>
        /// 画背景干扰文字
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawRandBgString()
        {
            Bitmap bitmap = new Bitmap(codeWidth, codeHeight);
            string[] randStr = { "a", "bitmap", "color", "value", "e", "fontFamily", "graphics", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            bitmap.MakeTransparent();
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.Transparent);
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //设置字体显示格式
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            FontFamily fontFamily = new FontFamily(GenericFontFamilies.Serif);
            Font textFont = new Font(fontFamily, randomStringFontSize, FontStyle.Underline);

            int randAngle = 60; //随机转动角度

            for (int i = 0; i < RandomStringCount; i++)
            {
                Brush brush = new SolidBrush(GetRandomLightColor());
                Point pot = new Point(random.Next(5, codeWidth - 5), random.Next(5, codeHeight - 5));
                //随机转动的度数
                float angle = random.Next(-randAngle, randAngle);
                //转动画布
                graphics.RotateTransform(angle);
                graphics.DrawString(randStr[random.Next(randStr.Length)], textFont, brush, pot, format);
                //转回去，为下一个字符做准备
                graphics.RotateTransform(-angle);
                //释放资源
                brush.Dispose();
            }
            textFont.Dispose();
            format.Dispose();
            fontFamily.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成随机字符串    
        /// </summary>
        /// <returns></returns>
        public string GetRandomString(Int32 textLength)
        {
            string[] randomArray = charCollection.Split(',');
            int arrayLength = randomArray.Length;
            string randomString = "";
            for (int i = 0; i < textLength; i++)
            {
                randomString += randomArray[random.Next(0, arrayLength)];
            }
            return randomString; //长度是textLength +1
        }

        /// <summary>
        /// 绘制验证码背景
        /// </summary>
        /// <param name="hatchStyle"></param>
        private void DrawBackground(HatchStyle hatchStyle)
        {
            //设置填充背景时用的笔刷
            HatchBrush hBrush = new HatchBrush(hatchStyle, backColor);
            //填充背景图片
            graphics.FillRectangle(hBrush, 0, 0, this.codeWidth, this.codeHeight);
        }

        /// <summary>
        /// 根据指定长度，返回随机验证码
        /// </summary>
        /// <param >制定长度</param>
        /// <returns>随即验证码</returns>
        public string Next(int length)
        {
            this.validationCode = GetRandomCode(length);
            return this.validationCode;
        }

        /// <summary>
        /// 根据指定大小返回随机验证码
        /// </summary>
        /// <param >字符串长度</param>
        /// <returns>随机字符串</returns>
        private string GetRandomCode(int length)
        {
            StringBuilder stringBuilder = new StringBuilder(6);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(Char.ConvertFromUtf32(RandomAZ09()));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 产生0-9A-Z的随机字符代码
        /// </summary>
        /// <returns>字符代码</returns>
        private int RandomAZ09()
        {
            int result = 48;
            Random ram = new Random();
            switch (ram.Next(2))
            {
                case 0:
                    result = ram.Next(48, 58);
                    break;
                case 1:
                    result = ram.Next(65, 91);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 返回一个随机点，该随机点范围在验证码背景大小范围内
        /// </summary>
        /// <returns>Point对象</returns>
        private Point RandomPoint()
        {
            Random ram = new Random();
            Point point = new Point(ram.Next(this.codeWidth), ram.Next(this.codeHeight));
            return point;
        }

        /// <summary>
        /// 生成随机深颜色
        /// </summary>
        /// <returns></returns>
        public Color GetRandomDeepColor()
        {
            // nBlue,nRed相差大一点 nGreen小一些
            int nRed, nGreen, nBlue;
            int redLow = 160;
            int greenLow = 100;
            int blueLow = 160;
            nRed = random.Next(redLow);
            nGreen = random.Next(greenLow);
            nBlue = random.Next(blueLow);
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 生成随机浅颜色
        /// </summary>
        /// <returns>randomColor</returns>
        public Color GetRandomLightColor()
        {
            //越大颜色越浅
            int nRed, nGreen, nBlue;
            //色彩的下限
            int low = 180;
            //色彩的上限          
            int high = 255;
            nRed = random.Next(high) % (high - low) + low;
            nGreen = random.Next(high) % (high - low) + low;
            nBlue = random.Next(high) % (high - low) + low;
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 生成随机颜色值
        /// </summary>
        /// <returns></returns>
        public Color GetRandomColor()
        {
            //越大颜色越浅
            int nRed, nGreen, nBlue;
            //色彩的下限
            int low = 10;
            //色彩的上限
            int high = 255;
            nRed = random.Next(high) % (high - low) + low;
            nGreen = random.Next(high) % (high - low) + low;
            nBlue = random.Next(high) % (high - low) + low;
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 获取与当前颜色值相加后的颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color GetLightColor(Color color, Int32 value)
        {
            //越大颜色越浅
            int nRed = color.R, nGreen = color.G, nBlue = color.B;
            if (nRed + value < 255 && nRed + value > 0)
            {
                nRed = color.R + 40;
            }
            if (nGreen + value < 255 && nGreen + value > 0)
            {
                nGreen = color.G + 40;
            }
            if (nBlue + value < 255 && nBlue + value > 0)
            {
                nBlue = color.B + 40;
            }
            color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>       
        /// 合并图片        
        /// </summary>        
        /// <param name="maps"></param>        
        /// <returns></returns>        
        private Bitmap MergerImg(params Bitmap[] maps)
        {
            int i = maps.Length;
            if (i == 0)
            {
                throw new Exception("图片数不能够为0");
            }
            //创建要显示的图片对象,根据参数的个数设置宽度            
            Bitmap backgroudImg = new Bitmap(i * 12, 16);
            Graphics graphics = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色            
            graphics.Clear(Color.White);
            for (int j = 0; j < i; j++)
            {
                //graphics.DrawImage(maps[j], j * 11, 0, maps[j].Width, maps[j].Height);
                graphics.DrawImageUnscaled(maps[j], 0, 0);
            }
            graphics.Dispose();
            return backgroudImg;
        }

        /// <summary>
        /// 生成不重复的随机数，该函数会消耗大量系统资源
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="Mode">缩放质量</param>
        /// <returns>处理以后的图片</returns>
        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH, InterpolationMode Mode)
        {
            try
            {
                Bitmap bitmap = new Bitmap(newW, newH);
                Graphics graphics = Graphics.FromImage(bitmap);
                // 插值算法的质量
                graphics.InterpolationMode = Mode;
                graphics.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                graphics.Dispose();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// C# GDI+ 绘制圆角矩形
        /// </summary>
        /// <param name="graphics">Graphics 对象</param>
        /// <param name="rectangle">Rectangle 对象，圆角矩形区域</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="borderWidth">边框宽度</param>
        /// <param name="r">圆角半径</param>
        private static void DrawRoundRectangle(Graphics graphics, Rectangle rectangle, Color borderColor, float borderWidth, int radius)
        {
            // 如要使边缘平滑，请取消下行的注释
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            // 由于边框也需要一定宽度，需要对矩形进行修正
            //rectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            Pen pen = new Pen(borderColor, borderWidth);
            // 调用 getRoundRectangle 得到圆角矩形的路径，然后再进行绘制
            graphics.DrawPath(pen, getRoundRectangle(rectangle, radius));
        }

        /// <summary>
        /// 根据普通矩形得到圆角矩形的路径
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="radius">半径</param>
        /// <returns>图形路径</returns>
        private static GraphicsPath getRoundRectangle(Rectangle rectangle, int radius)
        {
            int length = 2 * radius;
            // 把圆角矩形分成八段直线、弧的组合，依次加到路径中
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new Point(rectangle.X + radius, rectangle.Y), new Point(rectangle.Right - radius, rectangle.Y));
            graphicsPath.AddArc(new Rectangle(rectangle.Right - length, rectangle.Y, length, length), 270F, 90F);

            graphicsPath.AddLine(new Point(rectangle.Right, rectangle.Y + radius), new Point(rectangle.Right, rectangle.Bottom - radius));
            graphicsPath.AddArc(new Rectangle(rectangle.Right - length, rectangle.Bottom - length, length, length), 0F, 90F);

            graphicsPath.AddLine(new Point(rectangle.Right - radius, rectangle.Bottom), new Point(rectangle.X + radius, rectangle.Bottom));
            graphicsPath.AddArc(new Rectangle(rectangle.X, rectangle.Bottom - length, length, length), 90F, 90F);

            graphicsPath.AddLine(new Point(rectangle.X, rectangle.Bottom - radius), new Point(rectangle.X, rectangle.Y + radius));
            graphicsPath.AddArc(new Rectangle(rectangle.X, rectangle.Y, length, length), 180F, 90F);
            return graphicsPath;
        }

        ///<summary>
        /// 柔化
        /// </summary>
        /// <param name="bitmap">原始图</param>
        /// <returns>输出图</returns>
        public static Bitmap KiBlur(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            int w = bitmap.Width;
            int h = bitmap.Height;
            try
            {
                Bitmap bmpRtn = new Bitmap(w, h, PixelFormat.Format24bppRgb);

                BitmapData srcData = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;
                    byte* p;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                int r1, r2, r3, r4, r5, r6, r7, r8, r9;
                                int g1, g2, g3, g4, g5, g6, g7, g8, g9;
                                int b1, b2, b3, b4, b5, b6, b7, b8, b9;

                                float vR, vG, vB;

                                //左上
                                p = pIn - stride - 3;
                                r1 = p[2];
                                g1 = p[1];
                                b1 = p[0];

                                //正上
                                p = pIn - stride;
                                r2 = p[2];
                                g2 = p[1];
                                b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                r3 = p[2];
                                g3 = p[1];
                                b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                r4 = p[2];
                                g4 = p[1];
                                b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];

                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                r8 = p[2];
                                g8 = p[1];
                                b8 = p[0];

                                //自己
                                p = pIn;
                                r9 = p[2];
                                g9 = p[1];
                                b9 = p[0];

                                vR = (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8 + r9);
                                vG = (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8 + g9);
                                vB = (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8 + b9);

                                vR /= 9;
                                vG /= 9;
                                vB /= 9;

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;
                            }
                            pIn += 3;
                            pOut += 3;
                        }
                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    }
                }
                bitmap.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);
                return bmpRtn;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 红色滤镜
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="threshold">阀值 -255~255</param>
        /// <returns></returns>
        public Bitmap JustAddRed(Bitmap bitmap, int threshold)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一个像素
                    var pixel = bitmap.GetPixel(x, y);
                    int red = pixel.R + threshold;
                    red = Math.Max(red, 0);
                    red = Math.Min(255, red);
                    // 只写入红色的值, G B 都放零
                    Color newColor = Color.FromArgb(pixel.A, red, 0, 0);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 绿色滤镜
        /// </summary>
        /// <param name="bitmap">一个图片实例</param>
        /// <param name="threshold">阀值 -255~+255</param>
        /// <returns></returns>
        public Bitmap JustAddGreen(Bitmap bitmap, int threshold)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一个像素
                    var pixel = bitmap.GetPixel(x, y);
                    //判断是否超过255
                    int green = pixel.G + threshold;
                    if (green > 255)
                    {
                        green = 255;
                    }
                    else if (green < 0)
                    {
                        green = 0;
                    }
                    // 只写入的绿色的值, R B 都放零
                    Color newColor = Color.FromArgb(pixel.A, 0, green, 0);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 蓝色滤镜
        /// </summary>
        /// <param name="bitmap">一个图片实例</param>
        /// <param name="threshold">阀值 -255~255</param>
        /// <returns></returns>
        public Bitmap JustAddBlue(Bitmap bitmap, int threshold)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一个像素
                    var pixel = bitmap.GetPixel(x, y);
                    //判断是否超过255
                    var blue = pixel.B + threshold;
                    if (blue > 255)
                    {
                        blue = 255;
                    }
                    else if (blue < 0)
                    {
                        blue = 0;
                    }
                    // 只写入蓝色的值, R G 都放零
                    Color newColor = Color.FromArgb(pixel.A, 0, 0, blue);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 调整 RGB 色调
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdRed">红色阀值</param>
        /// <param name="thresholdBlue">蓝色阀值</param>
        /// <param name="thresholdGreen">绿色阀值</param>
        /// <returns></returns>
        public Bitmap AdjustToCustomColor(Bitmap bitmap, int thresholdRed, int thresholdGreen, int thresholdBlue)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一个像素
                    var pixel = bitmap.GetPixel(x, y);
                    var green = pixel.G + thresholdGreen;
                    if (green > 255)
                    {
                        green = 255;
                    }
                    else if (green < 0)
                    {
                        green = 0;
                    }
                    var red = pixel.R + thresholdRed;
                    //如果小於0就為0
                    if (red > 255)
                    {
                        red = 255;
                    }
                    else if (red < 0)
                    {
                        red = 0;
                    }
                    var blue = pixel.B + thresholdBlue;
                    if (blue > 255)
                    {
                        blue = 255;
                    }
                    else if (blue < 0)
                    {
                        blue = 0;
                    }
                    // 只写入绿色的值, R B 都放零
                    Color newColor = Color.FromArgb(pixel.A, red, green, blue);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 图片去色（图片黑白化）
        /// </summary>
        /// <param name="original">一个需要处理的图片</param>
        /// <returns></returns>
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(newBitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                          {
                                 new float[] {.3f, .3f, .3f, 0, 0},
                                 new float[] {.59f, .59f, .59f, 0, 0},
                                 new float[] {.11f, .11f, .11f, 0, 0},
                                 new float[] {0, 0, 0, 1, 0},
                                 new float[] {0, 0, 0, 0, 1}
                          });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            graphics.Dispose();
            return newBitmap;
        }

        /// <summary>
        /// 增加或减少亮度
        /// </summary>
        /// <param name="img"></param>
        /// <param name="valBrightness">0~255</param>
        /// <returns></returns>
        public Bitmap AdjustBrightness(Image img, int valBrightness)
        {
            // 图片转为Bitmap
            Bitmap bitmap = new Bitmap(img);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一个像素
                    var pixel = bitmap.GetPixel(x, y);
                    var red = ((pixel.R + valBrightness > 255) ? 255 : pixel.R + valBrightness) < 0 ? 0 : ((pixel.R + valBrightness > 255) ? 255 : pixel.R + valBrightness);
                    var green = ((pixel.G + valBrightness > 255) ? 255 : pixel.G + valBrightness) < 0 ? 0 : ((pixel.G + valBrightness > 255) ? 255 : pixel.G + valBrightness);
                    var blue = ((pixel.B + valBrightness > 255) ? 255 : pixel.B + valBrightness) < 0 ? 0 : ((pixel.B + valBrightness > 255) ? 255 : pixel.B + valBrightness);
                    // 將改過的 RGB 寫回
                    Color newColor = Color.FromArgb(pixel.A, red, green, blue);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 浮雕效果
        /// </summary>
        /// <param name="src">一个图片实例</param>
        /// <returns></returns>
        public Bitmap AdjustToStone(Bitmap src)
        {
            // 依照 Format24bppRgb 每三个表示一 Pixel 0: 蓝 1: 绿 2: 红
            BitmapData bitmapData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                // 抓住第一个 Pixel 第一个数值
                byte* p = (byte*)(void*)bitmapData.Scan0;
                // 跨步值 - 宽度 *3 可以算出畸零地 之后跳到下一行
                int nOffset = bitmapData.Stride - src.Width * 3;
                for (int y = 0; y < src.Height; ++y)
                {
                    for (int x = 0; x < src.Width; ++x)
                    {
                        // 为了理解方便 所以特地在命名
                        int r, graphics, bitmap;
                        // 先取得下一个 Pixel
                        var q = p + 3;
                        r = Math.Abs(p[2] - q[2] + 128);
                        r = r < 0 ? 0 : r;
                        r = r > 255 ? 255 : r;
                        p[2] = (byte)r;

                        graphics = Math.Abs(p[1] - q[1] + 128);
                        graphics = graphics < 0 ? 0 : graphics;
                        graphics = graphics > 255 ? 255 : graphics;
                        p[1] = (byte)graphics;

                        bitmap = Math.Abs(p[0] - q[0] + 128);
                        bitmap = bitmap < 0 ? 0 : bitmap;
                        bitmap = bitmap > 255 ? 255 : bitmap;
                        p[0] = (byte)bitmap;

                        // 跳去下一个 Pixel
                        p += 3;

                    }
                    // 跨越畸零地
                    p += nOffset;
                }
            }
            src.UnlockBits(bitmapData);
            return src;
        }

        /// <summary>
        /// 水波纹效果
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="nWave">坡度</param>
        /// <returns></returns>
        public Bitmap AddRippleEffect(Bitmap bitmap, short nWave)
        {
            int nWidth = bitmap.Width;
            int nHeight = bitmap.Height;
            // 透过公式进行水波纹的採样
            PointF[,] fp = new PointF[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];
            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;
            double newX, newY;
            double xo, yo;
            //先取样将水波纹座标跟RGB取出
            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    xo = ((double)nWave * Math.Sin(2.0 * 3.1415 * (float)y / 128.0));
                    yo = ((double)nWave * Math.Cos(2.0 * 3.1415 * (float)x / 128.0));
                    newX = (x + xo);
                    newY = (y + yo);
                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = (float)newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = (float)0.0;
                        pt[x, y].X = 0;
                    }
                    if (newY > 0 && newY < nHeight)
                    {
                        fp[x, y].Y = (float)newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].Y = (float)0.0;
                        pt[x, y].Y = 0;
                    }
                }
            //进行合成
            Bitmap bSrc = (Bitmap)bitmap.Clone();
            // 依照 Format24bppRgb 每三个表示一 Pixel 0: 蓝 1: 绿 2: 红
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                                           PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite,
                                             PixelFormat.Format24bppRgb);
            int scanline = bitmapData.Stride;
            IntPtr Scan0 = bitmapData.Scan0;
            IntPtr SrcScan0 = bmSrc.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;
                int nOffset = bitmapData.Stride - bitmap.Width * 3;
                int xOffset, yOffset;
                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        xOffset = pt[x, y].X;
                        yOffset = pt[x, y].Y;

                        if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                        {
                            p[0] = pSrc[(yOffset * scanline) + (xOffset * 3)];
                            p[1] = pSrc[(yOffset * scanline) + (xOffset * 3) + 1];
                            p[2] = pSrc[(yOffset * scanline) + (xOffset * 3) + 2];
                        }
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bitmapData);
            bSrc.UnlockBits(bmSrc);
            return bitmap;
        }

        /// <summary>
        /// 调整曝光度值
        /// </summary>
        /// <param name="src">原图</param>
        /// <param name="r"></param>
        /// <param name="graphics"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Bitmap AdjustGamma(Bitmap src, double r, double graphics, double bitmap)
        {
            // 判断是不是在0.2~5 之间
            r = Math.Min(Math.Max(0.2, r), 5);
            graphics = Math.Min(Math.Max(0.2, graphics), 5);
            bitmap = Math.Min(Math.Max(0.2, bitmap), 5);
            // 依照 Format24bppRgb 每三个表示一 Pixel 0: 蓝 1: 绿 2: 红
            BitmapData bitmapData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                // 抓住第一个 Pixel 第一个数值
                byte* p = (byte*)(void*)bitmapData.Scan0;
                // 跨步值 - 宽度 *3 可以算出畸零地 之后跳到下一行
                int nOffset = bitmapData.Stride - src.Width * 3;
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < src.Width; x++)
                    {
                        p[2] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(p[2] / 255.0, 1.0 / r)) + 0.5));
                        p[1] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(p[1] / 255.0, 1.0 / graphics)) + 0.5));
                        p[0] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(p[0] / 255.0, 1.0 / bitmap)) + 0.5));
                        // 跳去下一个 Pixel
                        p += 3;
                    }
                    // 跨越畸零地
                    p += nOffset;
                }
            }
            src.UnlockBits(bitmapData);
            return src;
        }

        /// <summary>
        /// 高对比,对过深的颜色调浅，过浅的颜色调深。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="effectThreshold"> 高对比程度 -100~100</param>
        /// <returns></returns>
        public Bitmap Contrast(Bitmap src, float effectThreshold)
        {

            // 依照 Format24bppRgb 每三个表示一 Pixel 0: 蓝 1: 绿 2: 红
            BitmapData bitmapData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // 判断是否在 -100~100
            effectThreshold = effectThreshold < -100 ? -100 : effectThreshold;
            effectThreshold = effectThreshold > 100 ? 100 : effectThreshold;
            effectThreshold = (float)((100.0 + effectThreshold) / 100.0);
            effectThreshold *= effectThreshold;
            unsafe
            {
                // 抓住第一个 Pixel 第一个数值 www.it165.net
                byte* p = (byte*)(void*)bitmapData.Scan0;
                // 跨步值 - 宽度 *3 可以算出畸零地 之后跳到下一行
                int nOffset = bitmapData.Stride - src.Width * 3;
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < src.Width; x++)
                    {
                        double buffer = 0;
                        // 公式  (Red/255)-0.5= 偏离中间值程度
                        // ((偏离中间值程度 * 影响范围)+0.4 ) * 255
                        buffer = ((((p[2] / 255.0) - 0.5) * effectThreshold) + 0.5) * 255.0;
                        buffer = buffer > 255 ? 255 : buffer;
                        buffer = buffer < 0 ? 0 : buffer;
                        p[2] = (byte)buffer;

                        buffer = ((((p[1] / 255.0) - 0.5) * effectThreshold) + 0.5) * 255.0;
                        buffer = buffer > 255 ? 255 : buffer;
                        buffer = buffer < 0 ? 0 : buffer;
                        p[1] = (byte)buffer;

                        buffer = ((((p[0] / 255.0) - 0.5) * effectThreshold) + 0.5) * 255.0;
                        buffer = buffer > 255 ? 255 : buffer;
                        buffer = buffer < 0 ? 0 : buffer;
                        p[0] = (byte)buffer;
                        // 下一个像素
                        p += 3;
                    }
                    // 跨越畸零地
                    p += nOffset;
                }
            }
            src.UnlockBits(bitmapData);
            return src;
        }

        /// <summary>
        /// 对图片进行雾化效果
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public Bitmap Atomization(Bitmap bmp)
        {
            int Height = bmp.Height;
            int Width = bmp.Width;
            Bitmap newBitmap = new Bitmap(Width, Height);
            Bitmap oldBitmap = bmp;
            Color pixel;
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    Random MyRandom = new Random(Guid.NewGuid().GetHashCode());
                    int k = MyRandom.Next(123456);
                    //像素块大小
                    int dx = x + k % 19;
                    int dy = y + k % 19;
                    if (dx >= Width)
                    {
                        dx = Width - 1;
                    }
                    if (dy >= Height)
                    {
                        dy = Height - 1;
                    }
                    pixel = oldBitmap.GetPixel(dx, dy);
                    newBitmap.SetPixel(x, y, pixel);
                }
            }
            return newBitmap;
        }
    }

    /// <summary>
    /// 高斯模糊算法
    /// </summary>
    public class GaussFunction
    {
        public static double[,] Calculate1DSampleKernel(double deviation, int size)
        {
            double[,] ret = new double[size, 1];
            double sum = 0;
            int half = size / 2;
            for (int i = 0; i < size; i++)
            {
                ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) * Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
                sum += ret[i, 0];
            }
            return ret;
        }

        public static double[,] Calculate1DSampleKernel(double deviation)
        {
            int size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
            return Calculate1DSampleKernel(deviation, size);
        }

        public static double[,] CalculateNormalized1DSampleKernel(double deviation)
        {
            return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        }

        public static double[,] NormalizeMatrix(double[,] matrix)
        {
            double[,] ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double sum = 0;
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                    sum += matrix[i, j];
            }
            if (sum != 0)
            {
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = matrix[i, j] / sum;
                }
            }
            return ret;
        }

        public static double[,] GaussianConvolution(double[,] matrix, double deviation)
        {
            double[,] kernel = CalculateNormalized1DSampleKernel(deviation);
            double[,] res1 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double[,] res2 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res1[i, j] = processPoint(matrix, i, j, kernel, 0);
            }
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res2[i, j] = processPoint(res1, i, j, kernel, 1);
            }
            return res2;
        }

        private static double processPoint(double[,] matrix, int x, int y, double[,] kernel, int direction)
        {
            double res = 0;
            int half = kernel.GetLength(0) / 2;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                int cox = direction == 0 ? x + i - half : x;
                int coy = direction == 1 ? y + i - half : y;
                if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
                {
                    res += matrix[cox, coy] * kernel[i, 0];
                }
            }
            return res;
        }

        /// <summary>
        /// 对颜色值进行灰色处理
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        private Color grayscale(Color cr)
        {
            return Color.FromArgb(cr.A, (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
               (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
              (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11));
        }

        /// <summary>
        /// 对图片进行高斯模糊
        /// </summary>
        /// <param name="value">模糊数值，数值越大模糊越很</param>
        /// <param name="image">一个需要处理的图片</param>
        /// <returns></returns>
        public Bitmap FilterProcessImage(double value, Bitmap image)
        {
            Bitmap ret = new Bitmap(image.Width, image.Height);
            Double[,] matrixR = new Double[image.Width, image.Height];
            Double[,] matrixG = new Double[image.Width, image.Height];
            Double[,] matrixB = new Double[image.Width, image.Height];
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    matrixR[i, j] = image.GetPixel(i, j).R;
                    matrixG[i, j] = image.GetPixel(i, j).G;
                    matrixB[i, j] = image.GetPixel(i, j).B;
                }
            }
            matrixR = GaussFunction.GaussianConvolution(matrixR, value);
            matrixG = GaussFunction.GaussianConvolution(matrixG, value);
            matrixB = GaussFunction.GaussianConvolution(matrixB, value);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Int32 R = (int)Math.Min(255, matrixR[i, j]);
                    Int32 G = (int)Math.Min(255, matrixG[i, j]);
                    Int32 B = (int)Math.Min(255, matrixB[i, j]);
                    ret.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            return ret;
        }
    }

}