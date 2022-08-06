using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Customization
{
    [Feature(Title = "Customize boot logo", Icon = "Image24")]
    internal class CustomizeBootLogo : ActionFeature
    {
        public override bool IsSupported()
        {
            return true;
        }

        public override void OnAction()
        {
            throw new NotImplementedException();
        }
    }
}
