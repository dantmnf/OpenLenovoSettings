using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.CustomizeHandler
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TargetFeatureAttribute : Attribute
    {
        public Type FeatureType { get; set; }
        public TargetFeatureAttribute(Type featureType)
        {
            FeatureType = featureType;
        }
    }
}
