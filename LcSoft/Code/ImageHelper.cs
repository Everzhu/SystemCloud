using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class ImageHelper
    {
        /// <summary> 
        /// 为图片生成缩略图
        /// </summary> 
        /// <param name="phyPath">原图片的路径</param> 
        /// <param name="width">缩略图宽</param> 
        /// <param name="height">缩略图高</param> 
        /// <returns></returns> 
        public System.Drawing.Image GetHvtThumbnail(System.Drawing.Image image, int width, int height)
        {
            var m_hovertreeBmp = new System.Drawing.Bitmap(width, height);
            //从Bitmap创建一个System.Drawing.Graphics 
            var m_HvtGr = System.Drawing.Graphics.FromImage(m_hovertreeBmp);
            //设置  
            m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //下面这个也设成高质量 
            m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //下面这个设成High 
            m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //把原始图像绘制成上面所设置宽高的缩小图 
            var rectDestination = new System.Drawing.Rectangle(0, 0, width, height);

            int m_width, m_height;
            if (image.Width * height > image.Height * width)
            {
                m_height = image.Height;
                m_width = (image.Height * width) / height;
            }
            else
            {
                m_width = image.Width;
                m_height = (image.Width * height) / width;
            }

            m_HvtGr.DrawImage(image, rectDestination, 0, 0, m_width, m_height, System.Drawing.GraphicsUnit.Pixel);

            return m_hovertreeBmp;
        }
    }
}