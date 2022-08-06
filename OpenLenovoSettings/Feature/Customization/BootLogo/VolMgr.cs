using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using static OpenLenovoSettings.Feature.Customization.BootLogo.Util;

namespace OpenLenovoSettings.Feature.Customization.BootLogo
{
    internal record class VolumeRecord(string VolumePath, string NtObject);

    internal unsafe class VolMgr
    {
        [StructLayout(LayoutKind.Sequential)]
        struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;

            public override string ToString()
            {
                if (Length == 0 || Buffer == IntPtr.Zero) return string.Empty;
                return Marshal.PtrToStringUni(Buffer, Length / 2);
            }
        }

        [DllImport("ntdll.dll")]
        static extern int NtQuerySystemInformation(int SystemInformationClass, void* SystemInformation, uint SystemInformationLength, out uint ReturnLength);
        const int SystemSystemPartitionInformation = 0x62;

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr FindFirstVolumeW(char* lpszVolumeName, int cchBufferLength);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FindNextVolumeW(IntPtr hFindVolume, char* lpszVolumeName, uint cchBufferLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FindVolumeClose(IntPtr hFindVolume);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateFileW(string filename, int access, int share, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);
        const int OPEN_EXISTING = 3;
        const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetFinalPathNameByHandle(IntPtr hFile, char* lpszFilePath, uint cchFilePath, FinalPathFlags dwFlags);

        [Flags]
        enum FinalPathFlags : uint
        {
            VOLUME_NAME_DOS = 0x0,
            FILE_NAME_NORMALIZED = 0x0,
            VOLUME_NAME_GUID = 0x1,
            VOLUME_NAME_NT = 0x2,
            VOLUME_NAME_NONE = 0x4,
            FILE_NAME_OPENED = 0x8
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hHandle);

        public static VolumeRecord[] FindVolumes()
        {
            var buf = stackalloc char[260];
            IntPtr find = FindFirstVolumeW(buf, 260);
            if (find == IntPtr.Zero) return Array.Empty<VolumeRecord>();
            var list = new List<VolumeRecord>();
            var volpath = new string(buf);
            list.Add(new(volpath, VolumePathToNtObjectOrEmpty(volpath)));
            while (FindNextVolumeW(find, buf, 260))
            {
                volpath = new string(buf);
                list.Add(new(volpath, VolumePathToNtObjectOrEmpty(volpath)));
            }
            FindVolumeClose(find);
            return list.ToArray();
        }

        private static string VolumePathToNtObject(string volumePath)
        {
            var hfile = CreateFileW(volumePath, 0, 0, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
            if (hfile == new IntPtr(-1)) ThrowForLastWin32Error();
            var len = GetFinalPathNameByHandle(hfile, null, 0, FinalPathFlags.VOLUME_NAME_NT);
            if (len == 0) ThrowForLastWin32Error();
            var buf = stackalloc char[(int)len];
            GetFinalPathNameByHandle(hfile, buf, len, FinalPathFlags.VOLUME_NAME_NT);
            var result = new string(buf, 0, (int)len - 2);
            CloseHandle(hfile);
            return result;
        }

        private static string VolumePathToNtObjectOrEmpty(string volumePath)
        {
            try
            {
                return VolumePathToNtObject(volumePath);
            }
            catch
            {
                return "";
            }
        }

        public static string? GetEfiSystemPartitionVolumePath()
        {
            PromoteProcessPrivileges(SE_BACKUP_NAME, true);
            uint buflen = 520;
            IntPtr info;
            while (true)
            {
                info = Marshal.AllocHGlobal((int)buflen);
                int status;
                status = NtQuerySystemInformation(SystemSystemPartitionInformation, (void*)info, buflen, out buflen);
                if (status >= 0) break;
                if (status != -1073741789) throw new Exception();
                Marshal.FreeHGlobal(info);
            }
            var systemPartitionNT = ((UNICODE_STRING*)info)->ToString();
            Marshal.FreeHGlobal(info);

            var vols = FindVolumes();

            var efivol = vols.FirstOrDefault((x) => x.NtObject == systemPartitionNT);

            if (efivol != null) return efivol.VolumePath;

            return null;
        }
    }
}