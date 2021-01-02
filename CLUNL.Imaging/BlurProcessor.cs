using CLUNL.Imaging.GPUAcceleration;
using OpenCL.NetCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{
    public partial class BlurProcessor : IImageProcessor
    {
        public static BlurProcessor CurrentBlurProcessor = new();
        public Color Process(Color c)
        {
            return c;
        }
        float Radius = 1;
        float SquareRadius;
        float D;
        float BlurMode = 0;
        float PixelSkips = 1;
        float SampleSkips = 1;
        float ComputeMode = 0;
        bool isRoundSample = false;
        bool useWeight = false;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, ProcessorArguments arguments, Action OnCompleted = null)
        {
            if (arguments is not null)
            {
                arguments.ApplyFloats(ref Radius, ref PixelSkips, ref SampleSkips, ref BlurMode, ref ComputeMode);
                try
                {
                    arguments.ApplyBools(ref isRoundSample, ref useWeight);
                }
                catch (Exception)
                {
                }
            }
            if (ComputeMode != 0)
            {
                int GPU = (int)ComputeMode - 1;
                CommonGPUAcceleration.SetGPU(GPU);
                var Kernel = CommonGPUAcceleration.Compile(BlurProgram, "ProcessImage");
                var imageByte = Utilities.BitmapToIntArray(Processing);
                int[] result = new int[imageByte.Length];
                int FloatSize = sizeof(float);
                int BoolSize = sizeof(bool);
                int IntSize = sizeof(int);
                int BlurMode = (int)this.BlurMode;
                var A0 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, imageByte.Length * sizeof(int));
                var A1 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, IntSize);//Processing.Height
                var A2 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, IntSize);//Width
                var A3 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, FloatSize);//Radius
                var A4 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, IntSize);//BlurMode
                var A5 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, FloatSize);//PixelSkips
                var A6 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, FloatSize);//SampleSkips
                var A7 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, BoolSize);//isRoundSample
                var A8 = CommonGPUAcceleration.CreateBuffer(MemFlags.ReadOnly, BoolSize);//useWeight
                var A9 = CommonGPUAcceleration.CreateBuffer(MemFlags.WriteOnly, imageByte.Length * sizeof(int));//Result
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 0, sizeof(byte) * imageByte.Length, A0, imageByte.Length * sizeof(byte), imageByte);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 1, IntSize, A1, IntSize, Processing.Height);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 2, IntSize, A2, IntSize, Processing.Width);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 3, FloatSize, A3, FloatSize, Radius);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 4, IntSize, A4, IntSize, BlurMode);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 5, FloatSize, A5, FloatSize, PixelSkips);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 6, FloatSize, A6, FloatSize, SampleSkips);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 7, BoolSize, A7, BoolSize, isRoundSample);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 8, BoolSize, A8, BoolSize, useWeight);
                CommonGPUAcceleration.SetArg(Kernel, Bool.True, 8, BoolSize, A9, BoolSize, useWeight,false);
                CommonGPUAcceleration.Execute(Kernel, imageByte.Length * sizeof(int));
                CommonGPUAcceleration.ReadArg<int>(A9,ref result);
                Utilities.WriteToBitmap(OutputBitmap, result);
                return;
            }
            D = Radius * 2;
            SquareRadius = Radius * Radius;
            W = Processing.Width;
            H = Processing.Height;

            for (int x = 0; x < W; x++)
            {
                if (x % (int)PixelSkips == 0)
                    for (int y = 0; y < H; y++)
                    {
                        if (y % (int)PixelSkips == 0)
                        {
                            if (BlurMode == 0)
                            {
                                OutputBitmap.SetPixel(x, y, GatherAndMix(Processing, x, y));
                            }
                            else if (BlurMode == 1)
                            {
                                OutputBitmap.SetPixel(x, y, VerticalMix(Processing, x, y));
                            }
                            else if (BlurMode == 2)
                            {
                                OutputBitmap.SetPixel(x, y, HorizontalMix(Processing, x, y));
                            }
                            else if (BlurMode == 3)
                            {
                                OutputBitmap.SetPixel(x, y, CrossMix(Processing, x, y));
                            }
                        }
                        else
                        {
                            OutputBitmap.SetPixel(x, y, Processing.GetPixel(x, y));
                        }
                    }
                else
                {
                    for (int y = 0; y < Processing.Height; y++)
                    {
                        OutputBitmap.SetPixel(x, y, Processing.GetPixel(x, y));
                    }
                }
            }
            if (OnCompleted is not null) OnCompleted();
        }
        int W;
        int H;
        public Color CrossMix(Bitmap Target, int CenterX, int CenterY)
        {
            float Count = 0;
            float R = 0;
            float G = 0;
            float B = 0;
            float A = 0;
            for (int y = 0; y < D; y++)
            {
                if (y % SampleSkips == 0)
                {
                    var disY = y - Radius;
                    var PR = (disY * disY);
                    {
                        int TargetX = CenterX;
                        int TargetY = CenterY + (int)disY;
                        if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                        {
                            var c = Target.GetPixel(TargetX, TargetY);
                            if (useWeight == false)
                            {
                                Count++;
                                R += c.R;
                                G += c.G;
                                B += c.B;
                                A += c.A;
                            }
                            else
                            {
                                var rate = PR / SquareRadius;
                                R += c.R * rate;
                                G += c.G * rate;
                                B += c.B * rate;
                                A += c.A * rate;
                                Count += rate;
                            }
                        }
                    }
                }

            }
            for (int x = 0; x < D; x++)
            {
                if (x % SampleSkips == 0)
                {

                    var disX = x - Radius;
                    var PR = (disX * disX);
                    {
                        int TargetX = CenterX + (int)disX;
                        int TargetY = CenterY;
                        if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                        {
                            var c = Target.GetPixel(TargetX, TargetY);
                            if (useWeight == false)
                            {
                                Count++;
                                R += c.R;
                                G += c.G;
                                B += c.B;
                                A += c.A;
                            }
                            else
                            {
                                var rate = PR / SquareRadius;
                                R += c.R * rate;
                                G += c.G * rate;
                                B += c.B * rate;
                                A += c.A * rate;
                                Count += rate;
                            }
                        }
                    }
                }
            }
            if (Count != 0)
                return Color.FromArgb((byte)(A / Count), (byte)(R / Count), (byte)(G / Count), (byte)(B / Count));
            else return Color.Transparent;
        }
        public Color VerticalMix(Bitmap Target, int CenterX, int CenterY)
        {
            float Count = 0;
            float R = 0;
            float G = 0;
            float B = 0;
            float A = 0;
            for (int y = 0; y < D; y++)
            {
                if (y % SampleSkips == 0)
                {
                    var disY = y - Radius;
                    var PR = (disY * disY);
                    if (PR <= SquareRadius)
                    {
                        int TargetX = CenterX;
                        int TargetY = CenterY + y - (int)Radius;
                        if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                        {
                            var c = Target.GetPixel(TargetX, TargetY);
                            if (useWeight == false)
                            {
                                Count++;
                                R += c.R;
                                G += c.G;
                                B += c.B;
                                A += c.A;
                            }
                            else
                            {
                                var rate = PR / SquareRadius;
                                R += c.R * rate;
                                G += c.G * rate;
                                B += c.B * rate;
                                A += c.A * rate;
                                Count += rate;
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < D; x++)
            {
                if (x % SampleSkips == 0)
                {

                    var disX = x - Radius;
                    var PR = (disX * disX);
                    if (PR <= SquareRadius)
                    {
                        int TargetX = CenterX + x - (int)Radius;
                        int TargetY = CenterY;
                        if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                        {
                            var c = Target.GetPixel(TargetX, TargetY);
                            if (useWeight == false)
                            {
                                Count++;
                                R += c.R;
                                G += c.G;
                                B += c.B;
                                A += c.A;
                            }
                            else
                            {
                                var rate = PR / SquareRadius;
                                R += c.R * rate;
                                G += c.G * rate;
                                B += c.B * rate;
                                A += c.A * rate;
                                Count += rate;
                            }
                        }
                    }
                }
            }
            if (Count != 0)
                return Color.FromArgb((byte)(A / Count), (byte)(R / Count), (byte)(G / Count), (byte)(B / Count));
            else return Color.Transparent;
        }
        public Color HorizontalMix(Bitmap Target, int CenterX, int CenterY)
        {
            float Count = 0;
            float R = 0;
            float G = 0;
            float B = 0;
            float A = 0;
            for (int x = 0; x < D; x++)
            {
                if (x % SampleSkips == 0)
                {

                    var disX = x - Radius;
                    var PR = (disX * disX);
                    if (PR <= SquareRadius)
                    {
                        int TargetX = CenterX + x - (int)Radius;
                        int TargetY = CenterY;
                        if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                        {
                            var c = Target.GetPixel(TargetX, TargetY);
                            if (useWeight == false)
                            {
                                Count++;
                                R += c.R;
                                G += c.G;
                                B += c.B;
                                A += c.A;
                            }
                            else
                            {
                                var rate = PR / SquareRadius;
                                R += c.R * rate;
                                G += c.G * rate;
                                B += c.B * rate;
                                A += c.A * rate;
                                Count += rate;
                            }
                        }
                    }
                }
            }
            if (Count != 0)
                return Color.FromArgb((byte)(A / Count), (byte)(R / Count), (byte)(G / Count), (byte)(B / Count));
            else return Color.Transparent;
        }
        public Color GatherAndMix(Bitmap Target, int CenterX, int CenterY)
        {

            float Count = 0;
            float R = 0;
            float G = 0;
            float B = 0;
            float A = 0;
            if (isRoundSample == true)
            {

                for (int x = 0; x < D; x++)
                {
                    if (x % SampleSkips == 0)
                        for (int y = 0; y < D; y++)
                        {
                            if (y % SampleSkips == 0)
                            {
                                var disX = x - Radius;
                                var disY = y - Radius;
                                var PR = (disX * disX + disY * disY);
                                if (PR <= SquareRadius)
                                {
                                    int TargetX = CenterX + x - (int)Radius;
                                    int TargetY = CenterY + y - (int)Radius;
                                    if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                                    {
                                        var c = Target.GetPixel(TargetX, TargetY);
                                        if (useWeight == false)
                                        {
                                            Count++;
                                            R += c.R;
                                            G += c.G;
                                            B += c.B;
                                            A += c.A;
                                        }
                                        else
                                        {
                                            var rate = PR / SquareRadius;
                                            R += c.R * rate;
                                            G += c.G * rate;
                                            B += c.B * rate;
                                            A += c.A * rate;
                                            Count += rate;
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            else
            {
                for (int x = 0; x < D; x++)
                {
                    if (x % SampleSkips == 0)
                        for (int y = 0; y < D; y++)
                        {
                            if (y % SampleSkips == 0)
                            {
                                int TargetX = CenterX + x - (int)Radius;
                                int TargetY = CenterY + y - (int)Radius;
                                if (TargetX >= 0 && TargetX < W && TargetY >= 0 && TargetY < H)
                                {
                                    var c = Target.GetPixel(TargetX, TargetY);
                                    if (useWeight == true)
                                    {
                                        var disX = x - Radius;
                                        var disY = y - Radius;
                                        var PR = (disX * disX + disY * disY);
                                        var rate = PR / SquareRadius;
                                        R += c.R * rate;
                                        G += c.G * rate;
                                        B += c.B * rate;
                                        A += c.A * rate;
                                        Count += rate;
                                    }
                                    else
                                    {
                                        Count++;
                                        R += c.R;
                                        G += c.G;
                                        B += c.B;
                                        A += c.A;

                                    }
                                }
                            }
                        }
                }
            }
            //Trace.WriteLine($"{CenterX},{CenterY}->A:{(A / Count)})({A}/{Count} pixels in total.)");
            if (Count != 0)
                return Color.FromArgb((byte)(A / Count), (byte)(R / Count), (byte)(G / Count), (byte)(B / Count));
            else return Color.Transparent;
        }
    }
}
