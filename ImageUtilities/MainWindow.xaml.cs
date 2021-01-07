using CLUNL.Imaging;
using CLUNL.Imaging.GPUAcceleration;
using Microsoft.Win32;
using OpenCL.NetCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ImageUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow CurrentWindow;
        public MainWindow()
        {
            InitializeComponent();
            CurrentWindow = this;
        }
        public void LockMainArea()
        {
            MainArea.IsEnabled = false;
        }
        public void UnlockMainArea()
        {
            MainArea.IsEnabled = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    try
                    {
                        if (VariablePool.OriginalImage is not null)
                            VariablePool.OriginalImage.Dispose();
                        if (VariablePool.CurrentBitmap is not null)
                            VariablePool.CurrentBitmap.Dispose();
                        if (VariablePool.CurrentBitmap_DownSized is not null)
                            VariablePool.CurrentBitmap_DownSized.Dispose();
                    }
                    catch (Exception)
                    {
                    }
                    VariablePool.OriginalImage = Bitmap.FromFile(openFileDialog.FileName);
                    VariablePool.CurrentBitmap = new Bitmap(VariablePool.OriginalImage);
                    if (VariablePool.CurrentBitmap.Width > 1024 && VariablePool.CurrentBitmap.Height > 1024)
                        VariablePool.CurrentBitmap_DownSized = Utilities.DownSize10X(VariablePool.CurrentBitmap);
                    else
                    if (VariablePool.CurrentBitmap.Width > 512 && VariablePool.CurrentBitmap.Height > 512)
                        VariablePool.CurrentBitmap_DownSized = Utilities.DownSize5X(VariablePool.CurrentBitmap);
                    else VariablePool.CurrentBitmap_DownSized = new Bitmap(VariablePool.OriginalImage);
                    //VariablePool.CurrentBitmap_Original = new Bitmap(VariablePool.OriginalImage);
                    VariablePool.CurrentFile = openFileDialog.FileName;
                    VariablePool.OriginalImage.Dispose();
                    UpdatePreview();
                    GC.Collect();
                }
                catch (Exception exception)
                {
                    ShowDialog("Error", "Cannot open file:" + exception);
                }
            }
        }
        public void DownSizeAll()
        {
            try
            {
                if (VariablePool.CurrentBitmap_DownSized is not null)
                    VariablePool.CurrentBitmap_DownSized.Dispose();
            }
            catch (Exception)
            {
            }
            if (VariablePool.CurrentBitmap.Width > 1024 && VariablePool.CurrentBitmap.Height > 1024)
                VariablePool.CurrentBitmap_DownSized = Utilities.DownSize10X(VariablePool.CurrentBitmap);
            else if (VariablePool.CurrentBitmap.Width > 512 && VariablePool.CurrentBitmap.Height > 512)
                VariablePool.CurrentBitmap_DownSized = Utilities.DownSize5X(VariablePool.CurrentBitmap);
            else VariablePool.CurrentBitmap_DownSized = VariablePool.CurrentBitmap.Clone(new System.Drawing.Rectangle(0, 0, VariablePool.CurrentBitmap.Width, VariablePool.CurrentBitmap.Height), VariablePool.CurrentBitmap.PixelFormat);
        }
        public void UpdatePreview()
        {
            var ImgSrc = Utilities.ImageSourceFromBitmap(VariablePool.CurrentBitmap);
            PreviewImage.Source = (ImageSource)ImgSrc;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new TransparencyConversationPage());
            System.GC.Collect();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new Grayscale());
            System.GC.Collect();
        }

        private void Rectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var x = (e.NewSize.Width / 100.0);
            var y = (e.NewSize.Height / 100.0);
            BG.Viewport = new Rect(0, 0, (1.0 / x), (1.0 / y));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (VariablePool.CurrentBitmap is null)
            {
                ShowDialog("Error", "There's nothing to save.");
                return;
            }
            SaveFile(VariablePool.CurrentFile);
            ShowDialog("Done", "Image have been saved to:" + VariablePool.CurrentFile);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (VariablePool.CurrentBitmap is null)
            {
                ShowDialog("Error", "There's nothing to save.");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = VariablePool.CurrentFile;
            if (sfd.ShowDialog() == true)
            {
                if (!File.Exists(sfd.FileName))
                    File.Create(sfd.FileName).Close();
                SaveFile(sfd.FileName);
                VariablePool.CurrentFile = sfd.FileName;
                ShowDialog("Done", "Image have been saved to:" + VariablePool.CurrentFile);
            }
        }
        public void SaveFile(string FileName)
        {
            ImageFormat TargetFormat = ImageFormat.Png;
            FileInfo fi = new FileInfo(FileName);
            if (fi.Extension.ToUpper() == ".PNG")
                TargetFormat = ImageFormat.Png;
            else
            if (fi.Extension.ToUpper() == ".BMG")
                TargetFormat = ImageFormat.Bmp;
            else
            if (fi.Extension.ToUpper() == ".JPG" || fi.Extension.ToUpper() == ".JPEG")
                TargetFormat = ImageFormat.Jpeg;
            else
            if (fi.Extension.ToUpper() == ".ICO" || fi.Extension.ToUpper() == ".ICON")
                TargetFormat = ImageFormat.Icon;
            VariablePool.CurrentBitmap.Save(FileName, TargetFormat);
        }
        public void ShowDialog(string Title, string Message)
        {
            DialogTitle.Text = Title;
            DialogContent.Text = Message;
            MainArea.IsEnabled = false;
            DialogLayer.Visibility = Visibility.Visible;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MainArea.IsEnabled = true;
            DialogLayer.Visibility = Visibility.Collapsed;
        }
        public void ToBlankPage()
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new BlankPage());
            GC.Collect();
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new ColorAdjustment());
            GC.Collect();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var gpus = CommonGPUAcceleration.EnumerateGPUs();
            foreach (var item in gpus)
            {
                ErrorCode ec;
                string Name = Cl.GetDeviceInfo(item, DeviceInfo.Name, out ec)
                    + "," +
                    Cl.GetPlatformInfo(Cl.GetDeviceInfo(item, DeviceInfo.Platform, out ec).CastTo<Platform>(), PlatformInfo.Name, out ec) +
                     "," +
                    Cl.GetPlatformInfo(Cl.GetDeviceInfo(item, DeviceInfo.Platform, out ec).CastTo<Platform>(), PlatformInfo.Version, out ec);
                VariablePool.GPUs.Add(Name);
            }
            ToBlankPage();
            //            CommonGPUAcceleration.SetGPU(0);
            //            ShowDialog("OpenCL", ""+VariablePool.GPUs[1]);
            //            var Kernel = CommonGPUAcceleration.Compile(BlurProcessor.BlurProgram, "ProcessImage");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new AboutPage());
            GC.Collect();
        }
        public void UpdateProgressDescriptionText(string text)
        {
            if (text is not null)
                WorkDescription.Text = text;
        }
        public void ShowProgressOverlay()
        {
            ProgressBarOverlay.Visibility = Visibility.Visible;
        }
        public void HideProgressOverlay()
        {
            ProgressBarOverlay.Visibility = Visibility.Collapsed;
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            CentralFrame.Children.Clear();
            CentralFrame.Children.Add(new BlurPage());
            GC.Collect();
        }
    }
}
