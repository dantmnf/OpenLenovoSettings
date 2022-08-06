using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{
    internal record class SettingPageViewModel(string Title, IFeatureItem[] Features);
}
