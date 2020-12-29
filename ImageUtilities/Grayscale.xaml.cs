using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace ImageUtilities
{
    /// <summary>
    /// Interaction logic for Grayscale.xaml
    /// </summary>
    public partial class Grayscale : UserControl
    {
        public Grayscale()
        {
            InitializeComponent();
        }

        Bitmap Current;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (Current is not null)
                    Current.Dispose();
            }
            catch (Exception)
            {
            }
            Bitmap Operating = VariablePool.CurrentBitmap_DownSized;
            if (IsUseDownsize.IsChecked == false)
            {
                Operating = VariablePool.CurrentBitmap;
            }
            Current = new Bitmap(Operating.Width, Operating.Height);
            FunctionArea.IsEnabled = false;
            ProcessImage(Operating, Current);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Bitmap Operating = VariablePool.CurrentBitmap;
            Current = new Bitmap(Operating.Width, Operating.Height);
            FunctionArea.IsEnabled = false;
            ProcessImage(Operating, VariablePool.CurrentBitmap, () =>
            {
                MainWindow.CurrentWindow.UpdatePreview();
                MainWindow.CurrentWindow.DownSizeAll();
            });
        }
        public void UpdateView(Bitmap Current)
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(Current);
            Preview.Source = ImgSrc;

        }
        float RIntensity = 1;
        float GIntensity = 1;
        float BIntensity = 1;
        float AIntensity = 1;
        float RGBAIntensity = 4;
        float RValue = 255;
        float GValue = 255;
        float BValue = 255;
        float AValue = 255;
        bool isMixColor = false;
        bool isRGBAIntensity = false;
        bool isReserveTransparency = false;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, Action action = null)
        {
            RValue = (float)R.Value;
            GValue = (float)G.Value;
            BValue = (float)B.Value;
            AValue = (float)Alpha.Value;
            RIntensity = GetIntensity(RValue);
            GIntensity = GetIntensity(GValue);
            BIntensity = GetIntensity(BValue);
            AIntensity = GetIntensity(AValue);
            isMixColor = MixColorGrayscale.IsChecked.Value;
            RGBAIntensity = RIntensity + BIntensity + GIntensity + AIntensity;
            isReserveTransparency = ReserveTransparency.IsChecked.Value;
            isRGBAIntensity = UseRGBAIntensity.IsChecked.Value;
            float GetIntensity(double value) => (float)value / 255f;

            Task.Run(() =>
            {
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
                Dispatcher.Invoke(() =>
                {
                    UpdateView(OutputBitmap);
                    FunctionArea.IsEnabled = true;
                    if (action is not null) action();
                });
                GC.Collect();
            });
        }
        public Color Process(Color c)
        {

            {
                float GrayIntensity = 0.0f;
                if (isMixColor)
                {
                    float total = c.R * RIntensity + c.G * GIntensity + c.B * BIntensity + c.A * AIntensity;
                    float rate = total / (byte.MaxValue * (isRGBAIntensity==true? RGBAIntensity:1f));
                    GrayIntensity = rate;

                }
                else
                {
                    float total = c.R + c.G + c.B;
                    float rate = total / (byte.MaxValue * (isRGBAIntensity == true ? 3 : 1f));
                    GrayIntensity = rate;
                }
                var G = (byte)Math.Min((byte.MaxValue * GrayIntensity), byte.MaxValue);
                Color Result = Color.FromArgb(isReserveTransparency == false ? 255 : c.A, G, G, G);
                return Result;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Current is not null) Current.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
