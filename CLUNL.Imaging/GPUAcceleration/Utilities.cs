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
        public static int[] BitmapToIntArray(Bitmap TargetBitmap)
        {
            int[] result = new int[(TargetBitmap.Width+1) * (TargetBitmap.Height)*4];
            int W = TargetBitmap.Width;
            int H = TargetBitmap.Height;
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x * H + y) * 4;
                    Color c = TargetBitmap.GetPixel(x, y);
                    result[index] = c.R;
                    result[index+1] = c.G;
                    result[index+2] = c.B;
                    result[index+3] = c.A;
                }
            }
            return result;
        }
        public static void WriteToBitmap(Bitmap TargetMap, int[] data) {
            int W = TargetMap.Width;
            int H = TargetMap.Height;
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x *H+ y)*4;
                    TargetMap.SetPixel(x, y, Color.FromArgb(data[index + 3], data[index], data[index + 1], data[index + 2]));
                }
            }
        }
        public static Bitmap ByteArrayToBitmap(int[] array,int W,int H)
        {
            Bitmap result = new (W, H);
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x * H + y )* 4;
                    result.SetPixel(x, y, Color.FromArgb(array[index + 3], array[index], array[index + 1], array[index + 2]));
                }
            }
            return result;
        }
    }
}
