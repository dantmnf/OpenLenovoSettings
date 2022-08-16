using Microsoft.Win32;
using OpenLenovoSettings.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.PowerBattery
{

    public enum ChargingModeValue
    {
        Normal,
        Conservation,
        Express,
    }
    [Feature(Title = "Charging mode", Icon = "BatteryCharge24", Order = 0)]
    public class ChargingMode : EnumFeature<ChargingModeValue>
    {
        [Flags]
        private enum DriverFeature : uint
        {
            ExpressModeSupported = 1 << 17,
            ExpressModeEnabled = 0x04,
            ConservativeModeSupported = 1 << 23,
            ConservativeModeEnabled = 0x20,
            NewConservativeModeSupported = 0x4000,
        }
        private DriverFeature GetDriverFeatureMask()
        {
            if (AcpiVpcDrv.IsSupported)
            {
                AcpiVpcDrv.Ioctl(0x831020f8, (sbyte)-1, out DriverFeature featureMask);
                return featureMask;
            }
            return 0;
        }

        public override ChargingModeValue[] GetOptions()
        {
            var feature = GetDriverFeatureMask();
            return GetOptions(feature);
        }

        private static ChargingModeValue[] GetOptions(DriverFeature feature)
        {
            var result = new List<ChargingModeValue> { ChargingModeValue.Normal };
            if (feature.HasFlag(DriverFeature.ConservativeModeSupported)) result.Add(ChargingModeValue.Conservation);
            if (feature.HasFlag(DriverFeature.ExpressModeSupported)) result.Add(ChargingModeValue.Express);
            return result.ToArray();
        }

        public override ChargingModeValue GetValue()
        {
            var feature = GetDriverFeatureMask();
            if (feature.HasFlag(DriverFeature.ExpressModeEnabled)) return ChargingModeValue.Express;
            if (feature.HasFlag(DriverFeature.ConservativeModeEnabled)) return ChargingModeValue.Conservation;
            return ChargingModeValue.Normal;
        }

        public override bool IsSupported()
        {
            return AcpiVpcDrv.IsSupported && GetOptions().Length > 1;
        }

        private static void TrySynchronizeImControllerMode(ChargingModeValue mode)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lenovo\iMController\PluginData\IdeaNotebookPlugin", true);
                if (key != null)
                {
                    var modestr = mode switch
                    {
                        ChargingModeValue.Normal => "Normal",
                        ChargingModeValue.Conservation => "Storage",
                        ChargingModeValue.Express => "Quick",
                        _ => throw new ArgumentOutOfRangeException(nameof(mode))
                    };
                    key.SetValue("BatteryChargeMode", modestr);
                    key.SetValue("ChargeModeSetByUser", 1);
                }
            }
            catch { }
        }

        public override void SetValue(ChargingModeValue value)
        {
            var feature = GetDriverFeatureMask();
            var supportedModes = GetOptions(feature);
            if (!supportedModes.Contains(value)) throw new ArgumentException($"unsupported mode {value}", nameof(value));

            if (feature.HasFlag(DriverFeature.ExpressModeSupported))
            {
                if (value == ChargingModeValue.Express)
                {
                    // enable express mode
                    AcpiVpcDrv.Ioctl(0x831020f8, (byte)7);
                }
                else
                {
                    // disable express mode
                    AcpiVpcDrv.Ioctl(0x831020f8, (byte)8);
                }
            }

            if (feature.HasFlag(DriverFeature.ConservativeModeSupported))
            {
                var newmode = feature.HasFlag(DriverFeature.NewConservativeModeSupported);
                if (value == ChargingModeValue.Conservation)
                {
                    // enable conservative mode
                    AcpiVpcDrv.Ioctl(0x831020f8, (byte)3);
                    if (newmode) AcpiVpcDrv.Ioctl(0x831020f8, (byte)13);
                }
                else
                {
                    // disable conservative mode
                    AcpiVpcDrv.Ioctl(0x831020f8, (byte)5);
                    if (newmode) AcpiVpcDrv.Ioctl(0x831020f8, (byte)15);
                }

            }
            // lenovo services will change mode on start
            TrySynchronizeImControllerMode(value);
        }
    }
}
