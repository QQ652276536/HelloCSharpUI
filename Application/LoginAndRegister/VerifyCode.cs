using System;
using System.Drawing;
using System.Security.Cryptography;

namespace HelloCSharp
{
    /// <summary>
    /// 验证码类
    /// </summary>
    public class VerifyCode
    {
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Number(int length, bool isSleep)
        {
            if (isSleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            string result = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string StrAndNumber(int length, bool isSleep)
        {
            if (isSleep)
            {
                System.Threading.Thread.Sleep(500);
            }
            char[] charArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int arrayLength = charArray.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int tempIndex = random.Next(0, arrayLength);
                result += charArray[tempIndex];
            }
            return result;
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Letters(int length, bool isSleep)
        {
            if (isSleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            char[] charArray = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int arrayLength = charArray.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < arrayLength; i++)
            {
                int tempIndex = random.Next(0, arrayLength);
                result += charArray[tempIndex];
            }
            return result;
        }
    }

    /// <summary>
    /// 验证图片类
    /// </summary>
    public class VerifyImage
    {
        private string _text;
        private Bitmap _image;
        //单个字体的宽度范围
        private int _letterWidth = 16;
        //单个字体的高度范围
        private int _letterHeight = 29;
        //用于接收经过加密的强随机值
        private static byte[] _rngArray = new byte[4];
        //加密服务提供程序
        private static RNGCryptoServiceProvider _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private Font[] _fonts =
    {
       new Font(new FontFamily("Times New Roman"),10 + GetNextRandom(1),FontStyle.Regular),
       new Font(new FontFamily("Georgia"), 10 + GetNextRandom(1),FontStyle.Regular),
       new Font(new FontFamily("Arial"), 10 + GetNextRandom(1),FontStyle.Regular),
       new Font(new FontFamily("Comic Sans MS"), 10 + GetNextRandom(1),FontStyle.Regular)
    };
        //图片验证码的大小
        private Size _imageSize;

        public VerifyImage()
        {
            this._text = VerifyCode.StrAndNumber(4, false);
        }

        public Size ImageSize
        {
            get
            {
                return _imageSize;
            }
            set
            {
                _imageSize = value;
            }
        }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
        }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
            }
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        private static int GetNextRandom(int max)
        {
            //用经过强加密的随机值序列填充字节数组
            _rngCryptoServiceProvider.GetBytes(_rngArray);
            //返回从字节数组中指定位置的四个字节组成的32位有符号整数
            int value = BitConverter.ToInt32(_rngArray, 0);
            value = value % (max + 1);
            //如果是负数
            if (value < 0)
            {
                value = -value;
            }
            return value;
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private static int GetNextRandom(int min, int max)
        {
            int value = GetNextRandom(max - min) + min;
            return value;
        }

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        public Color GetRandomColor()
        {
            Random random1 = new Random(~unchecked((int)DateTime.Now.Ticks));
            System.Threading.Thread.Sleep(random1.Next(50));
            Random random2 = new Random((int)DateTime.Now.Ticks);
            int red = random1.Next(180);
            int green = random2.Next(180);
            int blue = (red + green > 300) ? 0 : 400 - red - green;
            blue = (blue > 255) ? 255 : blue;
            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// 绘制验证码
        /// </summary>
        public void CreateImage()
        {
            int imageWidth = this._text.Length * _letterWidth;
            Bitmap image = new Bitmap(_imageSize.Width, _imageSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            for (int i = 0; i < 2; i++)
            {
                int x1 = GetNextRandom(image.Width - 1);
                int x2 = GetNextRandom(image.Width - 1);
                int y1 = GetNextRandom(image.Height - 1);
                int y2 = GetNextRandom(image.Height - 1);
                graphics.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }
            //绘制内容的坐标
            int x = -12, y = 12;
            for (int int_index = 0; int_index < this._text.Length; int_index++)
            {
                x += GetNextRandom(12, 16);
                y = GetNextRandom(5, 2);
                string str_char = this._text.Substring(int_index, 1);
                str_char = GetNextRandom(1) == 1 ? str_char.ToLower() : str_char.ToUpper();
                Brush newBrush = new SolidBrush(GetRandomColor());
                Point thePos = new Point(x, y);
                graphics.DrawString(str_char, _fonts[GetNextRandom(_fonts.Length - 1)], newBrush, thePos);
            }
            for (int i = 0; i < 10; i++)
            {
                int tmepX = GetNextRandom(image.Width - 1);
                int tempY = GetNextRandom(image.Height - 1);
                image.SetPixel(tmepX, tempY, Color.FromArgb(GetNextRandom(0, 255), GetNextRandom(0, 255), GetNextRandom(0, 255)));
            }
            image = TwistImage(image, true, GetNextRandom(2, 5), GetNextRandom(4, 6));
            graphics.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, imageWidth - 1, (_letterHeight - 1));
            this._image = image;
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            double PI = 6.283185307179586476925286766559;
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
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
            srcBmp.Dispose();
            return destBmp;
        }
    }

}