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
        public void UpdateView(Bitmap Current)
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(Current);
            Preview.Source = ImgSrc;

        }
        float RValue = 255;
        float GValue = 255;
        float BValue = 255;
        float AValue = 255;
        bool isMixColor = false;
        bool isRGBAIntensity = false;
        bool isBlackAsFullTranparent = false;
        bool isReserveTransparency = false;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, Action action = null)
        {
            RValue = (float)R.Value;
            GValue = (float)G.Value;
            BValue = (float)B.Value;
            AValue = (float)Alpha.Value;
            isMixColor = MixColorGrayscale.IsChecked.Value;
            isReserveTransparency = ReserveTransparency.IsChecked.Value;
            isRGBAIntensity = UseRGBAIntensity.IsChecked.Value;
            isBlackAsFullTranparent = UseBlackAsTransparency.IsChecked.Value;

            ProcessorArguments arguments = new ProcessorArguments(RValue, GValue, BValue, AValue, isMixColor, isRGBAIntensity, isBlackAsFullTranparent, isReserveTransparency);


            Task.Run(() =>
            {
                GrayscaleProcessor.CurrentGrayscaleProcessor.ProcessImage(Processing, OutputBitmap, arguments,
                    () =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpdateView(OutputBitmap);
                            MainWindow.CurrentWindow.UnlockMainArea();
                            if (action is not null) action();
                        });
                    });

                GC.Collect();
            });
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
