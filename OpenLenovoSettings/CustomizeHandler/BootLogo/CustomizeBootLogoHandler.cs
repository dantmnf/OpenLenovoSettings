using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings.CustomizeHandler.BootLogo
{
    [TargetFeature(typeof(Feature.Customization.CustomizeBootLogo))]
    internal class CustomizeBootLogoHandler : CustomizeHandler
    {
        public override void Invoke()
        {
            var w = new BootLogoWindow();
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();
        }
    }
}
