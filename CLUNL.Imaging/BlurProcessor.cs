using CLUNL.Imaging.GPUAcceleration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{
    public class BlurProcessor : IImageProcessor
    {
        readonly static string BlurProgram = @"
__kernel void ProcessImage(byte* InBitmap,byte* OutBitmap, int H,int W,float Radius,int BlurMode,float PixelSkips,float SampleSkips,boolean isRoundSample,boolean useWeight){
    return;
}
";
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
                arguments.ApplyFloats(ref Radius, ref PixelSkips, ref SampleSkips, ref BlurMode,ref ComputeMode);
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
                var imageByte = Utilities.BitmapToByteArray(Processing);
                var Kernel=CommonGPUAcceleration.Compile(BlurProgram,"ProcessImage");
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
