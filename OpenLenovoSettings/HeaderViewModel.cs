using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings
{
    internal record class HeaderViewModel(string Title, string Description)
    {
        public string? Icon => null;
        public Visibility DescriptionVisibility => string.IsNullOrEmpty(Description) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsHeader => true;
        public bool IsAction => false;
        public bool IsSwitch => false;
        public bool IsComboBox => false;
    }
}
