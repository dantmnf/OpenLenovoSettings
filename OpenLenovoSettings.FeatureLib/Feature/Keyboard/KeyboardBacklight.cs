using OpenLenovoSettings.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Keyboard
{
    public enum KeyboardBacklightLevel
    {
        Off,
        Level1,
        Level2,
        Auto
    }
    [Feature(Title = "Keyboard backlight", Icon = "Keyboard24")]
    public class KeyboardBacklight : EnumFeature<KeyboardBacklightLevel>
    {
        private enum KeyboardBacklightCapability
        {
            Unknown,
            OneLevel,
            TwoLevel,
            TwoLevelAuto,
            LegacyOneLevel,
        }

        private static uint GetKBLCCapability()
        {
            AcpiVpcDrv.Ioctl(0x83102144, 1, out uint caps);
            return caps;
        }

        private static uint GetKblLevels()
        {
            try
            {
                var caps = GetKBLCCapability();
                if (caps != uint.MaxValue)
                {
                    if ((caps & 1) == 1)
                    {
                        return caps >> 1;
                    }
                }
            }
            catch { }
            return uint.MaxValue;
        }

        private static KeyboardBacklightCapability GetCapability()
        {
            if (!AcpiVpcDrv.IsSupported) return KeyboardBacklightCapability.Unknown;
            var levels = GetKblLevels();
            if (levels == 1) return KeyboardBacklightCapability.OneLevel;
            if (levels == 2) return KeyboardBacklightCapability.TwoLevel;
            if (levels == 3) return KeyboardBacklightCapability.TwoLevelAuto;
            if (levels == uint.MaxValue)
            {
                AcpiVpcDrv.Ioctl(0x831020e8, 2, out uint halscap);
                if ((halscap & 0x10) != 0)
                {
                    return KeyboardBacklightCapability.LegacyOneLevel;
                }
            }
            return KeyboardBacklightCapability.Unknown;
        }

        public override bool IsSupported() => AcpiVpcDrv.IsSupported && GetCapability() != KeyboardBacklightCapability.Unknown;

        public override KeyboardBacklightLevel[] GetOptions()
        {
            return GetCapability() switch
            {
                KeyboardBacklightCapability.OneLevel => new[] { KeyboardBacklightLevel.Off, KeyboardBacklightLevel.Level1 },
                KeyboardBacklightCapability.LegacyOneLevel => new[] { KeyboardBacklightLevel.Off, KeyboardBacklightLevel.Level1 },
                KeyboardBacklightCapability.TwoLevel => new[] { KeyboardBacklightLevel.Off, KeyboardBacklightLevel.Level1, KeyboardBacklightLevel.Level2 },
                KeyboardBacklightCapability.TwoLevelAuto => new[] { KeyboardBacklightLevel.Off, KeyboardBacklightLevel.Level1, KeyboardBacklightLevel.Level2, KeyboardBacklightLevel.Auto },
                _ => Array.Empty<KeyboardBacklightLevel>(),
            };
        }

        public override KeyboardBacklightLevel GetValue()
        {
            var cap = GetCapability();
            if (cap == KeyboardBacklightCapability.Unknown) return KeyboardBacklightLevel.Off;
            if (cap == KeyboardBacklightCapability.OneLevel)
            {
                AcpiVpcDrv.Ioctl(0x83102144, 18, out uint status);
                if ((status & 1) == 0) return KeyboardBacklightLevel.Off;
                var level = status >> 1 & 0x7FFF;
                return level switch
                {
                    1 => KeyboardBacklightLevel.Level1,
                    _ => KeyboardBacklightLevel.Off,
                };
            }
            if (cap == KeyboardBacklightCapability.TwoLevel)
            {
                AcpiVpcDrv.Ioctl(0x83102144, 34, out uint status);
                if ((status & 1) == 0) return KeyboardBacklightLevel.Off;
                var level = status >> 1 & 0x7FFF;
                return level switch
                {
                    1 => KeyboardBacklightLevel.Level1,
                    2 => KeyboardBacklightLevel.Level2,
                    _ => KeyboardBacklightLevel.Off,
                };
            }
            if (cap == KeyboardBacklightCapability.TwoLevelAuto)
            {
                AcpiVpcDrv.Ioctl(0x83102144, 50, out uint status);
                if ((status & 1) == 0) return KeyboardBacklightLevel.Off;
                if ((status & 0x10000) == 0) return KeyboardBacklightLevel.Off;
                var level = status >> 1 & 0x7FFF;
                return level switch
                {
                    1 => KeyboardBacklightLevel.Level1,
                    2 => KeyboardBacklightLevel.Level2,
                    3 => KeyboardBacklightLevel.Auto,
                    _ => KeyboardBacklightLevel.Off,
                };
            }
            if (cap == KeyboardBacklightCapability.LegacyOneLevel)
            {
                AcpiVpcDrv.Ioctl(0x831020e8, 2, out uint halscap);
                return (halscap & 0x20) != 0 ? KeyboardBacklightLevel.Level1 : KeyboardBacklightLevel.Off;
            }
            return KeyboardBacklightLevel.Off;
        }



        public override void SetValue(KeyboardBacklightLevel value)
        {
            uint command;
            switch (GetCapability())
            {
                case KeyboardBacklightCapability.OneLevel:
                    command = value switch
                    {
                        KeyboardBacklightLevel.Off => 0x13,
                        KeyboardBacklightLevel.Level1 => 0x10013,
                        _ => throw new ArgumentException($"invalid level {value}", nameof(value))
                    };
                    AcpiVpcDrv.Ioctl(0x83102144, command, out uint _);
                    break;
                case KeyboardBacklightCapability.TwoLevel:
                    command = value switch
                    {
                        KeyboardBacklightLevel.Off => 0x23,
                        KeyboardBacklightLevel.Level1 => 0x10023,
                        KeyboardBacklightLevel.Level2 => 0x20023,
                        _ => throw new ArgumentException($"invalid level {value}", nameof(value))
                    };
                    AcpiVpcDrv.Ioctl(0x83102144, command, out uint _);
                    break;
                case KeyboardBacklightCapability.TwoLevelAuto:
                    command = value switch
                    {
                        KeyboardBacklightLevel.Off => 0x33,
                        KeyboardBacklightLevel.Level1 => 0x10033,
                        KeyboardBacklightLevel.Level2 => 0x20033,
                        KeyboardBacklightLevel.Auto => 0x30033,
                        _ => throw new ArgumentException($"invalid level {value}", nameof(value))
                    };
                    AcpiVpcDrv.Ioctl(0x83102144, command, out uint _);
                    break;
                case KeyboardBacklightCapability.LegacyOneLevel:
                    command = value switch
                    {
                        KeyboardBacklightLevel.Off => 0x09,
                        KeyboardBacklightLevel.Level1 => 0x08,
                        _ => throw new ArgumentException($"invalid level {value}", nameof(value))
                    };
                    AcpiVpcDrv.Ioctl(0x831020e8, command, out uint _);
                    break;
            }
        }
    }
}
