using System;
using System.Collections.Generic;
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
using System.IO;
using Wpf.Ui.Controls;
using System.ComponentModel;
using Microsoft.Win32;
using OpenLenovoSettings.BootLogo;

namespace OpenLenovoSettings
{
    /// <summary>
    /// Interaction logic for BootLogoWindow.xaml
    /// </summary>
    public partial class BootLogoWindow
    {
        public BootLogoWindow()
        {
            InitializeComponent();
        }

        
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            Task.Run(ReloadSettings);
        }

        private void ReloadSettings()
        {
            var info = LogoSetting.ReadLogoInfo();
            string? logofile = null;
            if (info.Enabled != 0 && LogoSetting.EfiVolume != null)
            {
                var logocrc = LogoSetting.ReadLogoCrc();
                var logofiles = new List<string>();
                var prefix = Path.Combine(LogoSetting.EfiVolume, "EFI", "Lenovo", "Logo", $"mylogo_{info.Width}x{info.Height}");
                if (info.Format.HasFlag(LogoFormat.JPG))
                {
                    logofiles.Add(prefix + ".jpg");
                }
                if (info.Format.HasFlag(LogoFormat.BMP))
                {
                    logofiles.Add(prefix + ".bmp");
                }
                if (info.Format.HasFlag(LogoFormat.PNG))
                {
                    logofiles.Add(prefix + ".png");
                }

                foreach (var file in logofiles)
                {
                    try
                    {
                        var filecrc = LogoSetting.GetLogoFileCrc(file);
                        if (filecrc == logocrc)
                        {
                            logofile = file;
                            break;
                        }
                    }
                    catch (IOException) { }
                }
            }
            Stream? logoFileStream = null;
            if (logofile != null)
            {
                logoFileStream = new MemoryStream(File.ReadAllBytes(logofile));
            }
            Dispatcher.Invoke(() =>
            {
                if (logofile != null)
                {
                    defaultImageLabel.Visibility = Visibility.Collapsed;
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = logoFileStream;
                    bi.EndInit();
                    logoImage.Source = bi;

                    var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                    logoImage.LayoutTransform = new ScaleTransform(bi.DpiX / 96 / m.M11, bi.DpiY / 96 / m.M22);
                }
                else
                {
                    defaultImageLabel.Visibility = Visibility.Visible;
                    logoImage.Source = null;
                }
                imageWidth.Text = info.Width.ToString();
                imageHeight.Text = info.Height.ToString();
                formats.Text = LogoSetting.LogoFormatToString(info.Format);
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(()=>
            {
                LogoSetting.ResetLogo();
                ReloadSettings();
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var filters = new List<string>();
            var info = LogoSetting.ReadLogoInfo();
            filters.Add("*.bmp");
            filters.Add("*.png");
            filters.Add("*.jpg");
            filters.Add("*.jpeg");
            dlg.Filter = string.Join(", ", filters) + "|" + string.Join(";", filters);
            if (dlg.ShowDialog(this).GetValueOrDefault())
            {
                var filename = dlg.FileName;
                LogoSetting.SetLogoFile(filename);
                ReloadSettings();
            }
        }
    }
}
