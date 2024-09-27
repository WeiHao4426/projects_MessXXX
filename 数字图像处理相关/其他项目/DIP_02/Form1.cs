using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Numerics;


namespace DIP_02
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string curFileName;
        //图像对象
        private System.Drawing.Bitmap curBitmpap;
        private void open_Click(object sender, EventArgs e)//打开文件
        {
            //创建OpenFileDialog
            OpenFileDialog opnDlg = new OpenFileDialog();
            //为图像选择一个筛选器
            opnDlg.Filter = "所有图像文件|*.bmp;*.pcx;*.png;*.jpg;*.gif;" +
                "*.tif;*.ico;*.dxf;*.cgm;*.cdr;*.wmf;*.eps;*.emf|" +
                "位图(*.bmp;*.jpg;*.png;...)|*.bmp;*.pcx;*.png;*.jpg;*.gif;*.tif;*.ico|" +
                "矢量图(*.wmf;*.eps;*.emf;...)|*.dxf;*.cgm;*.cdr;*.wmf;*.eps;*.emf";
            //设置对话框标题
            opnDlg.Title = "打开图像文件";
            //启用“帮助”按钮
            opnDlg.ShowHelp = true;

            //如果结果为“打开”，选定文件
            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                //读取当前选中的文件名
                curFileName = opnDlg.FileName;
                //使用Image.FromFile创建图像对象
                try
                {
                    curBitmpap = (Bitmap)Image.FromFile(curFileName);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
            //对窗体进行重新绘制，这将强制执行paint事件处理程序
            Invalidate();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //获取Graphics对象
            Graphics g = e.Graphics;
            if (curBitmpap != null)
            {
                //使用DrawImage方法绘制图像
                //160,20：显示在主窗体内，图像左上角的坐标
                //curBitmpap.Width, curBitmpap.Height图像的宽度和高度
                g.DrawImage(curBitmpap, 160, 20, curBitmpap.Width, curBitmpap.Height);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            //如果没有创建图像，则退出
            if (curBitmpap == null)
                return;

            //调用SaveFileDialog
            SaveFileDialog saveDlg = new SaveFileDialog();
            //设置对话框标题
            saveDlg.Title = "保存为";
            //改写已存在文件时提示用户
            saveDlg.OverwritePrompt = true;
            //为图像选择一个筛选器
            saveDlg.Filter = "BMP文件(*.bmp)|*.bmp|" + "Gif文件(*.gif)|*.gif|" + "JPEG文件(*.jpg)|*.jpg|" + "PNG文件(*.png)|*.png";
            //启用“帮助”按钮
            saveDlg.ShowHelp = true;

            //如果选择了格式，则保存图像
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                //获取用户选择的文件名
                string filename = saveDlg.FileName;
                string strFilExtn = filename.Remove(0, filename.Length - 3);

                //保存文件
                switch (strFilExtn)
                {
                    //以指定格式保存
                    case "bmp":
                        curBitmpap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        curBitmpap.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        curBitmpap.Save(filename, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        curBitmpap.Save(filename, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        curBitmpap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }

        public class Complex
        {
            ///实部
            public double re  = 0;
            ///虚部
            public double im  = 0;
            //加法
            public static Complex operator +(Complex a, Complex b)
            {
                return new Complex { re = a.re + b.re, im = a.im + b.im };
            }
            //减法
            public static Complex operator -(Complex a, Complex b)
            {
                return new Complex { re = a.re - b.re, im = a.im - b.im };
            }
            //乘法
            public static Complex operator *(Complex a, Complex b)
            {
                return new Complex
                {
                    re = a.re * b.re - a.im * b.im,
                    im = a.im * b.re + a.re * b.im
                };
            }
            public override string ToString()
            {
                if (im > 0)
                {
                    return re + "+" + im + "i";
                }
                if (im == 0)
                {
                    return re.ToString();
                }
                if (im < 0)
                {
                    return re.ToString() + im.ToString() + "i";
                }
                return null;
            }
        }
        
        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法
        /// </summary>
        /// <param name="srcBmp">原始图像</param>
        /// <param name="dstBmp">处理后图像</param>
        /// <returns>处理成功 true 失败 false</returns>
        public static bool Balance(Bitmap srcBmp, out Bitmap dstBmp)
        {
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            int[] histogramArrayR = new int[256];//各个灰度级的像素数R
            int[] histogramArrayG = new int[256];//各个灰度级的像素数G
            int[] histogramArrayB = new int[256];//各个灰度级的像素数B
            int[] tempArrayR = new int[256];
            int[] tempArrayG = new int[256];
            int[] tempArrayB = new int[256];
            byte[] pixelMapR = new byte[256];
            byte[] pixelMapG = new byte[256];
            byte[] pixelMapB = new byte[256];
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                //统计各个灰度级的像素个数
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        histogramArrayB[*(ptr + j * 3)]++;
                        histogramArrayG[*(ptr + j * 3 + 1)]++;
                        histogramArrayR[*(ptr + j * 3 + 2)]++;
                    }
                }
                //计算各个灰度级的累计分布函数
                for (int i = 0; i < 256; i++)
                {
                    if (i != 0)
                    {
                        tempArrayB[i] = tempArrayB[i - 1] + histogramArrayB[i];
                        tempArrayG[i] = tempArrayG[i - 1] + histogramArrayG[i];
                        tempArrayR[i] = tempArrayR[i - 1] + histogramArrayR[i];
                    }
                    else
                    {
                        tempArrayB[0] = histogramArrayB[0];
                        tempArrayG[0] = histogramArrayG[0];
                        tempArrayR[0] = histogramArrayR[0];
                    }
                    //计算累计概率函数，并将值放缩至0~255范围内
                    pixelMapB[i] = (byte)(255.0 * tempArrayB[i] / (bmpData.Width * bmpData.Height) + 0.5);//加0.5为了四舍五入取整
                    pixelMapG[i] = (byte)(255.0 * tempArrayG[i] / (bmpData.Width * bmpData.Height) + 0.5);
                    pixelMapR[i] = (byte)(255.0 * tempArrayR[i] / (bmpData.Width * bmpData.Height) + 0.5);
                }
                //映射转换
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(ptr + j * 3) = pixelMapB[*(ptr + j * 3)];
                        *(ptr + j * 3 + 1) = pixelMapG[*(ptr + j * 3 + 1)];
                        *(ptr + j * 3 + 2) = pixelMapR[*(ptr + j * 3 + 2)];
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap dstBmp;
            Balance(curBitmpap, out dstBmp);
            curBitmpap = dstBmp;
            Invalidate();
        }

        private void histogram_Click(object sender, EventArgs e)
        {
                histogram winform = new histogram(curBitmpap);
                winform.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)//高斯滤波
        {
            curBitmpap = Smooth(curBitmpap);
            Invalidate();
        }

        private double[,] GaussianBlur;//声明私有的高斯模糊卷积核函数
        /// 构造卷积（Convolution）类函数
        public void Convolution()
        {
            //初始化高斯模糊卷积核
            int k = 273;
            GaussianBlur = new double[5, 5]{{(double)1/k,(double)4/k,(double)7/k,(double)4/k,(double)1/k},
                                            {(double)4/k,(double)16/k,(double)26/k,(double)16/k,(double)4/k},
                                            {(double)7/k,(double)26/k,(double)41/k,(double)26/k,(double)7/k},
                                            {(double)4/k,(double)16/k,(double)26/k,(double)16/k,(double)4/k},
                                            {(double)1/k,(double)4/k,(double)7/k,(double)4/k,(double)1/k}};
        }
        /// <summary>
        /// 对图像进行平滑处理（利用高斯平滑Gaussian Blur）
        /// </summary>
        /// <param name="bitmap">要处理的位图</param>
        /// <returns>返回平滑处理后的位图</returns>
        public Bitmap Smooth(Bitmap bitmap)
        {

            Convolution();//调用卷积函数

            int[,,] InputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输入的位图的数组

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    InputPicture[0, i, j] = color.R;
                    InputPicture[1, i, j] = color.G;
                    InputPicture[2, i, j] = color.B;
                }
            }

            int[,,] OutputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输出的位图的数组
            Bitmap smooth = new Bitmap(bitmap.Width, bitmap.Height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    //每一个像素计算使用高斯模糊卷积核进行计算
                    for (int r = 0; r < 5; r++)//循环卷积核的每一行
                    {
                        for (int f = 0; f < 5; f++)//循环卷积核的每一列
                        {
                            //控制与卷积核相乘的元素
                            int row = i - 2 + r;
                            int index = j - 2 + f;

                            //当超出位图的大小范围时，选择最边缘的像素值作为该点的像素值
                            row = row < 0 ? 0 : row;
                            index = index < 0 ? 0 : index;
                            row = row >= bitmap.Width ? bitmap.Width - 1 : row;
                            index = index >= bitmap.Height ? bitmap.Height - 1 : index;

                            //输出得到像素的RGB值
                            Console.WriteLine("Gauss: " + GaussianBlur[r, f]);
                            R += (int)(GaussianBlur[r, f] * InputPicture[0, row, index]);
                            G += (int)(GaussianBlur[r, f] * InputPicture[1, row, index]);
                            B += (int)(GaussianBlur[r, f] * InputPicture[2, row, index]);
                        }
                    }
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(i, j, color);//位图存储该点像素值
                }
            }
            return smooth;
        }

        private void lowpass_Click(object sender, EventArgs e)//低通滤波
        {
            curBitmpap = LowPass(curBitmpap);
            Invalidate();
        }
        public Bitmap LowPass(Bitmap bitmap)
        {
            return bitmap;
        }

        // 二维傅立叶变换
        //  快速傅立叶变换
        public static byte[] ChangeByte(Bitmap bitmap)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Dispose();
            return bytes;
        }
        public static Bitmap ChangeBitmap(byte[] ImageByte)
        {
            Bitmap bitmap = null; using (MemoryStream stream = new MemoryStream(ImageByte))
            {
                bitmap = new Bitmap((Image)new Bitmap(stream));
            }
            return bitmap;
        }
        public static void FFT(Complex[] t, Complex[] f, int r)   // t为时域，f为频域 r为2的幂数
        {
            long count;
            int i, j, k, p, bsize;
            Complex[] W;
            Complex[] X1;
            Complex[] X2;
            Complex[] X;
            Complex comp;  // 用于暂存中间值
            double angle;  // 计算加权时所需角度
            count = 1 << r;

            comp = new Complex();

            W = new Complex[count / 2];
            X1 = new Complex[count];
            X2 = new Complex[count];
            X = new Complex[count];
            for (i = 0; i < count / 2; i++)
            {
                angle = i * Math.PI * 2 / count;
                W[i].re = (double)Math.Cos(angle);
                W[i].im = -(double)Math.Sin(angle);
            }

            t.CopyTo(X1, 0);

            for (k = 0; k < r; k++)
            {
                for (j = 0; j < 1 << k; j++)
                {
                    bsize = 1 << (r - k);
                    for (i = 0; i < bsize / 2; i++)
                    {
                        p = j * bsize;
                        X2[i + p].im = X1[i + p].im + X1[i + p + bsize / 2].im;
                        X2[i + p].re = X1[i + p].re + X1[i + p + bsize / 2].re;

                        comp.im = X1[i + p].im - X1[i + p + bsize / 2].im;
                        comp.re = X1[i + p].re - X1[i + p + bsize / 2].re;

                        X2[i + p + bsize / 2].re = comp.re * W[i * (1 << k)].re -
                            comp.im * W[i * (1 << k)].im;
                        X2[i + p + bsize / 2].im = comp.re * W[i * (1 << k)].im +
                         comp.im * W[i * (1 << k)].re; ;
                    }
                }
                X = X1;
                X1 = X2;
                X2 = X;
            }

            for (j = 0; j < count; j++)
            {
                p = 0;
                for (i = 0; i < r; i++)
                {
                    if ((j & (1 << i)) != 0)
                    {
                        p += 1 << (r - i - 1);
                    }
                }
                f[j].re = X1[p].re;
                f[j].im = X1[p].im;
            }
        }
        public static Bitmap Fourier(Bitmap tp)
        {
            // 原图像的宽与高
            int w = tp.Width;
            int h = tp.Height;
            // 傅立叶变换的实际宽高
            long lw = 1;
            long lh = 1;
            // 迭代次数
            int wp = 0; int hp = 0;
            long i, j;
            long n, m;
            double temp;
            byte[] ky = new byte[w * h];
            ky = ChangeByte(tp);
            Complex[] t; Complex[] f;

            while (lw * 2 <= w)
            {
                lw *= 2;
                wp++;
            }
            while (lh * 2 <= h)
            {
                lh *= 2;
                hp++;
            }
            t = new Complex[lw * lh];
            f = new Complex[lw * lh];
            Complex[] tw = new Complex[lw];
            Complex[] th = new Complex[lw];
            for (i = 0; i < lh; i++)
            {
                for (j = 0; j < lw; j++)
                {
                    Console.WriteLine(i * w + j);
                    Console.WriteLine(t[i * lw + j].re);
                    t[i * lw + j].re = ky[i * w + j];
                    t[i * lw + j].im = 0;
                }
            }
            for (i = 0; i < lh; i++) // 垂直方向傅立叶变换
            {

                Array.Copy(t, i * lw, tw, 0, lw);
                Array.Copy(f, i * lw, th, 0, lw);
                FFT(tw, th, wp);
                // Array.Copy(tw, 0, t, i * lw, lw);
                Array.Copy(th, 0, f, i * lw, lw);
            }

            for (i = 0; i < lh; i++)
            {
                for (j = 0; j < lw; j++)
                {
                    t[j * lh + i].re = f[i * lw + j].re;
                    t[j * lh + i].im = f[i * lw + j].im;
                }
            }

            Complex[] ow = new Complex[lh];
            Complex[] oh = new Complex[lh];
            for (i = 0; i < lw; i++)
            {
                Array.Copy(t, i * lh, ow, 0, lh);
                Array.Copy(f, i * lh, oh, 0, lh);
                FFT(ow, oh, hp);
                //Array.Copy(ow, 0, t, i * lh, lh);
                oh.CopyTo(f, i * lh);
            }

            for (i = 0; i < lh; i++)
            {
                for (j = 0; j < lw; j++)
                {
                    temp = Math.Sqrt(f[j * lh + i].re * f[j * lh + i].re + f[j * lh + i].im * f[j * lh + i].im) / 100;
                    if (temp > 255)
                    {
                        temp = 255;
                    }
                    n = i < lh / 2 ? i + lh / 2 : i - lh / 2;
                    m = j < lw / 2 ? j + lw / 2 : j - lw / 2;
                    ky[n * w + m] = (byte)(temp);
                }
            }

            tp = ChangeBitmap(ky);
            return tp;

        }

        private void FTT_Click(object sender, EventArgs e)
        {
            curBitmpap = Fourier(curBitmpap);
            Invalidate();
        }
    }
}
