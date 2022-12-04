using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Keyboard
{
    [Feature(Title = "Cortana shortcut (Fn-Ctrl)", Icon = "PersonVoice24", Order = 2)]
    public class CortanaShortcut : SwitchFeature
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
                using var hkey1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lenovo\lva");
                if (hkey1 != null) return false;
                using var hkey2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lenovo\lvaprcu");
                if (hkey2 != null) return false;
                using var hkey3 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lenovo\lvaw");
                if (hkey3 != null) return false;
                using var hkey4 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lenovo\lvad");
                if (hkey4 != null) return false;
            }
            catch { }
            return true;
        }

        public override bool IsSupported()
        {
            using var svc = OpenService();
            if (svc == null) return false;
            return svc.Status == ServiceControllerStatus.Running;
        }

        public override void SetValue(bool value)
        {
            if (value)
            {
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Lenovo\lva", false);
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Lenovo\lvaprcu", false);
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Lenovo\lvaw", false);
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Lenovo\lvad", false);
            }
            else
            {
                using var hkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Lenovo\lva");
            }
        }
    }
}
