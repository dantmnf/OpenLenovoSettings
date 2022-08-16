using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{
    public class FeatureHub
    {
        public static event Action? ReloadRequested;

        private static ConcurrentDictionary<Type, IFeatureItem> _features = new();
        private static ConcurrentDictionary<Type, bool> _supported = new();

        private class CacheSupportedFeatureProxy : IFeatureItem
        {
            private IFeatureItem feature;
            internal CacheSupportedFeatureProxy(IFeatureItem feature)
            {
                while (feature is CacheSupportedFeatureProxy x)
                {
                    // unfold nested CacheSupportedFeatureProxy
                    feature = x.feature;
                }
                this.feature = feature;
            }

            public object[] GetOptions() => feature.GetOptions();
            public object? GetValue() => feature.GetValue();

            public Type GetValueType() => feature.GetValueType();

            public bool IsSupported()
            {
                return _supported.GetOrAdd(feature.GetType(), _ => feature.IsSupported());
            }
            public void SetValue(object? value) => feature.SetValue(value);
            public Type GetRealType() => feature.GetRealType();
        }

        public static IFeatureItem GetFeatureInstance(Type t)
        {
            return _features.GetOrAdd(t, (t) => new CacheSupportedFeatureProxy((IFeatureItem)Activator.CreateInstance(t)!));
        }

        public static void RequestReloadFeatures()
        {
            _supported.Clear();
            ReloadRequested?.Invoke();
        }
    }
}
