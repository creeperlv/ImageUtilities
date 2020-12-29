using Microsoft.Win32;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
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
                        if (VariablePool.CurrentBitmap_Original is not null)
                            VariablePool.CurrentBitmap_Original.Dispose();
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
                    VariablePool.CurrentBitmap_Original = new Bitmap(VariablePool.OriginalImage);
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
            if (VariablePool.CurrentBitmap is null) {
                ShowDialog("Error", "There's nothing to save.");
                return; }
            VariablePool.CurrentBitmap.Save(VariablePool.CurrentFile, ImageFormat.Png);
            ShowDialog("Done","Image have been saved to:"+VariablePool.CurrentFile);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (VariablePool.CurrentBitmap is null)
            {
                ShowDialog("Error", "There's nothing to save.");
                return;
            }
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.FileName = VariablePool.CurrentFile;
            if (sfd.ShowDialog() == true)
            {
                if (!File.Exists(sfd.FileName))
                    File.Create(sfd.FileName).Close();
                VariablePool.CurrentBitmap.Save(sfd.FileName, ImageFormat.Png);
                VariablePool.CurrentFile = sfd.FileName;
                ShowDialog("Done", "Image have been saved to:" + VariablePool.CurrentFile);
            }
        }
        public void ShowDialog(string Title,string Message)
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
    }
}
