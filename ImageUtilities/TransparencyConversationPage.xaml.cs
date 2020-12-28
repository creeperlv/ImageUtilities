using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for TransparencyConversationPage.xaml
    /// </summary>
    public partial class TransparencyConversationPage : UserControl
    {
        public TransparencyConversationPage()
        {
            InitializeComponent();
        }
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
        int CutoutMode1 = 0;
        int CutoutMode2 = 0;
        bool isTransparencyCutout = false;
        bool isMixColor = false;
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
            RValue = (float)R.Value;
            GValue = (float)G.Value;
            BValue = (float)B.Value;
            R1Value = (float)R1.Value;
            G1Value = (float)G1.Value;
            B1Value = (float)B1.Value;
            AValue = (float)Alpha.Value;
            RIntensity = GetIntensity(RValue);
            GIntensity = GetIntensity(GValue);
            BIntensity = GetIntensity(BValue);
            AIntensity = GetIntensity(AValue);
            isTransparencyCutout = TransparencyCutout.IsChecked.Value;
            isMixColor = MixColorTransparency.IsChecked.Value;
            CutoutMode1 = CutoutMode.SelectedIndex;
            CutoutMode2 = CutoutModeTC.SelectedIndex;
            RGBIntensity = RIntensity + BIntensity + GIntensity;
            Bitmap Operating = VariablePool.CurrentBitmap_DownSized;
            if (IsUseDownsize.IsChecked == false)
            {
                Operating = VariablePool.CurrentBitmap;
            }
            Current = new Bitmap(Operating.Width, Operating.Height);
            float GetIntensity(double value)
            {
                return (float)value / 255f;
            }
            (sender as Button).IsEnabled = false;
            Task.Run(() =>
            {
                int WB = Operating.Width;
                int H = Operating.Height;
                for (int w = 0; w < WB; w++)
                {
                    for (int h = 0; h < H; h++)
                    {
                        var c = Operating.GetPixel(w, h);
                        Current.SetPixel(w, h, Process(c));
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    UpdateView();
                    (sender as Button).IsEnabled = true;
                });
                System.GC.Collect();
            });

        }
        public void UpdateView()
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(Current);
            Preview.Source = ImgSrc;

        }
        public Color Process(Color c)
        {
            if (isTransparencyCutout)
            {
                if (CutoutMode2 == 0)
                    if (c.R <= R1Value && c.G <= G1Value && c.B <= B1Value)
                    {
                        return Color.FromArgb(c.R, c.G, c.B, 0);
                    }
                    else
                if (CutoutMode2 == 1)
                        if (c.R <= R1Value || c.G <= G1Value || c.B <= B1Value)
                        {
                            return Color.FromArgb(c.R, c.G, c.B, 0);
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
                var A = (byte)(Byte.MaxValue * AlphaIntensity);
                Color Result = Color.FromArgb(A, c.R, c.G, c.B);
                return Result;
            }
            return c;
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
