using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing.Drawing2D;

namespace ImageUtilities
{
    static class VariablePool
    {
        public static Bitmap CurrentBitmap;
        public static Bitmap CurrentBitmap_DownSized;
        public static Bitmap CurrentBitmap_Original;
        public static string CurrentFile;
        public static Image OriginalImage;
    }
    static class Utilities
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
        public static Bitmap DownSize10X(Bitmap Original)
        {
            int width = Original.Width / 10;
            int height = Original.Height/ 10;
            var bmp = new Bitmap(Original.Width/10, Original.Height/10);
            var graph = Graphics.FromImage(bmp);

            var brush = new SolidBrush(System.Drawing.Color.Transparent);

            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(Original.Width * 0.1);
            var scaleHeight = (int)(Original.Height * 0.1);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(Original, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
            return bmp;
        }
        public static Bitmap DownSize5X(Bitmap Original)
        {
            int width = Original.Width / 5;
            int height = Original.Height/ 5;
            var bmp = new Bitmap(Original.Width/5, Original.Height/5);
            var graph = Graphics.FromImage(bmp);

            var brush = new SolidBrush(System.Drawing.Color.Transparent);

            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(Original.Width /5);
            var scaleHeight = (int)(Original.Height /5);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(Original, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
            return bmp;
        }
    }
}
