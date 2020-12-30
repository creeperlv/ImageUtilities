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
        public ProcessorArguments(params object[] Arguments)
        {
            FloatArguments = new ();
            Switches = new ();
            foreach (var item in Arguments)
            {
                if(item is float)
                {
                    FloatArguments.Add((float)item);
                }else if(item is bool)
                {
                    Switches.Add((bool)item);
                }
            }
        }
    }
}
