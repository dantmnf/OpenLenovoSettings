using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Keyboard
{
    [Feature(Title = "Caps Lock OSD", Icon = "CursorHover24", Order = 1)]
    public class CapsLockOsd : SwitchFeature
    {
        private const int SVC_CTL_SEND_UI_CAPSLOCK_ON = 141;
        private const int SVC_CTL_SEND_UI_CAPSLOCK_OFF = 142;
        private static ServiceController? OpenService()
        {
            try
            {
                return new ServiceController("LenovoFnAndFunctionKeys");
            }
            catch
            {
                return null;
            }
        }

        public override bool GetValue()
        {
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\LenovoFnAndFunctionKeys\ControlShowOSD");
                if (hkey == null) return false;
                var enabled = (int)hkey.GetValue("ShowCapsNumlkOSD")!;
                return enabled != 0;
            }
            catch
            {
                return false;
            }
        }

        public override bool IsSupported()
        {
            using var svc = OpenService();
            if (svc == null) return false;
            return svc.Status == ServiceControllerStatus.Running;
        }

        public override void SetValue(bool value)
        {
            using var svc = OpenService();
            if (svc == null) return;
            svc.ExecuteCommand(value ? SVC_CTL_SEND_UI_CAPSLOCK_ON : SVC_CTL_SEND_UI_CAPSLOCK_OFF);
        }
    }
}
