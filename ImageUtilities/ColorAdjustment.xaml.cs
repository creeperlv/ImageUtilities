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
    /// Interaction logic for ColorAdjustment.xaml
    /// </summary>
    public partial class ColorAdjustment : UserControl
    {
        public ColorAdjustment()
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
            Current = new (Operating.Width, Operating.Height);
            MainWindow.CurrentWindow.LockMainArea();
            ProcessImage(Operating, Current);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Bitmap Operating = VariablePool.CurrentBitmap;
            Current = new (Operating.Width, Operating.Height);
            MainWindow.CurrentWindow.LockMainArea();
            ProcessImage(Operating, VariablePool.CurrentBitmap, () =>
            {
                MainWindow.CurrentWindow.UpdatePreview();
                MainWindow.CurrentWindow.DownSizeAll();
            });
        }
        float CB_RValue = 255;
        float CB_GValue = 255;
        float CB_BValue = 255;
        float CB_AValue = 255;
        float BrightIntensity = 1;
        float CIntensity = 1;
        float RIntensity = 1;
        float GIntensity = 1;
        float BIntensity = 1;
        float AIntensity = 1;
        int ColorBlendMode;
        bool isInventColor = false;
        bool WillAdjustBrightness = false;
        bool WillScaleBrightness = false;
        bool WillPerformColorBlend = false;
        bool WillPerformColorBlend_IgnoreTransparency = false;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, Action action = null)
        {
            CB_RValue = (float)CB_R.Value;
            CB_GValue = (float)CB_G.Value;
            CB_BValue = (float)CB_B.Value;
            CB_AValue = (float)CB_Alpha.Value;
            BrightIntensity = (float)Brightness.Value;
            CIntensity = (float)Contrast.Value;
            RIntensity = GetIntensity(CB_RValue);
            GIntensity = GetIntensity(CB_GValue);
            BIntensity = GetIntensity(CB_BValue);
            AIntensity = GetIntensity(CB_AValue);
            ColorBlendMode = ColorBlendModeSelector.SelectedIndex;
            isInventColor = InventColor.IsChecked.Value;
            WillAdjustBrightness = AdjustBrightness.IsChecked.Value;
            WillScaleBrightness = ScaleBrightness.IsChecked.Value;
            WillPerformColorBlend = ColorBlend.IsChecked.Value;
            WillPerformColorBlend_IgnoreTransparency = IgnoreTransparency.IsChecked.Value;
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
                    MainWindow.CurrentWindow.UnlockMainArea();
                    if (action is not null) action();
                });
                GC.Collect();
            });
        }
        public void UpdateView(Bitmap Current)
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(Current);
            PreviewImage.Source = ImgSrc;

        }
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
                if(WillPerformColorBlend_IgnoreTransparency==false)
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
