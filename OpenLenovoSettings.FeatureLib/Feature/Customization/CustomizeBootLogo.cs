using OpenLenovoSettings.BootLogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings.Feature.Customization
{
    [Feature(Title = "Customize boot logo", Icon = "Image24")]
    public class CustomizeBootLogo : ActionFeature
    {
        public override bool IsSupported()
        {
            try
            {
                var logoinfo = LogoSetting.ReadLogoInfo();
                return logoinfo.Format.HasFlag(LogoFormat.BMP);
            } catch { }
            return false;
        }

        public override void OnAction()
        {

        }
    }
}
