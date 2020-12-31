using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging.Carve
{
    [Serializable]
    public class Carve
    {
        static Carve()
        {
            Empty.SetPixel(0, 0, Color.Empty);
        }
        public static Bitmap Empty = new(1, 1);
        public List<CarveElement> Elements = new List<CarveElement>();
        public float BackgroundHeight = 0.5f;
        public Bitmap GetBitmap(float TargetW, float TargetH)
        {
            Bitmap bitmap = new((int)TargetW, (int)TargetH);
            var graph = Graphics.FromImage(bitmap);
            byte G = (byte)(byte.MaxValue * BackgroundHeight);
            var brush = new SolidBrush(Color.FromArgb(G, G, G));
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.FillRectangle(brush, new RectangleF(0, 0, TargetW, TargetH));
            foreach (var item in Elements)
            {
                graph.DrawImage(item.GetBitmap(TargetW, TargetH), 0, 0);
            }
            return bitmap;
        }
    }
    [Serializable]
    public class CarveElement
    {
        public float Height;
        public float PositionX;
        public float PositionY;
        public virtual Bitmap GetBitmap(float CanvasW, float CanvasH)
        {
            return Carve.Empty;
        }
    }
    [Serializable]
    public class CarveLine: CarveElement
    {
        public LineNode Start;

    }
    [Serializable]
    public class LineNode
    {
        public float PositionX;
        public float PositionY;
        public LineNode NextNode;
    }
    [Serializable]
    public class CarveText : CarveElement
    {
        public Font Font;
        public string Text;
        public float Size;
        /// <summary>
        /// 0-Left,1-Center,2-Right
        /// </summary>
        public int HorizontalAlignMode;
        public override Bitmap GetBitmap(float CanvasW, float CanvasH)
        {
            return base.GetBitmap(CanvasW, CanvasH);
        }
    }
}
