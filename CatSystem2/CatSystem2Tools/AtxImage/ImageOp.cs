using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace AtxImage
{
    public static class ImageOp
    {

        /// <summary>
        /// 截取获得org图片中，rect内部的子图片
        /// </summary>
        /// <param name="org"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Bitmap GetSubBitmap(Bitmap org, Rectangle rect)
        {
            Bitmap rtn = new Bitmap(rect.Width, rect.Height, org.PixelFormat);
            Graphics g = Graphics.FromImage(rtn);
            g.DrawImage(org, 0, 0, rect, GraphicsUnit.Pixel);
            g.Dispose();
            return rtn;
        }


        /// <summary>
        /// 在back图上，以(x,y)为起始位置，把fore图片写入
        /// </summary>
        /// <param name="back"></param>
        /// <param name="fore"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static bool WriteBitmapAtTargetPosition(Bitmap back, Bitmap fore, int x, int y)
        {
            if(x + fore.Width > back.Width || y + fore.Height > back.Height)
            {
                return false;
            }
            Rectangle rect = new Rectangle(x, y, fore.Width, fore.Height);
            using(Graphics g = Graphics.FromImage(back))
            {
                g.DrawImage(fore, x, y);
            }
            return true;
        }
    }
}
