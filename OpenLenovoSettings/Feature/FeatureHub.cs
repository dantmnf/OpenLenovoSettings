using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature
{
    internal class FeatureHub
    {
        public static event Action? ReloadRequested;

        public static void RequestReloadFeatures()
        {
            ReloadRequested?.Invoke();
        }
    }
}
