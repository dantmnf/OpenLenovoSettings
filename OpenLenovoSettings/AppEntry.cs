using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLenovoSettings
{
    class AppEntry
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-boot")
            {
                AutoRun.ApplySettings();
                return 0;
            }

            var a = new App();
            a.InitializeComponent();
            return a.Run();
        }
    }
}
