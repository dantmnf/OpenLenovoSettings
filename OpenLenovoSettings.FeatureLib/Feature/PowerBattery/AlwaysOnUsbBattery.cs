using System.ComponentModel.DataAnnotations;

namespace OpenLenovoSettings.Feature.PowerBattery
{
    [Feature(Title = "Always-on USB (battery power)", Icon = "Battery1024", Order = 12)]
    public class AlwaysOnUsbBattery : BaseAcpiVpcCommandMaskFeature
    {
        public AlwaysOnUsbBattery() : base(0x831020E8, 2, 0x4000, 0x8000, 19, 18) { }
    }
}
