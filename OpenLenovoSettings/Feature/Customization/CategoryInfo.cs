using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Customization
{
    internal class CategoryInfo : ICategoryInfo
    {
        public string Title => "Customization";
        public int Order => 9;
        public string? Icon => "PaintBrush24";
    }
}
