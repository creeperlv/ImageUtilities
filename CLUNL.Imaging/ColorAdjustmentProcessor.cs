using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{
    public class ColorAdjustmentProcessor : IImageProcessor
    {
        public static ColorAdjustmentProcessor CurrentColorAdjustmentProcessor = new();
        float CB_RValue = 255;
        float CB_GValue = 255;
        float CB_BValue = 255;
        float CB_AValue = 255;
        float BrightIntensity = 1;
        float CIntensity = 1;
        float ColorBlendMode;
        bool isInventColor = false;
        bool WillAdjustBrightness = false;
        bool WillScaleBrightness = false;
        bool WillPerformColorBlend = false;
        bool WillPerformColorBlend_IgnoreTransparency = false;
        public Color Process(Color c)
        {
            byte R = c.R;
            byte G = c.G;
            byte B = c.B;
            byte A = c.A;
            if (isInventColor == true)
            {
                R = (byte)(byte.MaxValue - R);
                G = (byte)(byte.MaxValue - G);
                B = (byte)(byte.MaxValue - B);
            }
            if (WillAdjustBrightness)
            {
                {
                    float RRate = R + BrightIntensity;
                    R = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
                {
                    float RRate = G + BrightIntensity;
                    G = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
                {
                    float RRate = B + BrightIntensity;
                    B = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
            }
            if (WillScaleBrightness)
            {
                {
                    float RRate = R * CIntensity;
                    R = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
                {
                    float RRate = G * CIntensity;
                    G = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
                {
                    float RRate = B * CIntensity;
                    B = (byte)(Math.Max(Math.Min(RRate, byte.MaxValue), 0));
                }
            }
            if (WillPerformColorBlend)
            {
                R = (byte)PerformBlend(R, CB_RValue);
                G = (byte)PerformBlend(G, CB_GValue);
                B = (byte)PerformBlend(B, CB_BValue);
                if (WillPerformColorBlend_IgnoreTransparency == false)
                    A = (byte)PerformBlend(A, CB_AValue);
                float PerformBlend(float Base, float Layer)
                {

                    if (ColorBlendMode == 1)
                        return (float)Math.Min(Base * GetIntensity(Layer), byte.MaxValue);
                    else if (ColorBlendMode == 0)
                        return (float)Math.Min(Base + Layer, byte.MaxValue);
                    else if (ColorBlendMode == 2)
                        return (float)Math.Max(Base - Layer, 0);
                    return Base;
                }
            }
            {
                Color Result = Color.FromArgb(A, R, G, B);
                return Result;
            }
            float GetIntensity(double value) => (float)value / 255f;
        }

        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, ProcessorArguments arguments, Action OnCompleted = null)
        {
            if(arguments is not null)
            {
                arguments.ApplyFloats(ref CB_RValue,ref CB_GValue,ref CB_BValue,ref CB_AValue,ref BrightIntensity,ref CIntensity,ref ColorBlendMode);
                arguments.ApplyBools(ref isInventColor, ref WillAdjustBrightness, ref WillScaleBrightness, ref WillPerformColorBlend, ref WillPerformColorBlend_IgnoreTransparency);
            }
            int WB = Processing.Width;
            int H = Processing.Height;
            for (int w = 0; w < WB; w++)
            {
                for (int h = 0; h < H; h++)
                {
                    var c = Processing.GetPixel(w, h);
                    OutputBitmap.SetPixel(w, h, Process(c));
                }
            }
            if(OnCompleted is not null)
            {
                OnCompleted();
            }
        }
    }
}
