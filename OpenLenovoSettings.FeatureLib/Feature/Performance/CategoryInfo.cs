using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    public class CategoryInfo : ICategoryInfo
    {
        public string Title => "Performance";
        public int Order => 1;
        public string? Icon => "Gauge24";
    }
}
