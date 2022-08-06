using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings.Feature.Customization
{
    [Feature(Title = "Customize boot logo", Icon = "Image24")]
    internal class CustomizeBootLogo : ActionFeature
    {
        public override bool IsSupported()
        {
            try
            {
                var logoinfo = BootLogo.LogoSetting.ReadLogoInfo();
                return logoinfo.Format.HasFlag(BootLogo.LogoFormat.BMP);
            } catch { }
            return false;
        }

        public override void OnAction()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var w = new BootLogoWindow();
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
            });
        }
    }
}
