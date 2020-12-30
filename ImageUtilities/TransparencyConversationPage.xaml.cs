using CLUNL.Imaging;
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
            Bitmap Operating = VariablePool.CurrentBitmap_DownSized;
            if (IsUseDownsize.IsChecked == false)
            {
                Operating = VariablePool.CurrentBitmap;
            }
            Current = new Bitmap(Operating.Width, Operating.Height);
            MainWindow.CurrentWindow.LockMainArea();
            ProcessImage(Operating, Current);

        }
        public void ProcessImage(Bitmap Processing, Bitmap OutputBitmap, Action action = null)
        {
            RValue = (float)R.Value;
            GValue = (float)G.Value;
            BValue = (float)B.Value;
            R1Value = (float)R1.Value;
            G1Value = (float)G1.Value;
            B1Value = (float)B1.Value;
            AValue = (float)Alpha.Value;
            isTransparencyCutout = TransparencyCutout.IsChecked.Value;
            isMixColor = MixColorTransparency.IsChecked.Value;
            CutoutMode1 = CutoutMode.SelectedIndex;
            CutoutMode2 = CutoutModeTC.SelectedIndex;
            ProcessorArguments arguments = new ProcessorArguments(RValue, GValue, BValue, AValue, R1Value, G1Value, B1Value, (float)CutoutMode1, (float)CutoutMode2, isMixColor, isTransparencyCutout);
            Task.Run(() =>
            {
                TransparencyProcessor.CurrentTransparencyProcessor.ProcessImage(Processing, OutputBitmap, arguments, () => {

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
    }
}
