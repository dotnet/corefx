// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public static class DynamicMethodJumpStubTests
    {
        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void DynamicMethodJumpStubTest()
        {
            if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            {
                return;
            }

            // Reserve memory around framework libraries. This is just a best attempt, it typically doesn't help since the
            // precode allocator may have already committed pages it can allocate from, or it may commit reserved pages close to
            // framework libraries.
            ReserveMemoryAround(new Action(ExecutionContext.RestoreFlow).Method.MethodHandle);

            for (int i = 0; i < 64; ++i)
            {
                DynamicMethod dynamicMethod = CreateDynamicMethod("DynMethod" + i);
                Action dynamicMethodDelegate = (Action)dynamicMethod.CreateDelegate(typeof(Action));

                // Before compiling the dynamic method, reserve memory around its current entry point, which should be its
                // precode. Then, when compiling the method, there would be a good chance that the code will be located far from
                // the precode, forcing the use of a jump stub.
                ReserveMemoryAround(
                    (RuntimeMethodHandle)
                    typeof(DynamicMethod).InvokeMember(
                        "GetMethodDescriptor",
                        BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                        null,
                        dynamicMethod,
                        null));

                dynamicMethodDelegate();
            }

            // This test does not release reserved pages because they may have been committed by other components on the system
        }

        private static DynamicMethod CreateDynamicMethod(string name)
        {
            var dynamicMethod = new DynamicMethod(name, null, null);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ret);
            return dynamicMethod;
        }

        private const uint AllocationGranularity = (uint)64 << 10;
        private const ulong ReserveRangeRadius = (ulong)4 << 30; // reserve 4 GB before and after the base address

        private static void ReserveMemoryAround(RuntimeMethodHandle methodHandle)
        {
            ulong baseAddress = (ulong)methodHandle.Value.ToInt64();

            ulong low = baseAddress - ReserveRangeRadius;
            if (low > baseAddress)
            {
                low = ulong.MinValue;
            }
            else
            {
                low &= ~((ulong)AllocationGranularity - 1);
            }

            ulong high = baseAddress + ReserveRangeRadius;
            if (high < baseAddress)
            {
                high = ulong.MaxValue;
            }

            for (ulong address = low; address <= high; address += AllocationGranularity)
            {
                VirtualAlloc(
                    new UIntPtr(address),
                    new UIntPtr(AllocationGranularity),
                    AllocationType.RESERVE,
                    MemoryProtection.NOACCESS).ToUInt64();
            }
        }

        [Flags]
        private enum AllocationType : uint
        {
            COMMIT = 0x1000,
            RESERVE = 0x2000,
            RESET = 0x80000,
            LARGE_PAGES = 0x20000000,
            PHYSICAL = 0x400000,
            TOP_DOWN = 0x100000,
            WRITE_WATCH = 0x200000
        }

        [Flags]
        private enum MemoryProtection : uint
        {
            EXECUTE = 0x10,
            EXECUTE_READ = 0x20,
            EXECUTE_READWRITE = 0x40,
            EXECUTE_WRITECOPY = 0x80,
            NOACCESS = 0x01,
            READONLY = 0x02,
            READWRITE = 0x04,
            WRITECOPY = 0x08,
            GUARD_Modifierflag = 0x100,
            NOCACHE_Modifierflag = 0x200,
            WRITECOMBINE_Modifierflag = 0x400
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr VirtualAlloc(
            UIntPtr lpAddress,
            UIntPtr dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);
    }
}
