using OpenLenovoSettings.Feature;
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

namespace OpenLenovoSettings.Pages
{
    /// <summary>
    /// Interaction logic for DeviceSettingsPage.xaml
    /// </summary>
    public partial class DeviceSettingsPage : Page
    {
        public DeviceSettingsPage()
        {
            InitializeComponent();
        }

        private void Page_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is not SettingPageViewModel context) return;
            Task.Run(() =>
            {
                var anysupported = false;
                var settings = new List<object>();
                foreach (var (feature, attr) in context.Features)
                {
                    try
                    {
                        if (feature.IsSupported())
                        {
                            anysupported = true;
                            var vm = new SettingViewModel(feature, attr);
                            vm.OnSettingChanged += (sender, value) =>
                            {
                                Task.Run(() =>
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        vm.IsApplyInProgress = true;
                                    });
                                    sender.SetValue(value);
                                    Dispatcher.Invoke(() =>
                                    {
                                        vm.IsApplyInProgress = false;
                                        vm.FireSettingChanged();
                                    });
                                });
                            };

                            settings.Add(vm);
                        }
                    }
                    catch { }
                }
                if (anysupported)
                {
                    Dispatcher.Invoke(() =>
                    {
                        itemsCtl.ItemsSource = settings;
                    });
                }
            });

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Page_DataContextChanged(this, default);
            FeatureHub.ReloadRequested += OnFeatureReload;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            FeatureHub.ReloadRequested -= OnFeatureReload;
        }

        private void OnFeatureReload()
        {
            Dispatcher.Invoke(() =>
            {
                Page_DataContextChanged(this, default);
            });
        }
    }
}
