using OpenLenovoSettings.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Wpf.Ui.Controls;

namespace OpenLenovoSettings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private (ICategoryInfo catinfo, (IFeatureItem feature, FeatureAttribute attr)[])[]? categories = null;

        private void PopulateFeaturesAndCategories()
        {
            var featurelib = typeof(IFeatureItem).Assembly;
            var featureGroups = featurelib.GetTypes()
                .Select(t => (Type: t, Attr: t.GetCustomAttributes(typeof(FeatureAttribute), false).FirstOrDefault() as FeatureAttribute))
                .Where((t) => t.Attr != null)
                .OrderBy(x => x.Attr!.Order).ThenBy(x => x.Attr!.Title)
                .GroupBy(x=>x.Type.Namespace ?? x.Type.Name).Select(x=>(x.Key, x.ToArray()))
                .ToArray()!;

            var result = new List<(ICategoryInfo, (IFeatureItem, FeatureAttribute)[])>();

            foreach (var (ns, features) in featureGroups)
            {
                ICategoryInfo? catinfo = null;
                var infotype = featurelib.GetType(ns + ".CategoryInfo");
                if (infotype != null)
                {
                    catinfo = Activator.CreateInstance(infotype) as ICategoryInfo;
                }
                if (catinfo == null)
                {
                    catinfo = new DefaultCategoryInfo(ns, 0, "Question24");
                }

                var instances = new List<(IFeatureItem, FeatureAttribute)>();
                foreach (var (feature_t, attr) in features)
                {
                    var instance = FeatureHub.GetFeatureInstance(feature_t);
                    if (instance != null)
                    {
                        instances.Add((instance!, attr!));
                    }
                }
                result.Add((catinfo, instances.ToArray()));
            }
            categories = result.OrderBy(x => x.Item1.Order).ThenBy(x => x.Item1.Title).ToArray();
        }

        private void RenderCategories()
        {
            if (categories == null) PopulateFeaturesAndCategories();
            rootNavigation.Items.Clear();
            foreach (var (catinfo, features) in categories!)
            {
                Wpf.Ui.Common.SymbolRegular icon = Wpf.Ui.Common.SymbolRegular.Question24;
                Enum.TryParse(catinfo.Icon, true, out icon);
                var navitem = new NavigationItem
                {
                    Content = catinfo.Title,
                    Icon = icon,
                    PageTag = catinfo.ToString(),
                    PageType = typeof(Page),
                    Cache = true,
                };
                var dc = new SettingPageViewModel(catinfo.Title, features);
                //navitem.Click += (s, e) =>
                //{
                //    if (((NavigationItem)s).IsActive)
                //    {
                //        e.Handled = true;
                //    }
                //};
                navitem.Activated += (s, e) =>
                {
                    rootFrame.Navigate(new Pages.DeviceSettingsPage() { DataContext = dc });
                };
                rootNavigation.Items.Add(navitem);
            }

        }

        private void UpdateCategoryVisibility()
        {
            if (categories == null) RenderCategories();
            var firstvisible = -1;
            for (int i = 0; i < categories!.Length; i++)
            {
                var category = categories[i];
                var anysupported = false;
                foreach (var (feature, attr) in category.Item2)
                {
                    try
                    {
                        if (feature.IsSupported())
                        {
                            anysupported = true;
                        }
                    } catch { }
                }
                if (anysupported)
                {
                    if (firstvisible == -1)
                        firstvisible = i;
                    ((NavigationItem)rootNavigation.Items[i]).Visibility = Visibility.Visible;
                }
                else
                {
                    ((NavigationItem)rootNavigation.Items[i]).Visibility = Visibility.Collapsed;

                }
                if ((rootNavigation.Current as NavigationItem)?.Visibility != Visibility.Visible)
                {
                    if (firstvisible != -1)
                        rootNavigation.Navigate(firstvisible);
                    else
                    {
                        rootNavigation.NavigateExternal(new Pages.UnsupportedPage());
                    }
                }
            }
        }

        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            RenderCategories();
            UpdateCategoryVisibility();
            FeatureHub.ReloadRequested += FeatureHub_ReloadRequested;
        }

        private void FeatureHub_ReloadRequested()
        {
            Dispatcher.Invoke(UpdateCategoryVisibility);
        }

        private void UiWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            FeatureHub.ReloadRequested -= FeatureHub_ReloadRequested;
        }
    }
}
