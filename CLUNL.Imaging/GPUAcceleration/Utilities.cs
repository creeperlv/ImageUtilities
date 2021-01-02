using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging.GPUAcceleration
{
    public static class Utilities
    {
        public static byte[] BitmapToByteArray(Bitmap TargetBitmap)
        {
            byte[] result = new byte[TargetBitmap.Width * TargetBitmap.Height*4];
            for (int x = 0; x < TargetBitmap.Width; x++)
            {
                for (int y = 0; y < TargetBitmap.Height; y++)
                {
                    int index = x * y;
                    Color c = TargetBitmap.GetPixel(x, y);
                    result[index*4] = c.R;
                    result[index+1] = c.G;
                    result[index+2] = c.B;
                    result[index+3] = c.A;
                }
            }
            return result;
        }
        public static Bitmap ByteArrayToBitmap(byte[] array,int W,int H)
        {
            Bitmap result = new (W, H);
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = x * y;
                    result.SetPixel(x, y, Color.FromArgb(array[index + 3], array[index], array[index + 1], array[index + 2]));
                }
            }
            return result;
        }
    }
}
