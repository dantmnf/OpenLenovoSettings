using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.CustomizeHandler
{
    internal abstract class CustomizeHandler
    {
        public abstract void Invoke();

        private static Lazy<Dictionary<Type, CustomizeHandler>> handlers = new(PopulateHandlers);

        private static Dictionary<Type, CustomizeHandler> PopulateHandlers()
        {
            var result = new Dictionary<Type, CustomizeHandler>();

            var handlertypes = typeof(CustomizeHandler).Assembly.GetTypes().Where(x => x.IsDefined(typeof(TargetFeatureAttribute), false)).ToArray();
            foreach (var handlertype in handlertypes)
            {
                var targetFeature = handlertype.GetCustomAttributes(typeof(TargetFeatureAttribute), false).FirstOrDefault() as TargetFeatureAttribute;
                if (targetFeature != null)
                {
                    result.Add(targetFeature.FeatureType, (CustomizeHandler)Activator.CreateInstance(handlertype)!);
                }
            }
            return result;
        }

        public static CustomizeHandler? GetHandlerForFeature(Type featureType)
        {
            return handlers.Value.GetValueOrDefault(featureType);
        }
    }
}
