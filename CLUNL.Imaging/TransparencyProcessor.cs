using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{
    public class TransparencyProcessor : IImageProcessor
    {
        public static TransparencyProcessor CurrentTransparencyProcessor = new();
        float RIntensity = 1;
        float GIntensity = 1;
        float BIntensity = 1;
        float AIntensity = 1;
        float RGBIntensity = 3;
        float RValue = 255;
        float GValue = 255;
        float BValue = 255;
        float AValue = 255;
        float R1Value = 0;
        float G1Value = 0;
        float B1Value = 0;
        float CutoutMode1 = 0;
        float CutoutMode2 = 0;
        bool isTransparencyCutout = false;
        bool isMixColor = false;
        public Color Process(Color c)
        {
            if (isTransparencyCutout)
            {
                if (CutoutMode2 == 0)
                {
                    if (c.R <= R1Value && c.G <= G1Value && c.B <= B1Value)
                    {
                        return Color.FromArgb(0, c.R, c.G, c.B);
                    }
                }
                else
                if (CutoutMode2 == 1)
                    if (c.R <= R1Value || c.G <= G1Value || c.B <= B1Value)
                    {
                        return Color.FromArgb(0, c.R, c.G, c.B);
                    }
            }
            if (isMixColor)
            {

                float AlphaIntensity = 0.0f;
                {
                    float total = c.R * RIntensity + c.G * GIntensity + c.B * BIntensity;
                    float rate = total / (255f * RGBIntensity);
                    float rate2 = rate * AIntensity;
                    //AlphaIntensity = 1-rate2;
                    AlphaIntensity = rate2;
                    if (CutoutMode1 == 1)
                        if (c.R >= RValue && c.G >= GValue && c.B >= BValue)
                        {
                            rate2 = ((float)c.A) / 255f;
                            AlphaIntensity = rate2;
                        }
                    if (CutoutMode1 == 2)
                        if (c.R >= RValue || c.G >= GValue || c.B >= BValue)
                        {
                            rate2 = ((float)c.A) / 255f;
                            //rate2 = AIntensity;
                            AlphaIntensity = rate2;
                        }

                }
                var A = (byte)Math.Min((Byte.MaxValue * AlphaIntensity), Byte.MaxValue);
                Color Result = Color.FromArgb(A, c.R, c.G, c.B);
                return Result;
            }
            return c;
        }


        float GetIntensity(double value) => (float)value / 255f;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, ProcessorArguments arguments, Action OnCompleted = null)
        {
            if (arguments is not null)
            {
                arguments.ApplyFloats(ref RValue, ref GValue, ref BValue, ref AValue, ref R1Value, ref G1Value, ref B1Value, ref CutoutMode1, ref CutoutMode2);
                arguments.ApplyBools(ref isMixColor, ref isTransparencyCutout);
            }
            RIntensity = GetIntensity(RValue);
            GIntensity = GetIntensity(GValue);
            BIntensity = GetIntensity(BValue);
            AIntensity = GetIntensity(AValue);

            RGBIntensity = RIntensity + BIntensity + GIntensity;

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
            if (OnCompleted is not null)
            {
                OnCompleted();
            }
        }

        public int GetProgressStatusCode()
        {
            return -1;
        }
    }
}
