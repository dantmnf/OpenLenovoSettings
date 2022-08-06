using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Keyboard
{
    internal class CategoryInfo : ICategoryInfo
    {
        public string Title => "Keyboard";
        public int Order => 2;
        public string? Icon => "Keyboard24";
    }
}
