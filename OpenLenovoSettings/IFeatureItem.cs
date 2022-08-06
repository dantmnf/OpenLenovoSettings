using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{

    public interface IFeatureItem
    {
        bool IsSupported();
        Type GetValueType();
        object[] GetOptions();
        object? GetValue();
        void SetValue(object? value);
    }

    public abstract class SwitchFeature : IFeatureItem
    {
        public Type GetValueType() => typeof(bool);

        public abstract bool IsSupported();
        public bool[] GetOptions() => new[] { true, false };
        public abstract bool GetValue();
        public abstract void SetValue(bool value);
        object[] IFeatureItem.GetOptions() => GetOptions().Cast<object>().ToArray();
        object IFeatureItem.GetValue() => GetValue();
        void IFeatureItem.SetValue(object? value) => SetValue((bool)value!);
    }

    public abstract class EnumFeature<T> : IFeatureItem where T: struct, Enum
    {
        public Type GetValueType() => typeof(T);
        public abstract bool IsSupported();
        public virtual T[] GetOptions() => Enum.GetValues<T>();
        public abstract T GetValue();
        public abstract void SetValue(T value);
        object[] IFeatureItem.GetOptions() => GetOptions().Cast<object>().ToArray();
        object IFeatureItem.GetValue() => GetValue();
        void IFeatureItem.SetValue(object? value) => SetValue((T)value!);
    }

    public abstract class ActionFeature : IFeatureItem
    {
        public Type GetValueType() => typeof(Action);
        public abstract bool IsSupported();
        public object[] GetOptions() => Array.Empty<object>();
        public object? GetValue() => null;
        public void SetValue(object? value)
        {
            OnAction();
        }
        public abstract void OnAction();
    }
}
