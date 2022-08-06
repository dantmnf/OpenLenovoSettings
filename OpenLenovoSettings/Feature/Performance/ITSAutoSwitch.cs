using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    [Feature(Title = "Performance mode auto-switch (ITS service)", Description = "Based on foreground application", Icon = "Gauge24", Order = 1)]
    internal class ITSAutoSwitch : SwitchFeature
    {
        const string regkey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LITSSVC\LNBITS\IC\AUTODETECT";

        public override bool IsSupported() => ITSService.IsSupported() && ITSService.GetItsServiceCapability() >= ITSService.ITS_VERSION_5;

        public override bool GetValue()
        {
            try
            {
                var state = (int)Registry.GetValue(regkey, "Capability", 0)!;
                return (state & 2) != 0;
            }
            catch
            {
                return false;
            }
        }

        public override void SetValue(bool value)
        {
            using var svc = ITSService.OpenService();
            if (svc != null)
            {
                svc.ExecuteCommand(value ? 0x9A : 0x9B);
            }
        }
    }
}
