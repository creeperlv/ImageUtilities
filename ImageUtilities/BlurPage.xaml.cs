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

namespace ImageUtilities
{
    /// <summary>
    /// Interaction logic for BlurPage.xaml
    /// </summary>
    public partial class BlurPage : UserControl
    {
        public BlurPage()
        {
            InitializeComponent();
        }

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
            MainWindow.CurrentWindow.LockMainArea();
            ProcessImage(Operating, Current);
        }
        Bitmap Current;
        float RadiusValue;
        float PixelSkipCount;
        float SamplePixelSkipCount;
        int BlurMode = 0;
        int AccelerationMode = 0;
        bool isRoundRange;
        bool useWeight=false;
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, Action action = null)
        {
            RadiusValue= (float)Radius.Value;
            PixelSkipCount = (float)PixelSkip.Value + 1;
            SamplePixelSkipCount = (float)SamplePixelSkip.Value + 1;
            AccelerationMode = ComputeMode.SelectedIndex;
            BlurMode = BlurModeSelector.SelectedIndex;
            if (SamplePixelSkipCount > RadiusValue)
            {
                SamplePixelSkipCount = RadiusValue;
            }
            isRoundRange = RoundRange.IsChecked.Value;
            useWeight = UseWeightedSample.IsChecked.Value;
            ProcessorArguments arguments = new ProcessorArguments(RadiusValue,PixelSkipCount, SamplePixelSkipCount,BlurMode, AccelerationMode, isRoundRange,useWeight);
            Task.Run(() =>
            {
                BlurProcessor.CurrentBlurProcessor.ProcessImage(Processing, OutputBitmap, arguments, () => {

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
            Preview.Source = ImgSrc;

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Bitmap Operating = VariablePool.CurrentBitmap;
            Current = new Bitmap(Operating.Width, Operating.Height);
            MainWindow.CurrentWindow.LockMainArea();
            ProcessImage(Operating, VariablePool.CurrentBitmap, () =>
            {
                MainWindow.CurrentWindow.UpdatePreview();
                MainWindow.CurrentWindow.DownSizeAll();
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentWindow.ToBlankPage();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in VariablePool.GPUs)
            {
                ComputeMode.Items.Add(new ComboBoxItem() { Content= item });
            }
        }
    }
}
