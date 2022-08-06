using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenLenovoSettings
{
    /// <summary>
    /// Interaction logic for SettingCard.xaml
    /// </summary>
    public partial class SettingCard : UserControl
    {
        public SettingCard()
        {
            InitializeComponent();
        }

        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingViewModel vm)
            {
                vm.ActionClicked();
            }
        }
    }
}
