using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{
    /// <summary>
    /// A grayscale processor with many custom options. For arguments, it should follow following order: R,G,B,A,isMixColor,isRGBAIntensity,isBlackAsFullTranparent,isReserveTransparency
    /// </summary>
    public class GrayscaleProcessor : IImageProcessor
    {
        public static GrayscaleProcessor CurrentGrayscaleProcessor = new GrayscaleProcessor();
        float RIntensity = 1;
        float GIntensity = 1;
        float BIntensity = 1;
        float AIntensity = 0;
        float RGBAIntensity = 4;
        float RValue = 255;
        float GValue = 255;
        float BValue = 255;
        float AValue = 0;
        bool isMixColor = false;
        bool isRGBAIntensity = false;
        bool isBlackAsFullTranparent = false;
        bool isReserveTransparency = true;
        public Color Process(Color c)
        {
            float GrayIntensity = 0.0f;
            if (isMixColor)
            {
                float total = c.R * RIntensity + c.G * GIntensity + c.B * BIntensity + c.A * AIntensity;
                float rate = total / (byte.MaxValue * (isRGBAIntensity == true ? RGBAIntensity : 1f));
                GrayIntensity = rate;

            }
            else
            {
                float total = c.R + c.G + c.B;
                float rate = total / (byte.MaxValue * (isRGBAIntensity == true ? 3 : 1f));
                GrayIntensity = rate;
            }
            var G = (byte)Math.Min((byte.MaxValue * GrayIntensity), byte.MaxValue);
            if (isBlackAsFullTranparent == true) if (c.A == 0) G = 0;
            Color Result = Color.FromArgb(isReserveTransparency == false ? 255 : c.A, G, G, G);
            return Result;

        }

        float GetIntensity(double value) => (float)value / 255f;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, ProcessorArguments arguments = null, Action OnCompleted = null)
        {
            if (arguments is not null)
            {
                arguments.ApplyFloats(ref RValue, ref GValue,ref BValue, ref AValue);
                arguments.ApplyBools(ref isMixColor, ref isRGBAIntensity, ref isRGBAIntensity, ref isReserveTransparency);
                RIntensity = GetIntensity(RValue);
                GIntensity = GetIntensity(GValue);
                BIntensity = GetIntensity(BValue);
                AIntensity = GetIntensity(AValue);
                RGBAIntensity = RIntensity + BIntensity + GIntensity + AIntensity;
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
            if (OnCompleted is not null)
                OnCompleted();
            GC.Collect();
        }
        public int GetProgressStatusCode()
        {
            return -1;
        }
    }
}
