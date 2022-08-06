using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    public enum ITSMode
    {
        None,
        Auto,
        Cool,
        Performance,
    }

    [Feature(Title = "Performance mode (ITS service)", Icon = "Gauge24", Order = 0)]
    internal class PerformanceModeITS : EnumFeature<ITSMode>
    {
        const string regkey = @"SYSTEM\CurrentControlSet\Services\LITSSVC\LNBITS\IC\MMC";

        public override bool IsSupported() => ITSService.IsSupported() && GetOptions().Length > 1;

        public override ITSMode[] GetOptions()
        {
            var none = new[] { ITSMode.None };
            var modes = new List<ITSMode>();
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(regkey);
                if (hkey == null) return none;
                var capability = (int)hkey.GetValue("Capability")!;
                if ((capability & 1) == 0)
                    modes.Add(ITSMode.Auto);
                if ((capability & 2) != 0)
                    modes.Add(ITSMode.Cool);
                if ((capability & 8) != 0)
                    modes.Add(ITSMode.Performance);
                if (modes.Count == 0)
                    return none;
                return modes.ToArray();
            }
            catch
            {
                return none;
            }
        }
        private static ITSMode GetModeService()
        {
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(regkey);
                if (hkey == null) return ITSMode.None;
                var capability = (int)hkey.GetValue("Capability")!;
                var automode = (int)hkey.GetValue("AutomaticModeSetting")!;
                if (automode == 2)
                {
                    //var current = (int)hkey.GetValue("CurrentSetting")!;
                    //if (current != 0)
                    //{
                    return ITSMode.Auto;
                    //}
                }
                else if (automode == 1)
                {
                    var current = (int)hkey.GetValue("CurrentSetting")!;
                    return current switch
                    {
                        1 => ITSMode.Cool,
                        3 => ITSMode.Performance,
                        _ => ITSMode.None,
                    };
                }
            }
            catch { }
            return ITSMode.None;
        }


        private static void SetModeService(ITSMode mode)
        {
            var controlcode = mode switch
            {
                ITSMode.None => 0x86,
                ITSMode.Auto => 0x87,
                ITSMode.Cool => 0x92,
                ITSMode.Performance => 0x94,
                _ => throw new ArgumentException("invalid mode", nameof(mode))
            };
            using var svc = ITSService.OpenService();
            if (svc != null)
                svc.ExecuteCommand(controlcode);
        }

        public override ITSMode GetValue()
        {
            return GetModeService();
        }

        public override void SetValue(ITSMode value)
        {
            SetModeService(value);
        }


    }
}
