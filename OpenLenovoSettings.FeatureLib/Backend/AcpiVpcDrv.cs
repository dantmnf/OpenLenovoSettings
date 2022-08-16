using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenLenovoSettings.Backend
{
    internal class AcpiVpcDrv
    {
        private static readonly Lazy<SafeHandle> lazyHandle = new(OpenDriver);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool DeviceIoControl(SafeHandle hDevice, uint dwIoControlCode,
            IntPtr lpInBuffer, uint nInBufferSize,
            IntPtr lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateFileW(string filename, uint access, int share, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        private static SafeHandle OpenDriver()
        {
            var hfile = CreateFileW(@"\\.\EnergyDrv", 0xC0000000, 3, IntPtr.Zero, 3, 0x80, IntPtr.Zero);
            return new SafeFileHandle(hfile, true);
        }

        public static bool IsSupported => !lazyHandle.Value.IsInvalid && !lazyHandle.Value.IsClosed;

        public static unsafe int Ioctl(uint code, ReadOnlySpan<byte> inBuffer, Span<byte> outBuffer)
        {
            bool result;
            uint size;
            fixed (byte* inbuf = inBuffer)
            fixed (byte* outbuf = outBuffer)
            {
                result = DeviceIoControl(lazyHandle.Value, code, (IntPtr)inbuf, (uint)inBuffer.Length, (IntPtr)outbuf, (uint)outBuffer.Length, out size, IntPtr.Zero);
            }
            if (!result)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return (int)size;
        }

        public static unsafe void Ioctl<TIn, TOut>(uint code, in TIn inBuffer, out TOut outBuffer, bool checkUnderflow = true)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (void* inbuf = &inBuffer)
            fixed (void* outbuf = &outBuffer)
            {
                var len = Ioctl(code, new(inbuf, sizeof(TIn)), new(outbuf, sizeof(TOut)));
                if (checkUnderflow && len != sizeof(TOut))
                {
                    throw new IOException("buffer underrun");
                }
            }

        }

        public static unsafe void Ioctl<TIn>(uint code, in TIn inBuffer) where TIn : unmanaged
        {
            fixed (void* inbuf = &inBuffer)
            {
                Ioctl(code, in inBuffer, out uint _, false);
            }

        }
    }
}
