using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.PowerBattery
{
    internal class CategoryInfo : ICategoryInfo
    {
        public string Title => "Power & battery";
        public int Order => 0;
        public string? Icon => "Power24";
    }
}
