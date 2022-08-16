using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings
{
    [Display(Name = "name", Description = "description")]
    internal class DesignTimeFeature : IFeatureItem
    {
        public object[] GetOptions() => new object[] { Visibility.Visible, Visibility.Collapsed, Visibility.Hidden };

        public object? GetValue() => Visibility.Visible;

        public Type GetValueType() => typeof(Action);

        public bool IsSupported() => true;

        public void SetValue(object? value)
        {
            throw new NotImplementedException();
        }
    }

    internal class DesignTimeSettingViewModel : SettingViewModel
    {
        public DesignTimeSettingViewModel() : base(new DesignTimeFeature()) { }
    }
}
