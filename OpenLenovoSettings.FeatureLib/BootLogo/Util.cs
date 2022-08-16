using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLenovoSettings.BootLogo
{
    internal static class Util
    {
        public static void ThrowForLastWin32Error()
        {
            var err = Marshal.GetLastWin32Error();
            if (err == 0) return;
            throw new Win32Exception(err);
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCommandLineW();
        public static string GetArgv0()
        {
            var cmdline = Marshal.PtrToStringUni(GetCommandLineW())!.TrimStart();
            string argv0;
            if (cmdline.StartsWith("\""))
            {
                var end = cmdline.IndexOf('"', 1);
                argv0 = cmdline.Substring(0, end + 1);
            }
            else
            {
                var end = cmdline.IndexOf(' ');
                argv0 = cmdline.Substring(0, end);
            }
            return argv0;
        }


        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const int TOKEN_QUERY = 0x00000008;
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
        public const string SE_BACKUP_NAME = "SeBackupPrivilege";

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string? host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, in TokenPrivelege newst, int len, IntPtr prev, IntPtr relen);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokenPrivelege
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        public static void PromoteProcessPrivileges(string privilege, bool enable)
        {
            IntPtr hToken = IntPtr.Zero;

            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken))
            {
                ThrowForLastWin32Error();
            }

            TokenPrivelege tp;
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = enable ? SE_PRIVILEGE_ENABLED : 0;

            if (!LookupPrivilegeValue(null, privilege, ref tp.Luid))
            {
                ThrowForLastWin32Error();
            }
            if (!AdjustTokenPrivileges(hToken, false, tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                ThrowForLastWin32Error();
            }
        }
    }
}
