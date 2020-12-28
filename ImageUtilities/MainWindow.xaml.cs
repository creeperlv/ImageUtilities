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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                    UpdatePreview();
                    GC.Collect();
                }
                catch (Exception)
                {
                }
            }
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

        private void Rectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var x = (e.NewSize.Width / 100.0);
            var y = (e.NewSize.Height / 100.0);
            BG.Viewport = new Rect(0, 0, (1.0 / x), (1.0 / y));
        }
    }
}
