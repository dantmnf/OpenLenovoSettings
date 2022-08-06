using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature
{
    internal abstract class BaseAcpiVpcCommandMaskFeature : SwitchFeature
    {

        private uint GetDriverFeatureMask()
        {
            if (AcpiVpcDrv.IsSupported)
            {
                AcpiVpcDrv.Ioctl(controlCode, getMaskCommand, out uint featureMask);
                return featureMask;
            }
            return 0;
        }

        private readonly uint controlCode;
        private readonly byte getMaskCommand;
        private readonly uint supportedMask;
        private readonly uint enabledMask;
        private readonly byte enableCommand;
        private readonly byte disableCommand;

        protected BaseAcpiVpcCommandMaskFeature(uint controlCode, byte getMaskCommand, uint supportedMask, uint enabledMask, byte enableCommand, byte disableCommand)
        {
            this.controlCode = controlCode;
            this.getMaskCommand = getMaskCommand;
            this.supportedMask = supportedMask;
            this.enabledMask = enabledMask;
            this.enableCommand = enableCommand;
            this.disableCommand = disableCommand;
        }

        public override bool IsSupported() => AcpiVpcDrv.IsSupported && (GetDriverFeatureMask() & supportedMask) != 0;

        public override bool GetValue() => (GetDriverFeatureMask() & enabledMask) != 0;

        public override void SetValue(bool value)
        {
            AcpiVpcDrv.Ioctl(controlCode, value ? enableCommand : disableCommand, out uint _);
        }
    }
}
