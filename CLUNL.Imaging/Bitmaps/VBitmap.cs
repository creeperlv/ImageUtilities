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
    /// Virtual Bitmap
    /// </summary>
    public class VBitmap:IBitmapI
    {
        public readonly int W;
        public readonly int H;
        public int[] data;
        public object Property0;
        public VBitmap(int W,int H)
        {
            this.W = W;
            this.H = H;
        }
        public (int,int,int,int) GetPixel(int x,int y)
        {
            int index = (x * H + y) * 4;
            return (data[index], data[index+1], data[index+2], data[index+3]);
        }
        public long DataSize()
        {
            return data.LongLength * sizeof(int);
        }
        public long PixelCount() => W * H;
        public void SetPixel(int x,int y,int R,int G,int B,int A)
        {
            int index = (x * H + y) * 4;
            data[index] = R;
            data[index+1] = G;
            data[index+2] = B;
            data[index+3] = A;
        }
        public static VBitmap FromBitmap(Bitmap bitmap)
        {
            VBitmap result = new(bitmap.Width, bitmap.Height);
            result.data = Utilities.BitmapToIntArray(bitmap);
            return result;
        }
        public void ApplyToBitmap(Bitmap Target)
        {
            Utilities.WriteToBitmap(Target, data);
        }
        public Bitmap ToBitmap()
        {
            return Utilities.ByteArrayToBitmap(data, W, H);
        }
    }
    public class LBitmap:IBitmapB,IBitmapI
    {
        public readonly BigInteger W;
        public readonly BigInteger H;
        BigInteger WCount;
        BigInteger HCount;
        public List<VBitmap> RealData; 
        public LBitmap(BigInteger W,BigInteger H)
        {
            RealData = new();
            WCount = W / int.MaxValue;
            if (W % int.MaxValue != 0)
            {
                WCount ++;
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
                    int w=int.MaxValue;
                    int h = int.MaxValue;
                    VBitmap vBitmap = new(w, h);
                    vBitmap.Property0 = new VBitmapInLBitmap() { x = x, y = y };
                    RealData.Add(vBitmap);
                }
            }
        }

        public (int, int, int, int) GetPixel(BigInteger x, BigInteger y)
        {
            throw new NotImplementedException();
        }

        public void SetPixel(BigInteger x, BigInteger y, int R, int G, int B, int A)
        {
            throw new NotImplementedException();
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
    }
    public interface IBitmapI
    {
        long PixelCount();
        (int, int, int, int) GetPixel(int x, int y);
        void SetPixel(int x, int y, int R, int G, int B, int A);
    }
    public interface IBitmapUI
    {
        (int, int, int, int) GetPixel(uint x, uint y);
        void SetPixel(uint x, uint y, int R, int G, int B, int A);
    }
    public interface IBitmapL
    {
        (int, int, int, int) GetPixel(long x, long y);
        void SetPixel(long x, long y, int R, int G, int B, int A);
    }
    public interface IBitmapUL
    {
        (int, int, int, int) GetPixel(ulong x, ulong y);
        void SetPixel(ulong x, ulong y, int R, int G, int B, int A);
    }
    public interface IBitmapB
    {
        (int, int, int, int) GetPixel(BigInteger x, BigInteger y);
        void SetPixel(BigInteger x, BigInteger y, int R, int G, int B, int A);
    }
    struct VBitmapInLBitmap
    {
        public BigInteger x;
        public BigInteger y;
    }

}
