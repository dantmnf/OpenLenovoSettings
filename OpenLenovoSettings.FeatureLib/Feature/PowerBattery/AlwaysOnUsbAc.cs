namespace OpenLenovoSettings.Feature.PowerBattery
{
    [Feature(Title = "Always-on USB (AC power)", Icon = "BatteryCharge24", Order = 11)]
    public class AlwaysOnUsbAc : BaseAcpiVpcCommandMaskFeature
    {
        public AlwaysOnUsbAc() : base(0x831020E8, 2, 0x40, 0x80, 10, 11) { }
    }
}
