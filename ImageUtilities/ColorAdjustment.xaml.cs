using CLUNL.Imaging;
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
        int ColorBlendMode=0;
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
            ColorBlendMode = ColorBlendModeSelector.SelectedIndex;
            isInventColor = InventColor.IsChecked.Value;
            WillAdjustBrightness = AdjustBrightness.IsChecked.Value;
            WillScaleBrightness = ScaleBrightness.IsChecked.Value;
            WillPerformColorBlend = ColorBlend.IsChecked.Value;
            WillPerformColorBlend_IgnoreTransparency = IgnoreTransparency.IsChecked.Value;

            ProcessorArguments arguments = new(CB_RValue, CB_GValue, CB_BValue, CB_AValue, BrightIntensity, CIntensity, ColorBlendMode, isInventColor, WillAdjustBrightness, WillScaleBrightness, WillPerformColorBlend, WillPerformColorBlend_IgnoreTransparency);
            
            Task.Run(() =>
            {
                ColorAdjustmentProcessor.CurrentColorAdjustmentProcessor.ProcessImage(Processing, OutputBitmap, arguments, () => {
                    Dispatcher.Invoke(() =>
                    {
                        UpdateView(OutputBitmap);
                        MainWindow.CurrentWindow.UnlockMainArea();
                        if (action is not null) action();
                    });
                    GC.Collect();
                });
            });
        }
        public void UpdateView(Bitmap Current)
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(Current);
            PreviewImage.Source = ImgSrc;

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
