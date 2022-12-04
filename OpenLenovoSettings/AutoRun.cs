using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings
{
    class AutoRun
    {
        public static object? ReadSetting(string settingId, Type ofType)
        {
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OpenLenovoSettings\ApplyOnBoot");
                if (hkey == null) return null;
                var value = hkey.GetValue(settingId, null);
                if (value == null) return null;

                if (value.GetType() == ofType) return value;

                if (ofType.IsEnum)
                {
                    return Enum.ToObject(ofType, value);
                }
                return Convert.ChangeType(value, ofType);
            }
            catch { return null; }
        }

        public static bool WriteSetting(string settingId, object? value)
        {
            try
            {
                using var hkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\OpenLenovoSettings\ApplyOnBoot", true);
                if (hkey == null) return false;
                if (value == null)
                {
                    hkey.DeleteValue(settingId, false);
                    UpdateScheduledTask();
                    return true;
                }
                var regval = value;
                if (regval.GetType() != typeof(string))
                {
                    regval = Convert.ChangeType(value, typeof(int));
                }

                hkey.SetValue(settingId, regval);
                UpdateScheduledTask();
                return true;
            }
            catch { return false; }
        }

        private static void UpdateScheduledTask()
        {
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OpenLenovoSettings\ApplyOnBoot");
                if (hkey == null) return;
                var ids = hkey.GetValueNames();
                if (ids.Length > 0)
                {
                    EnsureScheduledTask();
                }
                else
                {
                    TryRemoveScheduledTask();
                }
            }
            catch { }

        }

        private static void EnsureScheduledTask()
        {
            using var ts = new TaskService();
            ts.RootFolder.DeleteTask("OpenLenovoSettings.AutoRun", false);

            var td = ts.NewTask();
            td.RegistrationInfo.Description = "Restore settings set in OpenLenovoSettings.";
            td.Triggers.AddNew(TaskTriggerType.Boot);
            td.Actions.Add(Environment.ProcessPath, "-boot");
            td.Settings.AllowDemandStart = true;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.Enabled = true;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            td.Principal.UserId = "NT AUTHORITY\\SYSTEM";
            td.Principal.RunLevel = TaskRunLevel.Highest;

            ts.RootFolder.RegisterTaskDefinition("OpenLenovoSettings.AutoRun", td);
        }

        private static void TryRemoveScheduledTask()
        {
            using var ts = new TaskService();
            ts.RootFolder.DeleteTask("OpenLenovoSettings.AutoRun", false);
        }

        public static void ApplySettings()
        {
            try
            {
                using var hkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OpenLenovoSettings\ApplyOnBoot");
                if (hkey == null) return;
                var ids = hkey.GetValueNames();
                foreach (var id in ids)
                {
                    try
                    {
                        var feature = FeatureHub.GetFeatureInstance(id);
                        var value = ReadSetting(id, feature.GetValueType());
                        feature.SetValue(value);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
