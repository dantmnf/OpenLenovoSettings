﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings
{

    internal delegate void SettingChangedHandler(IFeatureItem item, object newValue);

    internal record class EnumItemViewModel<T> where T : struct, Enum
    {
        public T Value { get; init; }

        public override string? ToString()
        {
            // FIXME: 
            return Value.ToString();
        }
    }

    internal class SettingViewModel : INotifyPropertyChanged
    {
        private readonly IFeatureItem feature;

        public string SettingId { get; init; }
        public string Title { get; }
        public string? Description { get; }
        public string? Icon { get; }
        public Visibility DescriptionVisibility => string.IsNullOrEmpty(Description) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsHeader => false;
        public bool IsAction { get; }
        public bool IsSwitch { get; }
        public bool IsComboBox { get; }

        private bool isApplyInProgress;
        public bool IsApplyInProgress {
            get => isApplyInProgress;
            set {
                isApplyInProgress = value;
                PropertyChanged?.Invoke(this, new(nameof(IsApplyInProgress)));
                PropertyChanged?.Invoke(this, new(nameof(IsControlEnabled)));
                PropertyChanged?.Invoke(this, new(nameof(ProgressRingVisibility)));
            }
        }
        public bool IsControlEnabled => !isApplyInProgress;
        public Visibility ProgressRingVisibility => isApplyInProgress ? Visibility.Visible : Visibility.Collapsed;


        public event Action<IFeatureItem, object?>? OnSettingChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsSwitchChecked { 
            get => IsSwitch ? (bool)feature.GetValue()! : false;
            set => NotifySettingChanged(value);
        }
        public object? ComboBoxSelectedItem { 
            get => IsComboBox ? feature.GetValue() : null;
            set => NotifySettingChanged(value);
        }
        public object[]? ComboBoxItemsSource => IsComboBox ? feature.GetOptions() : null;


        private void NotifySettingChanged(object? value)
        {
            OnSettingChanged?.Invoke(feature, value);
        }

        public void FireSettingChanged()
        {
            PropertyChanged?.Invoke(this, new(nameof(IsSwitchChecked)));
            PropertyChanged?.Invoke(this, new(nameof(ComboBoxSelectedItem)));
        }

        public void ActionClicked()
        {
            if (IsAction)
            {
                OnSettingChanged?.Invoke(feature, null);
            }
        }

        public SettingViewModel(IFeatureItem feature, FeatureAttribute? attr = null)
        {
            this.feature = feature;
            var featuretype = feature.GetType();
            SettingId = featuretype.Name;
            Title = SettingId;
            if (attr == null)
            {
                attr = featuretype.GetCustomAttributes(typeof(FeatureAttribute), false).FirstOrDefault() as FeatureAttribute;
            }
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.Title))
                    Title = attr.Title;
                Description = attr.Description;
                Icon = attr.Icon;
            }
            var valuetype = feature.GetValueType();
            IsAction = valuetype == typeof(Action);
            IsSwitch = valuetype == typeof(bool);
            IsComboBox = valuetype.IsEnum;

        }
    }
}
