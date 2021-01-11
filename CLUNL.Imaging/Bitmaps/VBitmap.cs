using CLUNL.Imaging.GPUAcceleration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging.Bitmaps
{
    /// <summary>
    /// Virtual Bitmap. Max pixel count: Sqrt(int.MaxValue/4)=23,170
    /// </summary>
    public class VBitmap : IBitmapI, IDisposable
    {
        public readonly int W;
        public readonly int H;
        public int[] data;
        public object Property0;
        public VBitmap(int W, int H)
        {
            this.W = W;
            this.H = H;
        }
        public (int, int, int, int) GetPixel(int x, int y)
        {
            int index = (x * H + y) * 4;
            return (data[index], data[index + 1], data[index + 2], data[index + 3]);
        }
        public long DataSize()
        {
            return data.LongLength * sizeof(int);
        }
        public long PixelCount() => W * H;
        public void SetPixel(int x, int y, int R, int G, int B, int A)
        {
            int index = (x * H + y) * 4;
            data[index] = R;
            data[index + 1] = G;
            data[index + 2] = B;
            data[index + 3] = A;
        }
        public void SetPixel(int x, int y, (int, int, int, int) c)
        {
            int index = (x * H + y) * 4;
            data[index] = c.Item1;
            data[index + 1] = c.Item2;
            data[index + 2] = c.Item3;
            data[index + 3] = c.Item4;
        }
        public static VBitmap FromBitmap(Bitmap bitmap)
        {
            VBitmap result = new(bitmap.Width, bitmap.Height);
            result.data = Utilities.BitmapToIntArray(bitmap);
            return result;
        }
        public void ApplyToBitmap(Bitmap Target)
        {
            Utilities.WriteToBitmap(Target, ref data);
        }
        public Bitmap ToBitmap()
        {
            return Utilities.ByteArrayToBitmap(data, W, H);
        }

        public void Dispose()
        {
            data = null;
        }
    }
    /// <summary>
    /// Very Large Bitmap - LBitmap. This class should support 49,757,196,100,990 pixels for List can only store 2147483647 elements.
    /// </summary>
    public class LBitmap : IBitmapB, IBitmapI, IBitmapL
    {
        public readonly BigInteger W;
        public readonly BigInteger H;
        BigInteger WCount;
        BigInteger HCount;
        public List<VBitmap> realData;
        public List<VBitmap> RealData { get => realData; }
        public LBitmap(BigInteger W, BigInteger H)
        {
            realData = new();
            WCount = W / int.MaxValue;
            if (W % int.MaxValue != 0)
            {
                WCount++;
            }
            HCount = H / int.MaxValue;
            if (H % int.MaxValue != 0)
            {
                HCount++;
            }

            for (BigInteger x = 0; x < WCount; x++)
            {
                for (BigInteger y = 0; y < HCount; y++)
                {
                    int w = int.MaxValue;
                    int h = int.MaxValue;
                    VBitmap vBitmap = new(w, h);
                    vBitmap.Property0 = new VBitmapInLBitmap() { x = x, y = y };
                    realData.Add(vBitmap);
                }
            }
        }

        public (int, int, int, int) GetPixel(BigInteger x, BigInteger y)
        {
            var WCount = x / int.MaxValue;
            var LW = WCount;
            if (W % int.MaxValue != 0)
            {
                WCount++;
            }
            var HCount = y / int.MaxValue;
            var LH = HCount;
            if (H % int.MaxValue != 0)
            {
                HCount++;
            }
            VBitmap b = realData.ElementAt((int)(WCount * HCount));
            int ix = (int)(x - LW * int.MaxValue);
            int iy = (int)(y - LH * int.MaxValue);
            return b.GetPixel(ix, iy);
        }

        public void SetPixel(BigInteger x, BigInteger y, int R, int G, int B, int A)
        {
            var WCount = x / int.MaxValue;
            var LW = WCount;
            if (W % int.MaxValue != 0)
            {
                WCount++;
            }
            var HCount = y / int.MaxValue;
            var LH = HCount;
            if (H % int.MaxValue != 0)
            {
                HCount++;
            }
            VBitmap b = realData.ElementAt((int)(WCount * HCount));
            int ix = (int)(x - LW * int.MaxValue);
            int iy = (int)(y - LH * int.MaxValue);
            b.SetPixel(ix, iy, R, G, B, A);
        }

        public (int, int, int, int) GetPixel(int x, int y)
        {
            return GetPixel((BigInteger)x, (BigInteger)y);
        }

        public void SetPixel(int x, int y, int R, int G, int B, int A)
        {
            SetPixel((BigInteger)x, (BigInteger)y, R, G, B, A);
        }

        public long PixelCount()
        {
            return (long)(W * H);
        }

        public (int, int, int, int) GetPixel(long x, long y)
        {
            return GetPixel((BigInteger)x, (BigInteger)y);
        }

        public void SetPixel(long x, long y, int R, int G, int B, int A)
        {
            SetPixel((BigInteger)x, (BigInteger)y, R, G, B, A);
        }

        public void SetPixel(BigInteger x, BigInteger y, (int, int, int, int) color)
        {
            SetPixel(x, y, color.Item1, color.Item2, color.Item3, color.Item4);
        }

        public void SetPixel(long x, long y, (int, int, int, int) color)
        {
            SetPixel((BigInteger)x, (BigInteger)y, color);
        }

        public void SetPixel(int x, int y, (int, int, int, int) color)
        {
            SetPixel((BigInteger)x, (BigInteger)y, color);
        }
    }
    public interface IBitmapI
    {
        long PixelCount();
        (int, int, int, int) GetPixel(int x, int y);
        void SetPixel(int x, int y, int R, int G, int B, int A);
        void SetPixel(int x, int y, (int, int, int, int) color);
    }
    public interface IBitmapUI
    {
        (int, int, int, int) GetPixel(uint x, uint y);
        void SetPixel(uint x, uint y, int R, int G, int B, int A);
        void SetPixel(uint x, uint y, (int, int, int, int) color);
    }
    public interface IBitmapL
    {
        (int, int, int, int) GetPixel(long x, long y);
        void SetPixel(long x, long y, int R, int G, int B, int A);
        void SetPixel(long x, long y, (int, int, int, int) color);
    }
    public interface IBitmapUL
    {
        (int, int, int, int) GetPixel(ulong x, ulong y);
        void SetPixel(ulong x, ulong y, int R, int G, int B, int A);
        void SetPixel(ulong x, ulong y, (int, int, int, int) color);
    }
    public interface IBitmapB
    {
        (int, int, int, int) GetPixel(BigInteger x, BigInteger y);
        void SetPixel(BigInteger x, BigInteger y, int R, int G, int B, int A);
        void SetPixel(BigInteger x, BigInteger y, (int, int, int, int) color);
    }
    struct VBitmapInLBitmap
    {
        public BigInteger x;
        public BigInteger y;
    }

}
