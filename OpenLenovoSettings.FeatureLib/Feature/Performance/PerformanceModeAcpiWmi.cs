using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    [Feature(Title = "Performance mode (ACPI WMI)", Icon = "Gauge24", Order = 0)]
    public class PerformanceModeAcpiWmi : EnumFeature<ITSMode>
    {
        public override ITSMode[] GetOptions() => new[] { ITSMode.Auto, ITSMode.Cool, ITSMode.Performance };
        public override bool IsSupported()
        {
            try
            {
                return GetValue() != ITSMode.None;
            }
            catch { }
            return false;
        }
        public override ITSMode GetValue()
        {
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var objectQuery = new ObjectQuery("SELECT * FROM LENOVO_GAMEZONE_DATA");
            using var searcher = new ManagementObjectSearcher(scope, objectQuery).Get();
            var obj = searcher.Cast<object>().FirstOrDefault();
            if (obj is ManagementObject mo)
            {
                using (mo)
                {
                    var mode = Convert.ToInt32(mo.InvokeMethod("GetSmartFanMode", null, null)?.Properties["Data"].Value);
                    return mode switch
                    {
                        1 => ITSMode.Cool,
                        2 => ITSMode.Auto,
                        3 => ITSMode.Performance,
                        _ => ITSMode.None,
                    };
                }
            }
            else
            {
                return ITSMode.None;
            }
        }

        public override void SetValue(ITSMode value)
        {
            var mode = value switch
            {
                ITSMode.Auto => 2,
                ITSMode.Cool => 1,
                ITSMode.Performance => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var objectQuery = new ObjectQuery("SELECT * FROM LENOVO_GAMEZONE_DATA");
            using var searcher = new ManagementObjectSearcher(scope, objectQuery).Get();
            var obj = searcher.Cast<object>().FirstOrDefault();
            if (obj is ManagementObject mo)
            {
                using (mo)
                {
                    var param = mo.GetMethodParameters("SetSmartFanMode");
                    param["Data"] = mode;
                    mo.InvokeMethod("GetSmartFanMode", param, null);
                }
            }
        }
    }
}
