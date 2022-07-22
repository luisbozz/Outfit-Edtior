using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Pointer = System.UInt64;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Globalization;

namespace mry
{
    [SuppressUnmanagedCodeSecurity]
    public class mem
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, IntPtr lpBuffer, UIntPtr dwSize, out ulong lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, IntPtr flNewProtect, out IntPtr lpflOldProtect);

        [DllImport("ntdll")]
        private static extern bool NtReadVirtualMemory(IntPtr ProcessHandle, IntPtr BaseAddress, byte[] Buffer, int NumberOfBytesToRead, int NumberOfBytesRead);
        [DllImport("ntdll")]
        private static extern bool NtWriteVirtualMemory(IntPtr ProcessHandle, IntPtr BaseAddress, byte[] Buffer, int NumberOfBytesToWrite, int NumberOfBytesWritten);

        public static UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer)
        {
            UIntPtr retVal;

            // TODO: Need to change this to only check once.
            if (IntPtr.Size == 8)
            {
                // 64 bit
                MEMORY_BASIC_INFORMATION64 tmp64 = new MEMORY_BASIC_INFORMATION64();
                retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp64, new UIntPtr((uint)Marshal.SizeOf(tmp64)));

                lpBuffer.BaseAddress = tmp64.BaseAddress;
                lpBuffer.AllocationBase = tmp64.AllocationBase;
                lpBuffer.AllocationProtect = tmp64.AllocationProtect;
                lpBuffer.RegionSize = (long)tmp64.RegionSize;
                lpBuffer.State = tmp64.State;
                lpBuffer.Protect = tmp64.Protect;
                lpBuffer.Type = tmp64.Type;

                return retVal;
            }

            MEMORY_BASIC_INFORMATION32 tmp32 = new MEMORY_BASIC_INFORMATION32();

            retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp32, new UIntPtr((uint)Marshal.SizeOf(tmp32)));

            lpBuffer.BaseAddress = tmp32.BaseAddress;
            lpBuffer.AllocationBase = tmp32.AllocationBase;
            lpBuffer.AllocationProtect = tmp32.AllocationProtect;
            lpBuffer.RegionSize = tmp32.RegionSize;
            lpBuffer.State = tmp32.State;
            lpBuffer.Protect = tmp32.Protect;
            lpBuffer.Type = tmp32.Type;

            return retVal;
        }

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION32 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION64 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32")]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        private int pid;
        internal static IntPtr procHandle;
        internal static Pointer BaseAddress;
        private int ImageSize;
        private String FileName;

        // privileges
        private static int PROCESS_WM_READ = 0x0010;
        private static int PROCESS_WM_WRITE = 0x0020;
        private static int PROCESS_ALL_ACCESS = 0x1F0FFF;

        // mem allocation
        private const uint MEM_FREE = 0x10000;
        private const uint MEM_COMMIT = 0x00001000;
        private const uint MEM_RESERVE = 0x00002000;

        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_WRITECOPY = 0x08;

        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        private const uint PAGE_EXECUTE = 0x10;
        private const uint PAGE_EXECUTE_READ = 0x20;

        private const uint PAGE_GUARD = 0x100;
        private const uint PAGE_NOACCESS = 0x01;

        private const uint MEM_PRIVATE = 0x20000;
        private const uint MEM_IMAGE = 0x1000000;

        private void initAddresses(System.Diagnostics.Process process, String moduleName)
        {
            if (moduleName == null)
            {
                BaseAddress = (Pointer)process.MainModule.BaseAddress;
                ImageSize = process.MainModule.ModuleMemorySize;
                return;
            }
            System.Diagnostics.ProcessModuleCollection Modules = process.Modules;
            int moduleCount = Modules.Count;
            if (moduleCount == 0)
            {
                BaseAddress = 0;
                ImageSize = 0;
                return;
            }
            for (int i = 0; i < moduleCount; i++)
            {
                if (Modules[i].ModuleName == moduleName)
                {
                    BaseAddress = (Pointer)Modules[i].BaseAddress;
                    ImageSize = Modules[i].ModuleMemorySize;
                    break;
                }
            }
        }

        private void Init(System.Diagnostics.Process process, String moduleName)
        {
            initAddresses(process, moduleName);
            pid = process.Id;
            FileName = process.MainModule.FileName;
            procHandle = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            isProcOpen = (procHandle != null) ? true : false;

        }

        public static bool isProcOpen;
        public bool IsProcOpen { get => isProcOpen; }

        public void OpenProcess(String processName, String moduleName = null)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
            if (processes.Length > 0)
                Init(processes[0], moduleName);
        }

        public void OpenProcess(int processId, String moduleName = null)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processId);
            Init(process, moduleName);
        }

        /// <summary>
        /// Close the process when finished.
        /// </summary>
        public void CloseProcess()
        {
            if (procHandle.ToInt64() != 0)
                CloseHandle(procHandle);
            isProcOpen = false;
        }


        public Pointer getBaseAddress()
        {
            return BaseAddress;
        }
        public int getProcessId()
        {
            return pid;
        }
        public IntPtr getModuleHandle()
        {
            return procHandle;
        }
        public String getFileName()
        {
            return FileName;
        }

        public Memory memory(long pointer)
        {
            return new Memory(pointer);
        }

        public Memory memory(string address)
        {
            return new Memory(address);
        }

        public Memory memory(long pointer, long offsets)
        {
            return new Memory(pointer, offsets);
        }

        public Memory memory(long pointer, int offsets)
        {
            return new Memory(pointer, offsets);
        }

        public Memory memory(long pointer, string offsets)
        {
            return new Memory(pointer, offsets);
        }

        public Memory memory(long pointer, string[] offsets)
        {
            return new Memory(pointer, offsets);
        }

        public Memory memory(long pointer, long[] offsets)
        {
            return new Memory(pointer, offsets);
        }

        public Memory memory(long pointer, int[] offsets)
        {
            return new Memory(pointer, offsets);
        }

        /// <summary>
        /// Array of byte scan.
        /// </summary>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(string search, bool writable = true, bool executable = false)
        {
            return AOB.AoBScan(0, long.MaxValue, search, true, writable, executable);
        }

        /// <summary>
        /// Array of byte scan.
        /// </summary>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(long start, long end, string search, bool writable = true, bool executable = false)
        {
            return AOB.AoBScan(start, end, search, true, writable, executable);
        }

        public async Task<IEnumerable<UIntPtr>> AOBScanFast(byte[] data)
        {
            return await new AOBScan((uint)pid).AobScan2(data);
        }

        public class Memory
        {
            long address;

            public Memory(string address)
            {
                this.address = long.Parse(address, System.Globalization.NumberStyles.HexNumber);
            }

            public Memory(long pointer)
            {
                address = (long)BaseAddress + (int)pointer;
            }

            public Memory(long pointer, long offset)
            {
                byte[] buffer_temp = new byte[16];

                ReadProcessMemory(procHandle, (long)BaseAddress + pointer, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                address = BitConverter.ToInt64(buffer_temp, 0) + offset;
            }

            public Memory(long pointer, string offset)
            {
                byte[] buffer_temp = new byte[16];

                ReadProcessMemory(procHandle, (long)BaseAddress + pointer, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                address = BitConverter.ToInt64(buffer_temp, 0) + Convert.ToInt64(offset);
            }

            public Memory(long pointer, long[] offsets)
            {
                byte[] buffer_temp = new byte[16];
                ReadProcessMemory(procHandle, (long)BaseAddress + pointer, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                long temp = BitConverter.ToInt64(buffer_temp, 0);
                for (int i = 0; i < offsets.Length; i++)
                {
                    temp += offsets[i];
                    ReadProcessMemory(procHandle, temp, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                    if (i < offsets.Length - 1)
                        temp = BitConverter.ToInt64(buffer_temp, 0);
                }
                address = temp;
            }

            public Memory(long pointer, int[] offsets)
            {
                byte[] buffer_temp = new byte[16];
                ReadProcessMemory(procHandle, (long)BaseAddress + pointer, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                long temp = BitConverter.ToInt64(buffer_temp, 0);
                for (int i = 0; i < offsets.Length; i++)
                {
                    temp = temp + offsets[i];
                    ReadProcessMemory(procHandle, temp, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                    if (i < offsets.Length - 1)
                        temp = BitConverter.ToInt64(buffer_temp, 0);
                }
                address = temp;
            }

            public Memory(long pointer, string[] offsets)
            {
                byte[] buffer_temp = new byte[16];
                ReadProcessMemory(procHandle, (long)BaseAddress + pointer, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                long temp = BitConverter.ToInt64(buffer_temp, 0);
                for (int i = 0; i < offsets.Length; i++)
                {
                    temp = temp + Convert.ToInt64(offsets[i]);
                    ReadProcessMemory(procHandle, temp, buffer_temp, buffer_temp.Length, IntPtr.Zero);
                    if (i < offsets.Length - 1)
                        temp = BitConverter.ToInt64(buffer_temp, 0);
                }
                address = temp;
            }

            public long GetAddress()
            {
                return address;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe T Get<T>() where T : struct
            {
                byte[] buffer = new byte[Unsafe.SizeOf<T>()];

                NtReadVirtualMemory(procHandle, (IntPtr)address, buffer, buffer.Length, 0);

                fixed (byte* b = buffer)
                    return Unsafe.Read<T>(b);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe bool Write<T>(T value) where T : struct
            {
                byte[] buffer = new byte[Unsafe.SizeOf<T>()];

                fixed (byte* b = buffer)
                    Unsafe.Write<T>(b, value);

                return NtWriteVirtualMemory(procHandle, (IntPtr)address, buffer, buffer.Length, 0);
            }

            public string GetString(int size = 255, bool unicode = true)
            {
                System.Text.Encoding encoding = unicode ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default;
                byte[] buffer = new byte[size];

                NtReadVirtualMemory(procHandle, (IntPtr)address, buffer, buffer.Length, 0);

                String str = encoding.GetString(buffer);
                int pos = str.IndexOf('\0');
                if (pos > -1)
                    str = str.Substring(0, pos);
                return str;
            }

            public unsafe byte[] GetBytes(int length)
            {
                byte[] buffer = new byte[length];

                NtReadVirtualMemory(procHandle, (IntPtr)address, buffer, buffer.Length, 0);

                return buffer;
            }

            private void WriteMemory(Pointer address, byte[] buffer)
            {
                IntPtr oldProtection;
                VirtualProtectEx(procHandle, (IntPtr)address, buffer.Length, (IntPtr)0x40, out oldProtection);
                WriteProcessMemory(procHandle, (IntPtr)address, buffer, buffer.Length, IntPtr.Zero);
                VirtualProtectEx(procHandle, (IntPtr)address, buffer.Length, oldProtection, out oldProtection);
            }

            public void WriteString(string str, bool unicode = true)
            {
                WriteMemory((UInt64)address, unicode ? System.Text.Encoding.UTF8.GetBytes(str) : System.Text.Encoding.Default.GetBytes(str));
            }

            //public unsafe void WriteString(string str, bool unicode = true)
            //{
            //    byte[] buffer = unicode ? System.Text.Encoding.UTF8.GetBytes(str) : System.Text.Encoding.Default.GetBytes(str);

            //    fixed (byte* b = buffer)
            //        Unsafe.Write<byte[]>(b, buffer);

            //    NtWriteVirtualMemory(procHandle, (IntPtr)address, buffer, buffer.Length, 0);
            //}

            public dynamic Set
            {
                set
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
            }


            // Write Int
            public bool SetInt(int value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetInt(uint value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetInt(string value)
            {
                if (isProcOpen)
                {
                    if (value != "")
                    {
                        byte[] bytes = null;
                        try
                        {
                            bytes = BitConverter.GetBytes(int.Parse(value));
                        }
                        catch (Exception)
                        {
                            bytes = BitConverter.GetBytes(uint.Parse(value));
                        }

                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }



            // Write Long
            public bool SetLong(long value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetLong(ulong value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetLong(string value)
            {
                if (isProcOpen)
                {
                    if (value != "")
                    {
                        byte[] bytes = BitConverter.GetBytes(long.Parse(value));
                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            /// <summary>
            /// Write a String into opened Process Memory. (Only ment to be used with Numeric Datatypes)
            /// </summary>
            /// <typeparam name="T">Numeric Input Types and Strings(int, float, double, long, byte)</typeparam>
            /// <param name="value">Generic value that gets written into the opened Memory.</param>
            /// <returns></returns>
            public bool SetString<T>(T value)
            {
                if (isProcOpen)
                {
                    if (value != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length + Convert.ToInt32(true), IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetString(string value)
            {
                if (isProcOpen)
                {
                    if (value != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(value);
                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length + Convert.ToInt32(true), IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }



            // Write Float
            public bool SetFloat(float value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetFloat(string value)
            {
                if (isProcOpen)
                {
                    if (value != "" && value != "-")
                    {
                        byte[] bytes = BitConverter.GetBytes(float.Parse(value));
                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }



            // Write Double
            public bool SetDouble(double value)
            {
                if (isProcOpen)
                {
                    byte[] bytes = BitConverter.GetBytes(value);
                    return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            public bool SetDobule(string value)
            {
                if (isProcOpen)
                {
                    if (value != "")
                    {
                        byte[] bytes = BitConverter.GetBytes(double.Parse(value));
                        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, bytes.Length, IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }



            // Write Bytes
            public bool SetBytes(byte[] value)
            {
                if (isProcOpen)
                {
                    if (value != null)
                    {
                        return WriteProcessMemory(procHandle, new IntPtr(address), value, value.Length, IntPtr.Zero);
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Please open the Process first.");
                }
            }

            #region Generic Test

            //public bool Set<T>(int value)
            //{
            //    if (isProcOpen)
            //    {
            //        byte[] bytes = null;

            //        if (typeof(T) == typeof(int))
            //            bytes = BitConverter.GetBytes((int)Convert.ChangeType(Convert.ToInt32(value), typeof(T)));
            //        else if (typeof(T) == typeof(float))
            //            bytes = BitConverter.GetBytes((float)Convert.ChangeType(Convert.ToSingle(value), typeof(T)));
            //        else if (typeof(T) == typeof(double))
            //            bytes = BitConverter.GetBytes((double)Convert.ChangeType(Convert.ToDouble(value), typeof(T)));
            //        else if (typeof(T) == typeof(long))
            //            bytes = BitConverter.GetBytes((long)Convert.ChangeType(Convert.ToInt64(value), typeof(T)));
            //        else if (typeof(T) == typeof(byte))
            //            bytes = BitConverter.GetBytes((byte)Convert.ChangeType(Convert.ToByte(value), typeof(T)));
            //        else if (typeof(T) == typeof(string))
            //            bytes = Encoding.UTF8.GetBytes((string)Convert.ChangeType(Convert.ToString(value), typeof(T)));
            //        else
            //            bytes = BitConverter.GetBytes((int)Convert.ChangeType(Convert.ToInt32(value), typeof(T)));

            //        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, checked((uint)bytes.Length), 0);
            //    }
            //    else
            //    {
            //        throw new Exception("Please Open the Process.");
            //    }
            //}


            //public bool Set<T>(string value)
            //{
            //    if (isProcOpen)
            //    {
            //        byte[] bytes = Encoding.UTF8.GetBytes(value);

            //        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, checked((uint)((long)(uint)bytes.Length + (long)Convert.ToInt32(true))), 0);
            //    }
            //    else
            //    {
            //        throw new Exception("Please Open the Process.");
            //    }
            //}
            //public bool Set<T>(double value)
            //{
            //    if (isProcOpen)
            //    {
            //        byte[] bytes = BitConverter.GetBytes(value);

            //        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, checked((uint)bytes.Length), 0);
            //    }
            //    else
            //    {
            //        throw new Exception("Please Open the Process.");
            //    }
            //}
            //public bool Set<T>(float value)
            //{
            //    if (isProcOpen)
            //    {
            //        byte[] bytes = BitConverter.GetBytes(value);

            //        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, checked((uint)bytes.Length), 0);
            //    }
            //    else
            //    {
            //        throw new Exception("Please Open the Process.");
            //    }
            //}
            //public bool Set<T>(byte value)
            //{
            //    if (isProcOpen)
            //    {
            //        byte[] bytes = BitConverter.GetBytes(value);

            //        return WriteProcessMemory(procHandle, new IntPtr(address), bytes, checked((uint)bytes.Length), 0);
            //    }
            //    else
            //    {
            //        throw new Exception("Please Open the Process.");
            //    }
            //}

            #endregion

        }

        public class SigScanSharp
        {
            private IntPtr g_hProcess { get; set; }
            private byte[] g_arrModuleBuffer { get; set; }
            private ulong g_lpModuleBase { get; set; }

            private Dictionary<string, string> g_dictStringPatterns { get; }

            public SigScanSharp(IntPtr hProc)
            {
                g_hProcess = hProc;
                g_dictStringPatterns = new Dictionary<string, string>();
            }

            public bool SelectModule(ProcessModule targetModule)
            {
                g_lpModuleBase = (ulong)targetModule.BaseAddress;
                g_arrModuleBuffer = new byte[targetModule.ModuleMemorySize];

                g_dictStringPatterns.Clear();

                return Win32.ReadProcessMemory(g_hProcess, g_lpModuleBase, g_arrModuleBuffer, targetModule.ModuleMemorySize);
            }

            public void AddPattern(string szPatternName, string szPattern)
            {
                g_dictStringPatterns.Add(szPatternName, szPattern);
            }

            private bool PatternCheck(int nOffset, byte[] arrPattern)
            {
                for (int i = 0; i < arrPattern.Length; i++)
                {
                    if (arrPattern[i] == 0x0)
                        continue;

                    if (arrPattern[i] != this.g_arrModuleBuffer[nOffset + i])
                        return false;
                }

                return true;
            }

            public ulong FindPattern(string szPattern, out long lTime)
            {
                if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                    throw new Exception("Selected module is null");

                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[] arrPattern = ParsePatternString(szPattern);

                for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
                {
                    if (this.g_arrModuleBuffer[nModuleIndex] != arrPattern[0])
                        continue;

                    if (PatternCheck(nModuleIndex, arrPattern))
                    {
                        lTime = stopwatch.ElapsedMilliseconds;
                        return g_lpModuleBase + (ulong)nModuleIndex;
                    }
                }

                lTime = stopwatch.ElapsedMilliseconds;
                return 0;
            }
            public Dictionary<string, ulong> FindPatterns(out long lTime)
            {
                if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                    throw new Exception("Selected module is null");

                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[][] arrBytePatterns = new byte[g_dictStringPatterns.Count][];
                ulong[] arrResult = new ulong[g_dictStringPatterns.Count];

                // PARSE PATTERNS
                for (int nIndex = 0; nIndex < g_dictStringPatterns.Count; nIndex++)
                    arrBytePatterns[nIndex] = ParsePatternString(g_dictStringPatterns.ElementAt(nIndex).Value);

                // SCAN FOR PATTERNS
                for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
                {
                    for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                    {
                        if (arrResult[nPatternIndex] != 0)
                            continue;

                        if (this.PatternCheck(nModuleIndex, arrBytePatterns[nPatternIndex]))
                            arrResult[nPatternIndex] = g_lpModuleBase + (ulong)nModuleIndex;
                    }
                }

                Dictionary<string, ulong> dictResultFormatted = new Dictionary<string, ulong>();

                // FORMAT PATTERNS
                for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                    dictResultFormatted[g_dictStringPatterns.ElementAt(nPatternIndex).Key] = arrResult[nPatternIndex];

                lTime = stopwatch.ElapsedMilliseconds;
                return dictResultFormatted;
            }

            private byte[] ParsePatternString(string szPattern)
            {
                List<byte> patternbytes = new List<byte>();

                foreach (var szByte in szPattern.Split(' '))
                    patternbytes.Add(szByte == "?" ? (byte)0x0 : Convert.ToByte(szByte, 16));

                return patternbytes.ToArray();
            }

            private static class Win32
            {
                [DllImport("kernel32.dll")]
                public static extern bool ReadProcessMemory(IntPtr hProcess, ulong lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead = 0);
            }
        }

        struct MemoryRegionResult
        {
            public UIntPtr CurrentBaseAddress { get; set; }
            public long RegionSize { get; set; }
            public UIntPtr RegionBase { get; set; }

        }

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public UIntPtr minimumApplicationAddress;
            public UIntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION32
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public struct MEMORY_BASIC_INFORMATION64
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public long RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public ulong getMinAddress()
        {
            SYSTEM_INFO SI;
            GetSystemInfo(out SI);
            return (ulong)SI.minimumApplicationAddress;
        }

        public class AOB
        {
            /// <summary>
            /// Array of Byte scan.
            /// </summary>
            /// <param name="start">Your starting address.</param>
            /// <param name="end">ending address</param>
            /// <param name="search">array of bytes to search for, OR your ini code label.</param>
            /// <param name="readable">Include readable addresses in scan</param>
            /// <param name="writable">Include writable addresses in scan</param>
            /// <param name="executable">Include executable addresses in scan</param>
            /// <returns>IEnumerable of all addresses found.</returns>
            public static Task<IEnumerable<long>> AoBScan(long start, long end, string search, bool readable, bool writable, bool executable)
            {
                return Task.Run(() =>
                {
                    var memRegionList = new List<MemoryRegionResult>();

                    string memCode = search;

                    string[] stringByteArray = memCode.Split(' ');

                    byte[] aobPattern = new byte[stringByteArray.Length];
                    byte[] mask = new byte[stringByteArray.Length];

                    for (var i = 0; i < stringByteArray.Length; i++)
                    {
                        string ba = stringByteArray[i];

                        if (ba == "??" || (ba.Length == 1 && ba == "?"))
                        {
                            mask[i] = 0x00;
                            stringByteArray[i] = "0x00";
                        }
                        else if (Char.IsLetterOrDigit(ba[0]) && ba[1] == '?')
                        {
                            mask[i] = 0xF0;
                            stringByteArray[i] = ba[0] + "0";
                        }
                        else if (Char.IsLetterOrDigit(ba[1]) && ba[0] == '?')
                        {
                            mask[i] = 0x0F;
                            stringByteArray[i] = "0" + ba[1];
                        }
                        else
                            mask[i] = 0xFF;
                    }


                    for (int i = 0; i < stringByteArray.Length; i++)
                        aobPattern[i] = (byte)(Convert.ToByte(stringByteArray[i], 16) & mask[i]);

                    SYSTEM_INFO sys_info = new SYSTEM_INFO();
                    GetSystemInfo(out sys_info);

                    UIntPtr proc_min_address = sys_info.minimumApplicationAddress;
                    UIntPtr proc_max_address = sys_info.maximumApplicationAddress;

                    if (start < (long)proc_min_address.ToUInt64())
                        start = (long)proc_min_address.ToUInt64();

                    if (end > (long)proc_max_address.ToUInt64())
                        end = (long)proc_max_address.ToUInt64();

                    Debug.WriteLine("[DEBUG] memory scan starting... (start:0x" + start.ToString("x16") + " end:0x" + end.ToString("x16") + " time:" + DateTime.Now.ToString("h:mm:ss tt") + ")");
                    UIntPtr currentBaseAddress = new UIntPtr((ulong)start);

                    MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();

                    //Debug.WriteLine("[DEBUG] start:0x" + start.ToString("X8") + " curBase:0x" + currentBaseAddress.ToUInt64().ToString("X8") + " end:0x" + end.ToString("X8") + " size:0x" + memInfo.RegionSize.ToString("X8") + " vAloc:" + VirtualQueryEx(pHandle, currentBaseAddress, out memInfo).ToUInt64().ToString());

                    while (VirtualQueryEx(procHandle, currentBaseAddress, out memInfo).ToUInt64() != 0 &&
                           currentBaseAddress.ToUInt64() < (ulong)end &&
                           currentBaseAddress.ToUInt64() + (ulong)memInfo.RegionSize >
                           currentBaseAddress.ToUInt64())
                    {
                        bool isValid = memInfo.State == MEM_COMMIT;
                        isValid &= memInfo.BaseAddress.ToUInt64() < (ulong)proc_max_address.ToUInt64();
                        isValid &= ((memInfo.Protect & PAGE_GUARD) == 0);
                        isValid &= ((memInfo.Protect & PAGE_NOACCESS) == 0);
                        isValid &= (memInfo.Type == mem.MEM_PRIVATE) || (memInfo.Type == MEM_IMAGE);

                        if (isValid)
                        {
                            bool isReadable = (memInfo.Protect & PAGE_READONLY) > 0;

                            bool isWritable = ((memInfo.Protect & PAGE_READWRITE) > 0) ||
                                              ((memInfo.Protect & PAGE_WRITECOPY) > 0) ||
                                              ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) ||
                                              ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

                            bool isExecutable = ((memInfo.Protect & PAGE_EXECUTE) > 0) ||
                                                ((memInfo.Protect & PAGE_EXECUTE_READ) > 0) ||
                                                ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) ||
                                                ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

                            isReadable &= readable;
                            isWritable &= writable;
                            isExecutable &= executable;

                            isValid &= isReadable || isWritable || isExecutable;
                        }

                        if (!isValid)
                        {
                            currentBaseAddress = new UIntPtr(memInfo.BaseAddress.ToUInt64() + (ulong)memInfo.RegionSize);
                            continue;
                        }

                        MemoryRegionResult memRegion = new MemoryRegionResult
                        {
                            CurrentBaseAddress = currentBaseAddress,
                            RegionSize = memInfo.RegionSize,
                            RegionBase = memInfo.BaseAddress
                        };

                        currentBaseAddress = new UIntPtr(memInfo.BaseAddress.ToUInt64() + (ulong)memInfo.RegionSize);

                        //Console.WriteLine("SCAN start:" + memRegion.RegionBase.ToString() + " end:" + currentBaseAddress.ToString());

                        if (memRegionList.Count > 0)
                        {
                            var previousRegion = memRegionList[memRegionList.Count - 1];

                            if ((long)previousRegion.RegionBase + previousRegion.RegionSize == (long)memInfo.BaseAddress)
                            {
                                memRegionList[memRegionList.Count - 1] = new MemoryRegionResult
                                {
                                    CurrentBaseAddress = previousRegion.CurrentBaseAddress,
                                    RegionBase = previousRegion.RegionBase,
                                    RegionSize = previousRegion.RegionSize + memInfo.RegionSize
                                };

                                continue;
                            }
                        }

                        memRegionList.Add(memRegion);
                    }

                    ConcurrentBag<long> bagResult = new ConcurrentBag<long>();

                    List<List<MemoryRegionResult>> ListOfChunks = new List<List<MemoryRegionResult>>();

                    int indexss = 0;
                    while (indexss < memRegionList.Count)
                    {
                        int count = memRegionList.Count - indexss > 1000 ? 1000 : memRegionList.Count - indexss;
                        ListOfChunks.Add(memRegionList.GetRange(indexss, count));
                        indexss += 1000;
                    }

                    foreach (List<MemoryRegionResult> results in ListOfChunks)
                    {
                        Parallel.ForEach(results, (item, parallelLoopState, index) =>
                        {
                            long[] compareResults = CompareScan(item, aobPattern, mask);

                            foreach (long result in compareResults)
                                bagResult.Add(result);
                        });
                    }

                    //Parallel.ForEach(memRegionList, (item, parallelLoopState, index) =>
                    //{
                    //    long[] compareResults = CompareScan(item, aobPattern, mask);

                    //    foreach (long result in compareResults)
                    //        bagResult.Add(result);
                    //});

                    Debug.WriteLine("[DEBUG] memory scan completed. (time:" + DateTime.Now.ToString("h:mm:ss tt") + ")");

                    return bagResult.ToList().OrderBy(c => c).AsEnumerable();
                });
            }
            private static long[] CompareScan(MemoryRegionResult item, byte[] aobPattern, byte[] mask)
            {
                if (mask.Length != aobPattern.Length)
                    throw new ArgumentException($"{nameof(aobPattern)}.Length != {nameof(mask)}.Length");

                IntPtr buffer = Marshal.AllocHGlobal((int)item.RegionSize);

                ReadProcessMemory(procHandle, item.CurrentBaseAddress, buffer, (UIntPtr)item.RegionSize, out ulong bytesRead);

                int result = 0 - aobPattern.Length;
                List<long> ret = new List<long>();
                unsafe
                {
                    do
                    {

                        result = FindPattern((byte*)buffer.ToPointer(), (int)bytesRead, aobPattern, mask, result + aobPattern.Length);

                        if (result >= 0)
                            ret.Add((long)item.CurrentBaseAddress + result);

                    } while (result != -1);
                }

                Marshal.FreeHGlobal(buffer);

                return ret.ToArray();
            }

            private static int FindPattern(byte[] body, byte[] pattern, byte[] masks, int start = 0)
            {
                int foundIndex = -1;

                if (body.Length <= 0 || pattern.Length <= 0 || start > body.Length - pattern.Length ||
                    pattern.Length > body.Length) return foundIndex;

                for (int index = start; index <= body.Length - pattern.Length; index++)
                {
                    if (((body[index] & masks[0]) == (pattern[0] & masks[0])))
                    {
                        var match = true;
                        for (int index2 = 1; index2 <= pattern.Length - 1; index2++)
                        {
                            if ((body[index + index2] & masks[index2]) == (pattern[index2] & masks[index2])) continue;
                            match = false;
                            break;

                        }

                        if (!match) continue;

                        foundIndex = index;
                        break;
                    }
                }

                return foundIndex;
            }

            private unsafe static int FindPattern(byte* body, int bodyLength, byte[] pattern, byte[] masks, int start = 0)
            {
                int foundIndex = -1;

                if (bodyLength <= 0 || pattern.Length <= 0 || start > bodyLength - pattern.Length ||
                    pattern.Length > bodyLength) return foundIndex;

                for (int index = start; index <= bodyLength - pattern.Length; index++)
                {
                    if (((body[index] & masks[0]) == (pattern[0] & masks[0])))
                    {
                        var match = true;
                        for (int index2 = 1; index2 <= pattern.Length - 1; index2++)
                        {
                            if ((body[index + index2] & masks[index2]) == (pattern[index2] & masks[index2])) continue;
                            match = false;
                            break;

                        }

                        if (!match) continue;

                        foundIndex = index;
                        break;
                    }
                }

                return foundIndex;
            }
        }

        public class AOBScan
        {
            protected uint ProcessID;
            public AOBScan(uint ProcessID)
            {
                this.ProcessID = ProcessID;
            }

            [DllImport("kernel32.dll")]
            protected static extern bool ReadProcessMemory(IntPtr hProcess, ulong lpBaseAddress, byte[] buffer, ulong size, int lpNumberOfBytesRead);
            [DllImport("kernel32.dll")]
            static extern int VirtualQueryEx(IntPtr hProcess, ulong lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, int dwLength);

            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION64
            {
                public ulong BaseAddress;
                public ulong AllocationBase;
                public AllocationProtect AllocationProtect;
                public int __alignment1;
                public ulong RegionSize;
                public State State;
                public AllocationProtect Protect;
                public Type Type;
                public int __alignment2;
            }

            public enum State : uint
            {
                MEM_COMMIT = 0x1000,
                MEM_FREE = 0x10000,
                MEM_RESERVE = 0x2000
            }

            public enum Type : uint
            {
                MEM_IMAGE = 0x1000000,
                MEM_MAPPED = 0x40000,
                MEM_PRIVATE = 0x20000
            }

            public enum AllocationProtect : uint
            {
                PAGE_EXECUTE = 0x00000010,
                PAGE_EXECUTE_READ = 0x00000020,
                PAGE_EXECUTE_READWRITE = 0x00000040,
                PAGE_EXECUTE_WRITECOPY = 0x00000080,
                PAGE_NOACCESS = 0x00000001,
                PAGE_READONLY = 0x00000002,
                PAGE_READWRITE = 0x00000004,
                PAGE_WRITECOPY = 0x00000008,
                PAGE_GUARD = 0x00000100,
                PAGE_NOCACHE = 0x00000200,
                PAGE_WRITECOMBINE = 0x00000400
            }

            protected List<MEMORY_BASIC_INFORMATION64> MemoryRegion { get; set; }

            public static class Pattern
            {
                public struct Byte
                {
                    public struct Nibble
                    {
                        public bool Wildcard;
                        public byte Data;
                    }

                    public Nibble N1;
                    public Nibble N2;
                }

                public static string Format(string pattern)
                {
                    var length = pattern.Length;
                    var result = new StringBuilder(length);
                    for (var i = 0; i < length; i++)
                    {
                        var ch = pattern[i];
                        if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f' || ch == '?')
                            result.Append(ch);
                    }
                    return result.ToString();
                }

                private static int hexChToInt(char ch)
                {
                    if (ch >= '0' && ch <= '9')
                        return ch - '0';
                    if (ch >= 'A' && ch <= 'F')
                        return ch - 'A' + 10;
                    if (ch >= 'a' && ch <= 'f')
                        return ch - 'a' + 10;
                    return -1;
                }

                public static Byte[] Transform(string pattern)
                {
                    pattern = Format(pattern);
                    var length = pattern.Length;
                    if (length == 0)
                        return null;
                    var result = new List<Byte>((length + 1) / 2);
                    if (length % 2 != 0)
                    {
                        pattern += "?";
                        length++;
                    }
                    var newbyte = new Byte();
                    for (int i = 0, j = 0; i < length; i++)
                    {
                        var ch = pattern[i];
                        if (ch == '?') //wildcard
                        {
                            if (j == 0)
                                newbyte.N1.Wildcard = true;
                            else
                                newbyte.N2.Wildcard = true;
                        }
                        else //hex
                        {
                            if (j == 0)
                            {
                                newbyte.N1.Wildcard = false;
                                newbyte.N1.Data = (byte)(hexChToInt(ch) & 0xF);
                            }
                            else
                            {
                                newbyte.N2.Wildcard = false;
                                newbyte.N2.Data = (byte)(hexChToInt(ch) & 0xF);
                            }
                        }

                        j++;
                        if (j == 2)
                        {
                            j = 0;
                            result.Add(newbyte);
                        }
                    }
                    return result.ToArray();
                }

                public static bool Find(byte[] data, Byte[] pattern)
                {
                    long temp;
                    return Find(data, pattern, out temp);
                }

                private static bool matchByte(byte b, ref Byte p)
                {
                    if (!p.N1.Wildcard) //if not a wildcard we need to compare the data.
                    {
                        var n1 = b >> 4;
                        if (n1 != p.N1.Data) //if the data is not equal b doesn't match p.
                            return false;
                    }
                    if (!p.N2.Wildcard) //if not a wildcard we need to compare the data.
                    {
                        var n2 = b & 0xF;
                        if (n2 != p.N2.Data) //if the data is not equal b doesn't match p.
                            return false;
                    }
                    return true;
                }

                public static bool Find(byte[] data, Byte[] pattern, out long offsetFound, long offset = 0)
                {
                    offsetFound = -1;
                    if (data == null || pattern == null)
                        return false;
                    var patternSize = pattern.LongLength;
                    if (data.LongLength == 0 || patternSize == 0)
                        return false;

                    for (long i = offset, pos = 0; i < data.LongLength; i++)
                    {
                        if (matchByte(data[i], ref pattern[pos])) //check if the current data byte matches the current pattern byte
                        {
                            pos++;
                            if (pos == patternSize) //everything matched
                            {
                                offsetFound = i - patternSize + 1;
                                return true;
                            }
                        }
                        else //fix by Computer_Angel
                        {
                            i -= pos;
                            pos = 0; //reset current pattern position
                        }
                    }

                    return false;
                }

                public static bool FindAll(byte[] data, Byte[] pattern, out List<long> offsetsFound)
                {
                    offsetsFound = new List<long>();
                    long size = data.Length, pos = 0;
                    while (size > pos)
                    {
                        if (Find(data, pattern, out long offsetFound, pos))
                        {
                            offsetsFound.Add(offsetFound);
                            pos = offsetFound + pattern.Length;
                        }
                        else
                            break;
                    }
                    if (offsetsFound.Count > 0)
                        return true;
                    else
                        return false;
                }

                public static Signature[] Scan(byte[] data, Signature[] signatures)
                {
                    var found = new ConcurrentBag<Signature>();
                    Parallel.ForEach(signatures, signature =>
                    {
                        if (Find(data, signature.Pattern_SIG, out signature.FoundOffset))
                            found.Add(signature);
                    });
                    return found.ToArray();
                }
            }

            public class Signature
            {
                public Signature(string name, Pattern.Byte[] pattern)
                {
                    Name = name;
                    Pattern_SIG = pattern;
                    FoundOffset = -1;
                }

                public Signature(string name, string pattern)
                {
                    Name = name;
                    Pattern_SIG = Pattern.Transform(pattern);
                    FoundOffset = -1;
                }

                public string Name { get; private set; }
                public Pattern.Byte[] Pattern_SIG { get; private set; }
                public long FoundOffset;

                public override string ToString()
                {
                    return Name;
                }
            }

            protected void MemInfo(IntPtr pHandle)
            {
                ulong Addy = new ulong();
                while (true)
                {
                    MEMORY_BASIC_INFORMATION64 MemInfo = new MEMORY_BASIC_INFORMATION64();
                    int MemDump = VirtualQueryEx(pHandle, Addy, out MemInfo, Marshal.SizeOf(MemInfo));
                    if (MemDump == 0) break;
                    if ((MemInfo.State == State.MEM_COMMIT) && (MemInfo.Protect == AllocationProtect.PAGE_WRITECOPY || MemInfo.Protect == AllocationProtect.PAGE_READWRITE))
                    {
                        MemoryRegion.Add(MemInfo);
                    }
                    Addy = (MemInfo.BaseAddress + MemInfo.RegionSize);
                }
            }
            protected IntPtr Scan(byte[] sIn, byte[] sFor)
            {
                int[] sBytes = new int[256]; int Pool = 0;
                int End = sFor.Length - 1;
                for (int i = 0; i < 256; i++)
                    sBytes[i] = sFor.Length;
                for (int i = 0; i < End; i++)
                    sBytes[sFor[i]] = End - i;
                while (Pool <= sIn.Length - sFor.Length)
                {
                    for (int i = End; sIn[Pool + i] == sFor[i]; i--)
                        if (i == 0) return new IntPtr(Pool);
                    Pool += sBytes[sIn[Pool + End]];
                }
                return IntPtr.Zero;
            }
            public ulong AobScanOld(byte[] Pattern)
            {
                Process Game = Process.GetProcessById((int)this.ProcessID);
                if (Game.Id == 0) return ulong.MinValue;
                MemoryRegion = new List<MEMORY_BASIC_INFORMATION64>();
                MemInfo(Game.Handle);
                for (int i = 0; i < MemoryRegion.Count; i++)
                {
                    byte[] buff;
                    try
                    {
                        buff = new byte[MemoryRegion[i].RegionSize];
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    ReadProcessMemory(Game.Handle, MemoryRegion[i].BaseAddress, buff, MemoryRegion[i].RegionSize, 0);

                    ulong Result = (ulong)Scan(buff, Pattern).ToInt64();

                    if (Result != ulong.MinValue)
                        return MemoryRegion[i].BaseAddress + Result;
                }
                return ulong.MinValue;
            }

            private ConcurrentBag<UIntPtr> resultCollection = new ConcurrentBag<UIntPtr>();

            public List<UIntPtr> AobScan(string AOB)
            {
                Pattern.Byte[] pattern = Pattern.Transform(AOB);

                Process Game = Process.GetProcessById((int)this.ProcessID);
                if (Game.Id == 0) return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
                MemoryRegion = new List<MEMORY_BASIC_INFORMATION64>();
                MemInfo(Game.Handle);
                resultCollection = new ConcurrentBag<UIntPtr>();


                Parallel.ForEach(MemoryRegion, (item, loopstate) =>
                {
                    byte[] buff = new byte[0];
                    try
                    {
                        buff = new byte[item.RegionSize];
                    }
                    catch (Exception)
                    {
                    }

                    ReadProcessMemory(Game.Handle, item.BaseAddress, buff, item.RegionSize, 0);

                    long Result;

                    bool temp = Pattern.Find(buff, pattern, out Result);

                    if (temp)
                    {
                        resultCollection.Add((UIntPtr)(item.BaseAddress + (ulong)Result));
                    }
                });

                if (resultCollection.Count > 0)
                {
                    GC.Collect();
                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();
                    return new List<UIntPtr>(resultCollection);
                }

                return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
            }

            public List<UIntPtr> AobScan(byte[] AOB)
            {
                Process Game = Process.GetProcessById((int)this.ProcessID);
                if (Game.Id == 0) return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
                MemoryRegion = new List<MEMORY_BASIC_INFORMATION64>();
                MemInfo(Game.Handle);
                resultCollection = new ConcurrentBag<UIntPtr>();


                Parallel.ForEach(MemoryRegion, (item, loopstate) =>
                {
                    byte[] buff = new byte[0];
                    try
                    {
                        buff = new byte[item.RegionSize];
                    }
                    catch (Exception)
                    {
                    }

                    ReadProcessMemory(Game.Handle, item.BaseAddress, buff, item.RegionSize, 0);

                    ulong Result = (ulong)Scan(buff, AOB).ToInt64();

                    if (Result != ulong.MinValue)
                    {
                        resultCollection.Add((UIntPtr)(item.BaseAddress + Result));
                    }
                });

                if (resultCollection.Count > 0)
                {
                    GC.Collect();
                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();
                    return new List<UIntPtr>(resultCollection);
                }

                return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
            }

            /// <summary>
            /// Splits a List<T> into multiple chunks
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list">The list to be chunked</param>
            /// <param name="chunkSize">The size of each chunk</param>
            /// <returns>A list of chunks</returns>
            public static List<List<T>> SplitIntoChunks<T>(List<T> list, int chunkSize)
            {
                if (chunkSize <= 0)
                {
                    throw new ArgumentException("Chunk size must be greater than 0");
                }
                List<List<T>> retVal = new List<List<T>>();
                int index = 0;
                while (index < list.Count)
                {
                    int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                    retVal.Add(list.GetRange(index, count));
                    index += chunkSize;
                }
                return retVal;
            }

            public async Task<List<UIntPtr>> AobScan2(string AOB)
            {
                Pattern.Byte[] pattern = Pattern.Transform(AOB);

                Process Game = Process.GetProcessById((int)this.ProcessID);
                if (Game.Id == 0) return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
                MemoryRegion = new List<MEMORY_BASIC_INFORMATION64>();
                MemInfo(Game.Handle);
                resultCollection = new ConcurrentBag<UIntPtr>();

                //Console.WriteLine(MemoryRegion.Count());
                //MemoryRegion.RemoveAll(x => x.Protect == AllocationProtect.PAGE_NOACCESS);
                //Console.WriteLine(MemoryRegion.Count());
                //MemoryRegion.RemoveAll(x => x.State != State.MEM_COMMIT);
                //Console.WriteLine(MemoryRegion.Count());

                Console.WriteLine(MemoryRegion.Where(x => x.State != State.MEM_COMMIT).Count());

                List<List<MEMORY_BASIC_INFORMATION64>> ListOfChunks = SplitIntoChunks(MemoryRegion, 50);

                int a = 1;
                await Task.Run(async () =>
                {
                    foreach (List<MEMORY_BASIC_INFORMATION64> ChunkOfHundred in ListOfChunks)
                    {
                        new Thread(new ThreadStart(async () =>
                        {
                            Parallel.ForEach(ChunkOfHundred, (item, loopstate) =>
                            {

                                byte[] buff = new byte[0];
                                try
                                {
                                    buff = new byte[item.RegionSize];
                                }
                                catch (Exception)
                                {
                                }
                                ReadProcessMemory(Game.Handle, item.BaseAddress, buff, item.RegionSize, 0);

                                long Result;
                                bool temp = Pattern.Find(buff, pattern, out Result);

                                if (temp)
                                {
                                    resultCollection.Add((UIntPtr)(item.BaseAddress + (ulong)Result));
                                }
                            });
                            a++;
                        }), 1000).Start();
                    }
                });

            check_size:
                if (a == ListOfChunks.Count())
                {
                    GC.Collect();
                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();
                    return new List<UIntPtr>(resultCollection);
                }
                else
                    goto check_size;
            }

            public async Task<List<UIntPtr>> AobScan2(byte[] AOB)
            {
                Process Game = Process.GetProcessById((int)this.ProcessID);
                if (Game.Id == 0) return new List<UIntPtr>(new UIntPtr[] { UIntPtr.Zero });
                MemoryRegion = new List<MEMORY_BASIC_INFORMATION64>();
                MemInfo(Game.Handle);
                resultCollection = new ConcurrentBag<UIntPtr>();

                List<List<MEMORY_BASIC_INFORMATION64>> ListOfChunks = SplitIntoChunks(MemoryRegion, 50);

                int a = 1;
                await Task.Run(async () =>
                {
                    foreach (List<MEMORY_BASIC_INFORMATION64> ChunkOfHundred in ListOfChunks)
                    {
                        new Thread(new ThreadStart(async () =>
                        {
                            Parallel.ForEach(ChunkOfHundred, (item, loopstate) =>
                            {
                                byte[] buff = new byte[0];
                                try
                                {
                                    buff = new byte[item.RegionSize];
                                }
                                catch (Exception)
                                {
                                }

                                ReadProcessMemory(Game.Handle, item.BaseAddress, buff, item.RegionSize, 0);

                                //if (item.BaseAddress.ToString("X") == "271A5AE0000")
                                //{
                                //    Console.WriteLine("found");
                                //    List<byte> t = buff.ToList();
                                //    int[] ttt = Enumerable.Range(0, t.Count()).Where(i => t[i] == AOB[0]).ToArray();
                                //    int[] ttd = Enumerable.Range(0, t.Count()).Where(i => t[i] == AOB[1]).ToArray();
                                //    int vla = 0;
                                //    for (int i = 0; i < ttt.Length; i++)
                                //    {
                                //        if (true)
                                //        {

                                //        }
                                //    }

                                //    Console.WriteLine(ttt.Count());

                                //    ulong res = (ulong)Scan(buff, AOB).ToInt64();

                                //    Console.WriteLine((item.BaseAddress + res).ToString("X"));
                                //}
                                List<int> templist = new BoyerMoore(AOB).Search(buff).ToList();
                                templist.Sort();
                                templist.ForEach(x => resultCollection.Add((UIntPtr)((ulong)item.BaseAddress + (uint)x)));

                                //ulong Result = (ulong)Scan(buff, AOB).ToInt64();

                                //if (Result != ulong.MinValue)
                                //{
                                //    resultCollection.Add((UIntPtr)(item.BaseAddress + Result));
                                //}
                            });
                            a++;
                        }), 1000).Start();
                    }
                });

            check_size:
                if (a == ListOfChunks.Count())
                {
                    GC.Collect();
                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();
                    return new List<UIntPtr>(resultCollection);
                }
                else
                    goto check_size;
            }

            public bool CheckPattern(string pattern, byte[] array2check)
            {
                int len = array2check.Length;
                string[] strBytes = pattern.Split(' ');
                int x = 0;
                foreach (byte b in array2check)
                {
                    if (strBytes[x] == "?" || strBytes[x] == "??")
                    {
                        x++;
                    }
                    else if (byte.Parse(strBytes[x], NumberStyles.HexNumber) == b)
                    {
                        x++;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            public IntPtr ScanNew(byte[] memDump, string pattern)
            {
                string[] pBytes = pattern.Split(' ');
                try
                {
                    for (int y = 0; y < memDump.Length; y++)
                    {
                        if (memDump[y] == byte.Parse(pBytes[0], NumberStyles.HexNumber))
                        {
                            byte[] checkArray = new byte[pBytes.Length];
                            for (int x = 0; x < pBytes.Length; x++)
                            {
                                checkArray[x] = memDump[y + x];
                            }
                            if (CheckPattern(pattern, checkArray))
                            {
                                return (IntPtr)y;
                            }
                            else
                            {
                                y += pBytes.Length - (pBytes.Length / 2);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return IntPtr.Zero;
                }
                return IntPtr.Zero;
            }


            public sealed class BoyerMoore
            {
                readonly byte[] needle;
                readonly int[] charTable;
                readonly int[] offsetTable;

                public BoyerMoore(byte[] needle)
                {
                    this.needle = needle;
                    this.charTable = makeByteTable(needle);
                    this.offsetTable = makeOffsetTable(needle);
                }

                public IEnumerable<int> Search(byte[] haystack)
                {
                    if (needle.Length == 0)
                        yield break;

                    for (int i = needle.Length - 1; i < haystack.Length;)
                    {
                        int j;

                        for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
                        {
                            if (j != 0)
                                continue;

                            yield return i;
                            i += needle.Length - 1;
                            break;
                        }

                        i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
                    }
                }

                static int[] makeByteTable(byte[] needle)
                {
                    const int ALPHABET_SIZE = 256;
                    int[] table = new int[ALPHABET_SIZE];

                    for (int i = 0; i < table.Length; ++i)
                        table[i] = needle.Length;

                    for (int i = 0; i < needle.Length - 1; ++i)
                        table[needle[i]] = needle.Length - 1 - i;

                    return table;
                }

                static int[] makeOffsetTable(byte[] needle)
                {
                    int[] table = new int[needle.Length];
                    int lastPrefixPosition = needle.Length;

                    for (int i = needle.Length - 1; i >= 0; --i)
                    {
                        if (isPrefix(needle, i + 1))
                            lastPrefixPosition = i + 1;

                        table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
                    }

                    for (int i = 0; i < needle.Length - 1; ++i)
                    {
                        int slen = suffixLength(needle, i);
                        table[slen] = needle.Length - 1 - i + slen;
                    }

                    return table;
                }

                static bool isPrefix(byte[] needle, int p)
                {
                    for (int i = p, j = 0; i < needle.Length; ++i, ++j)
                        if (needle[i] != needle[j])
                            return false;

                    return true;
                }

                static int suffixLength(byte[] needle, int p)
                {
                    int len = 0;

                    for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
                        ++len;

                    return len;
                }
            }
        }
    }
}
