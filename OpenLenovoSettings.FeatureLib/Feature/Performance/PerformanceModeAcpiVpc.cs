using OpenLenovoSettings.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    [Feature(Title = "Performance mode (ACPI VPC)", Icon = "Gauge24", Order = 0)]
    public class PerformanceModeAcpiVpc : EnumFeature<ITSMode>
    {
        private static uint BuildDytcCommand(ushort a1, byte a2, byte a3, byte a4)
        {
            return a1 & 0x1FFu | (a2 & 0xFu | 16 * (a3 & 0xFu | 16 * (a4 & 1u))) << 12;
        }

        public override ITSMode[] GetOptions() => new[] { ITSMode.Auto, ITSMode.Cool, ITSMode.Performance };
        public override bool IsSupported()
        {
            if (!AcpiVpcDrv.IsSupported) return false;
            if (ITSService.IsSupported()) return false;
            try
            {
                AcpiVpcDrv.Ioctl(0x8310213C, BuildDytcCommand(2, 0, 0, 0), out uint x);
                return true;
            }
            catch { }
            return false;
        }
        public override ITSMode GetValue()
        {
            AcpiVpcDrv.Ioctl(0x8310213C, BuildDytcCommand(7, 0, 0, 0), out uint mask);
            var mode = mask >> 16;

            return mode switch
            {
                2 => ITSMode.Performance,
                3 => ITSMode.Cool,
                15 => ITSMode.Auto,
                _ => ITSMode.None,
            };
        }

        public override void SetValue(ITSMode value)
        {
            var cmd = value switch
            {
                ITSMode.Auto => BuildDytcCommand(1, 11, 15, 0),
                ITSMode.Cool => BuildDytcCommand(1, 11, 3, 1),
                ITSMode.Performance => BuildDytcCommand(1, 11, 2, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
            AcpiVpcDrv.Ioctl(0x8310213C, cmd);

        }
    }
}
