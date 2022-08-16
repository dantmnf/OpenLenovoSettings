using OpenLenovoSettings.Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Feature.Performance
{
    [Feature(Title = "Use Lenovo ITS service", Icon = "ContentSettings24", Order = -1)]
    public class EnableITSService : SwitchFeature
    {
        public override bool GetValue()
        {
            try
            {
                using var svc = ITSService.OpenService();
                if (svc == null) return false;
                return svc.Status == System.ServiceProcess.ServiceControllerStatus.Running;
            }
            catch { }
            return false;
        }

        public override bool IsSupported()
        {
            if (!AcpiVpcDrv.IsSupported) return false;
            try
            {
                using var svc = ITSService.OpenService();
                return true;
            }
            catch { }
            return false;
        }

        private static void WaitServiceState(ServiceControllerStatus status)
        {
            try
            {
                using var svc = ITSService.OpenService();
                if (svc == null) return;
                svc.WaitForStatus(status, TimeSpan.FromSeconds(1));
            }
            catch { }
        }

        public override void SetValue(bool enabled)
        {
            if (enabled)
            {
                try
                {
                    Process.Start(new ProcessStartInfo() { UseShellExecute = false, FileName = "sc.exe", Arguments = "config LITSSVC start=auto", CreateNoWindow = true })!.WaitForExit();
                    Process.Start(new ProcessStartInfo() { UseShellExecute = false, FileName = "sc.exe", Arguments = "start LITSSVC", CreateNoWindow = true })!.WaitForExit();
                    WaitServiceState(ServiceControllerStatus.Running);
                }
                catch { }
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo() { UseShellExecute = false, FileName = "sc.exe", Arguments = "stop LITSSVC", CreateNoWindow = true })!.WaitForExit();
                    Process.Start(new ProcessStartInfo() { UseShellExecute = false, FileName = "sc.exe", Arguments = "config LITSSVC start=disabled", CreateNoWindow = true })!.WaitForExit();
                    WaitServiceState(ServiceControllerStatus.Stopped);
                }
                catch { }
            }
            FeatureHub.RequestReloadFeatures();
        }
    }
}
