using OpenLenovoSettings.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class FeatureAttribute : Attribute
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public int Order { get; set; } = 0;
        public string? Icon { get; set; }
    }
}
