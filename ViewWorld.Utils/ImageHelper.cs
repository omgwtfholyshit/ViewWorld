using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ViewWorld.Utils
{
    public class ImageHelper
    {
        /// <summary>
        /// 按照定义的宽度和高度截取图片
        /// </summary>
        /// <param name="originalPhotoPath">原图物理路径</param>
        /// <param name="thumbnailPath">新图物理路径</param>
        /// <param name="width">新图高度</param>
        /// <param name="height">新图宽度</param>
        public static void MakeThumbnail(string originalPhotoPath, string thumbnailPath, int width, int height)
        {
            if (File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }
            //获取原始图片
            Image currentImg = Image.FromFile(originalPhotoPath);
            //计算原始图片比例
            double divisor = (double)currentImg.Width / (double)currentImg.Height;
            //计算新图片比例
            double percent = (double)width / (double)height;
            //初始化图形变量
            int newWidth = 0;
            int newHeight = 0;
            int x = 0;
            int y = 0;
            //如果是长图
            if (divisor >= percent)
            {
                newWidth = height * currentImg.Width / currentImg.Height;
                newHeight = height;
                x = (newWidth - width) / 2;
            }
            else
            {
                newWidth = width;
                newHeight = width * currentImg.Height / currentImg.Width;
                y = (newHeight - height) / 2;
            }
            //新建TempImg用于转换图片大小
            Image temp = new Bitmap(newWidth, newHeight);
            Graphics tempG = Graphics.FromImage(temp);
            tempG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            tempG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            tempG.Clear(Color.Transparent);
            //以不变形的比例创建一张图片(宽 = 需要的宽度 || 高 = 需要的高度)
            tempG.DrawImage(currentImg, new Rectangle(0, 0, newWidth, newHeight));

            //新建一个bmp图片 
            Image bitmap = new Bitmap(width, height);
            //新建一个画板 
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(temp, new Rectangle(0, 0, temp.Width - x, temp.Height - y),
              new Rectangle(x, y, temp.Width - x, temp.Height - y),
              GraphicsUnit.Pixel);

            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            try
            {
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                Tools.WriteLog("切图", originalPhotoPath, e.Message);
            }
            finally
            {
                currentImg.Dispose();
                bitmap.Dispose();
                g.Dispose();
                temp.Dispose();
                tempG.Dispose();
            }

        }
    }
}
