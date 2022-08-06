using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Keyboard
{
    [Feature(Title = "Fn lock", Icon = "KeyboardShiftUppercase24")]
    internal class FnLock : BaseAcpiVpcCommandMaskFeature
    {
        public FnLock() : base(0x831020E8, 2, 0x200, 0x400, 14, 15) { }
    }
}
