using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging
{

    public interface IImageProcessor
    {
        void ProcessImage(Bitmap Processing, Bitmap OutputBitmap,ProcessorArguments arguments, Action OnCompleted = null);
        Color Process(Color c);
        int GetProgressStatusCode();
    }
    public record ProcessorArguments
    {
        public List<float> FloatArguments;
        public List<bool> Switches;
        public ProcessorArguments(List<float> Parameters,List<bool> Switches)
        {
            FloatArguments = Parameters;
            this.Switches = Switches;
        }
        public void ApplyFloats(ref float V1)
        {
            V1 = FloatArguments[0];
        }
        public void ApplyFloats(ref float V1, ref float V2)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5, ref float V6)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
            V6 = FloatArguments[5];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5, ref float V6, ref float V7)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
            V6 = FloatArguments[5];
            V7 = FloatArguments[6];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5, ref float V6, ref float V7, ref float V8)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
            V6 = FloatArguments[5];
            V7 = FloatArguments[6];
            V8 = FloatArguments[7];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5, ref float V6, ref float V7, ref float V8, ref float V9)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
            V6 = FloatArguments[5];
            V7 = FloatArguments[6];
            V8 = FloatArguments[7];
            V9 = FloatArguments[8];
        }
        public void ApplyFloats(ref float V1, ref float V2, ref float V3, ref float V4, ref float V5, ref float V6, ref float V7, ref float V8, ref float V9, ref float V10)
        {
            V1 = FloatArguments[0];
            V2 = FloatArguments[1];
            V3 = FloatArguments[2];
            V4 = FloatArguments[3];
            V5 = FloatArguments[4];
            V6 = FloatArguments[5];
            V7 = FloatArguments[6];
            V8 = FloatArguments[7];
            V9 = FloatArguments[8];
            V10 = FloatArguments[9];
        }
        public void ApplyBools(ref bool V1)
        {
            V1 = Switches[0];
        }
        public void ApplyBools(ref bool V1, ref bool V2)
        {
            V1 = Switches[0];
            V2 = Switches[1];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5, ref bool V6)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
            V6 = Switches[5];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5, ref bool V6, ref bool V7)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
            V6 = Switches[5];
            V7 = Switches[6];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5, ref bool V6, ref bool V7, ref bool V8)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
            V6 = Switches[5];
            V7 = Switches[6];
            V8 = Switches[7];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5, ref bool V6, ref bool V7, ref bool V8, ref bool V9)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
            V6 = Switches[5];
            V7 = Switches[6];
            V8 = Switches[7];
            V9 = Switches[8];
        }
        public void ApplyBools(ref bool V1, ref bool V2, ref bool V3, ref bool V4, ref bool V5, ref bool V6, ref bool V7, ref bool V8, ref bool V9, ref bool V10)
        {
            V1 = Switches[0];
            V2 = Switches[1];
            V3 = Switches[2];
            V4 = Switches[3];
            V5 = Switches[4];
            V6 = Switches[5];
            V7 = Switches[6];
            V8 = Switches[7];
            V9 = Switches[8];
            V10 = Switches[9];
        }
        public ProcessorArguments(params object[] Arguments)
        {
            FloatArguments = new ();
            Switches = new ();
            foreach (var item in Arguments)
            {
                if (item is float)
                {
                    FloatArguments.Add((float)item);
                }
                else
                if (item is int)
                {
                    FloatArguments.Add((int)item);
                }
                else if(item is bool)
                {
                    Switches.Add((bool)item);
                }
            }
        }
    }
}
