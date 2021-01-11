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
            int[] result = new int[(TargetBitmap.Width + 1) * (TargetBitmap.Height) * 4];
            int W = TargetBitmap.Width;
            int H = TargetBitmap.Height;
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x * H + y) * 4;
                    Color c = TargetBitmap.GetPixel(x, y);
                    result[index] = c.R;
                    result[index + 1] = c.G;
                    result[index + 2] = c.B;
                    result[index + 3] = c.A;
                }
            }
            return result;
        }
        public static void WriteToBitmap(Bitmap TargetMap,ref int[] data)
        {
            int W = TargetMap.Width;
            int H = TargetMap.Height;
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x * H + y) * 4;
                    TargetMap.SetPixel(x, y, Color.FromArgb(data[index + 3], data[index], data[index + 1], data[index + 2]));
                }
            }
        }
        /// <summary>
        /// Seperate the image data to target block counts.
        /// </summary>
        /// <param name="OriginalData"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="BlockCount"></param>
        /// <param name="Orientation">0-Horizontal, Other - Vertical</param>
        /// <returns></returns>
        public static List<int[]> SeperateArray(int[] OriginalData, int W, int H, int BlockCount, int Orientation)
        {
            List<int[]> result = new();
            if (Orientation == 0)
            {
                int[] temp = null;
                int CurrentBlock = 0;
                int HGap = H / BlockCount;
                for (int y = 0; y < H; y++)
                {
                    if (y % HGap == 0)
                    {
                        CurrentBlock++;
                        if(temp is not null)
                        {
                            result.Add(temp);
                        }
                        if (CurrentBlock == BlockCount)
                        {
                            temp = new int[(H - HGap * (CurrentBlock - 1)) * W];
                        }
                        else
                        {
                            temp = new int[HGap * W];
                        }
                    }
                    if (temp is null)
                    {
                        if (CurrentBlock == BlockCount)
                        {
                            temp = new int[(H - HGap * (CurrentBlock - 1)) * W];
                        }
                        else
                        {
                            temp = new int[HGap * W];
                        }
                    }
                    for (int x = 0; x < W; x++)
                    {
                        int index = (x * H + (y - HGap * (CurrentBlock - 1))) * 4;
                        int index2 = (x * H + y) * 4;

                        temp[index] = OriginalData[index2];
                        temp[index + 1] = OriginalData[index2 + 1];
                        temp[index + 2] = OriginalData[index2 + 2];
                        temp[index + 3] = OriginalData[index2 + 3];
                    }
                }
            }
            else
            {

            }
            return result;
        }
        public static Bitmap ByteArrayToBitmap(int[] array, int W, int H)
        {
            Bitmap result = new(W, H);
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    int index = (x * H + y) * 4;
                    result.SetPixel(x, y, Color.FromArgb(array[index + 3], array[index], array[index + 1], array[index + 2]));
                }
            }
            return result;
        }
    }
}
