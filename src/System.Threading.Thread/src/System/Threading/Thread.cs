// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

extern alias System_Collections;
extern alias System_Runtime;
extern alias System_Runtime_Extensions;
extern alias System_Security_Principal;
extern alias System_Threading;

using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using Internal.Runtime.Augments;

namespace System.Threading
{
    using AppDomain = System_Runtime_Extensions::System.AppDomain;
    using IPrincipal = System_Security_Principal::System.Security.Principal.IPrincipal;
    using LocalDataStoreSlot = System_Runtime::System.LocalDataStoreSlot;

    public sealed class Thread : CriticalFinalizerObject
    {
        [ThreadStatic]
        private static Thread t_currentThread;

        private readonly RuntimeThread m_runtimeThread;
        private Delegate m_start;
        private IPrincipal m_principal;

        private Thread(RuntimeThread runtimeThread)
        {
            Debug.Assert(runtimeThread != null);
            m_runtimeThread = runtimeThread;
        }

        public Thread(ThreadStart start)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            m_runtimeThread = RuntimeThread.Create(ThreadMain_ThreadStart);
            Debug.Assert(m_runtimeThread != null);
            m_start = start;
        }

        public Thread(ThreadStart start, int maxStackSize)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            if (maxStackSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxStackSize), SR.ArgumentOutOfRange_NeedNonnegativeNumber);
            }

            m_runtimeThread = RuntimeThread.Create(ThreadMain_ThreadStart, maxStackSize);
            Debug.Assert(m_runtimeThread != null);
            m_start = start;
        }

        public Thread(ParameterizedThreadStart start)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            m_runtimeThread = RuntimeThread.Create(ThreadMain_ParameterizedThreadStart);
            Debug.Assert(m_runtimeThread != null);
            m_start = start;
        }

        public Thread(ParameterizedThreadStart start, int maxStackSize)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            if (maxStackSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxStackSize), SR.ArgumentOutOfRange_NeedNonnegativeNumber);
            }

            m_runtimeThread = RuntimeThread.Create(ThreadMain_ParameterizedThreadStart, maxStackSize);
            Debug.Assert(m_runtimeThread != null);
            m_start = start;
        }

        private void ThreadMain_ThreadStart()
        {
            t_currentThread = this;

            var start = m_start;
            m_start = null;
            Debug.Assert(start is ThreadStart);
            ((ThreadStart)start)();
        }

        private void ThreadMain_ParameterizedThreadStart(object parameter)
        {
            t_currentThread = this;

            var start = m_start;
            m_start = null;
            Debug.Assert(start is ParameterizedThreadStart);
            ((ParameterizedThreadStart)start)(parameter);
        }

        public static Thread CurrentThread
        {
            get
            {
                var currentThread = t_currentThread;
                if (currentThread == null)
                {
                    t_currentThread = currentThread = new Thread(RuntimeThread.CurrentThread);
                }
                return currentThread;
            }
        }

        private void RequireCurrentThread()
        {
            if (this != CurrentThread)
            {
                throw new InvalidOperationException(SR.Thread_Operation_RequiresCurrentThread);
            }
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                RequireCurrentThread();
                return CultureInfo.CurrentCulture;
            }
            set
            {
                RequireCurrentThread();
                CultureInfo.CurrentCulture = value;
            }
        }

        public CultureInfo CurrentUICulture
        {
            get
            {
                RequireCurrentThread();
                return CultureInfo.CurrentUICulture;
            }
            set
            {
                RequireCurrentThread();
                CultureInfo.CurrentUICulture = value;
            }
        }

        public static IPrincipal CurrentPrincipal
        {
            get
            {
                return CurrentThread.m_principal;
            }
            set
            {
                CurrentThread.m_principal = value;
            }
        }

        public ExecutionContext ExecutionContext => ExecutionContext.Capture();
        public bool IsAlive => m_runtimeThread.IsAlive;
        public bool IsBackground { get { return m_runtimeThread.IsBackground; } set { m_runtimeThread.IsBackground = value; } }
        public bool IsThreadPoolThread => m_runtimeThread.IsThreadPoolThread;
        public int ManagedThreadId => m_runtimeThread.ManagedThreadId;
        public string Name { get { return m_runtimeThread.Name; } set { m_runtimeThread.Name = value; } }
        public ThreadPriority Priority { get { return m_runtimeThread.Priority; } set { m_runtimeThread.Priority = value; } }
        public ThreadState ThreadState => m_runtimeThread.ThreadState;

        public void Abort()
        {
            throw new PlatformNotSupportedException();
        }

        public void Abort(object stateInfo)
        {
            throw new PlatformNotSupportedException();
        }

        public static void ResetAbort()
        {
            throw new PlatformNotSupportedException();
        }

        public void Suspend()
        {
            throw new PlatformNotSupportedException();
        }

        public void Resume()
        {
            throw new PlatformNotSupportedException();
        }

        // Currently, no special handling is done for critical regions, and no special handling is necessary to ensure thread
        // affinity. If that changes, the relevant functions would instead need to delegate to RuntimeThread.
        public static void BeginCriticalRegion() { }
        public static void EndCriticalRegion() { }
        public static void BeginThreadAffinity() { }
        public static void EndThreadAffinity() { }

        public static LocalDataStoreSlot AllocateDataSlot() => LocalDataStore.AllocateSlot();
        public static LocalDataStoreSlot AllocateNamedDataSlot(string name) => LocalDataStore.AllocateNamedSlot(name);
        public static LocalDataStoreSlot GetNamedDataSlot(string name) => LocalDataStore.GetNamedSlot(name);
        public static void FreeNamedDataSlot(string name) => LocalDataStore.FreeNamedSlot(name);
        public static object GetData(LocalDataStoreSlot slot) => LocalDataStore.GetData(slot);
        public static void SetData(LocalDataStoreSlot slot, object data) => LocalDataStore.SetData(slot, data);

        [Obsolete("The ApartmentState property has been deprecated.  Use GetApartmentState, SetApartmentState or TrySetApartmentState instead.", false)]
        public ApartmentState ApartmentState
        {
            get
            {
                return GetApartmentState();
            }
            set
            {
                TrySetApartmentState(value);
            }
        }

        public ApartmentState GetApartmentState()
        {
#if PLATFORM_UNIX
            return ApartmentState.Unknown;
#else // !PLATFORM_UNIX
            return m_runtimeThread.GetApartmentState();
#endif // PLATFORM_UNIX
        }

        public void SetApartmentState(ApartmentState state)
        {
            if (!TrySetApartmentState(state))
            {
#if PLATFORM_UNIX
                throw new PlatformNotSupportedException();
#else // !PLATFORM_UNIX
                throw new InvalidOperationException(SR.Thread_ApartmentState_ChangeFailed);
#endif // PLATFORM_UNIX
            }
        }

        public bool TrySetApartmentState(ApartmentState state)
        {
            switch (state)
            {
                case ApartmentState.STA:
                case ApartmentState.MTA:
                case ApartmentState.Unknown:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_Enum, nameof(state));
            }

#if PLATFORM_UNIX
            return state == GetApartmentState();
#else // !PLATFORM_UNIX
            return m_runtimeThread.TrySetApartmentState(state);
#endif // PLATFORM_UNIX
        }

        private static int ToTimeoutMilliseconds(TimeSpan timeout)
        {
            var timeoutMilliseconds = (long)timeout.TotalMilliseconds;
            if (timeoutMilliseconds < -1 || timeoutMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), SR.ArgumentOutOfRange_TimeoutMilliseconds);
            }
            return (int)timeoutMilliseconds;
        }

        public static AppDomain GetDomain() => AppDomain.CurrentDomain;
        public static int GetDomainID() => GetDomain().Id;
        public override int GetHashCode() => ManagedThreadId;
        public void Interrupt() => m_runtimeThread.Interrupt();
        public void Join() => m_runtimeThread.Join();
        public bool Join(int millisecondsTimeout) => m_runtimeThread.Join(millisecondsTimeout);
        public bool Join(TimeSpan timeout) => Join(ToTimeoutMilliseconds(timeout));
        public static void MemoryBarrier() => Interlocked.MemoryBarrier();
        public static void Sleep(int millisecondsTimeout) => RuntimeThread.Sleep(millisecondsTimeout);
        public static void Sleep(TimeSpan timeout) => Sleep(ToTimeoutMilliseconds(timeout));
        public static void SpinWait(int iterations) => RuntimeThread.SpinWait(iterations);
        public static bool Yield() => false; // TODO: RuntimeThread.Yield();
        public void Start() => m_runtimeThread.Start();
        public void Start(object parameter) => m_runtimeThread.Start(parameter);

        public static byte VolatileRead(ref byte address) => Volatile.Read(ref address);
        public static double VolatileRead(ref double address) => Volatile.Read(ref address);
        public static short VolatileRead(ref short address) => Volatile.Read(ref address);
        public static int VolatileRead(ref int address) => Volatile.Read(ref address);
        public static long VolatileRead(ref long address) => Volatile.Read(ref address);
        public static IntPtr VolatileRead(ref IntPtr address) => Volatile.Read(ref address);
        public static object VolatileRead(ref object address) => Volatile.Read(ref address);
        [CLSCompliant(false)]
        public static sbyte VolatileRead(ref sbyte address) => Volatile.Read(ref address);
        public static float VolatileRead(ref float address) => Volatile.Read(ref address);
        [CLSCompliant(false)]
        public static ushort VolatileRead(ref ushort address) => Volatile.Read(ref address);
        [CLSCompliant(false)]
        public static uint VolatileRead(ref uint address) => Volatile.Read(ref address);
        [CLSCompliant(false)]
        public static ulong VolatileRead(ref ulong address) => Volatile.Read(ref address);
        [CLSCompliant(false)]
        public static UIntPtr VolatileRead(ref UIntPtr address) => Volatile.Read(ref address);
        public static void VolatileWrite(ref byte address, byte value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref double address, double value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref short address, short value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref int address, int value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref long address, long value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref IntPtr address, IntPtr value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref object address, object value) => Volatile.Write(ref address, value);
        [CLSCompliant(false)]
        public static void VolatileWrite(ref sbyte address, sbyte value) => Volatile.Write(ref address, value);
        public static void VolatileWrite(ref float address, float value) => Volatile.Write(ref address, value);
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ushort address, ushort value) => Volatile.Write(ref address, value);
        [CLSCompliant(false)]
        public static void VolatileWrite(ref uint address, uint value) => Volatile.Write(ref address, value);
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ulong address, ulong value) => Volatile.Write(ref address, value);
        [CLSCompliant(false)]
        public static void VolatileWrite(ref UIntPtr address, UIntPtr value) => Volatile.Write(ref address, value);

        /// <summary>
        /// Manages functionality required to support members of <see cref="Thread"/> dealing with thread-local data
        /// </summary>
        private static class LocalDataStore
        {
            private static Dictionary<string, LocalDataStoreSlot> s_nameToSlotMap;

            public static LocalDataStoreSlot AllocateSlot()
            {
                return new LocalDataStoreSlot(new ThreadLocal<object>());
            }

            public static Dictionary<string, LocalDataStoreSlot> EnsureNameToSlotMap()
            {
                var nameToSlotMap = s_nameToSlotMap;
                if (nameToSlotMap != null)
                {
                    return nameToSlotMap;
                }

                nameToSlotMap = new Dictionary<string, LocalDataStoreSlot>();
                return Interlocked.CompareExchange(ref s_nameToSlotMap, nameToSlotMap, null) ?? nameToSlotMap;
            }

            public static LocalDataStoreSlot AllocateNamedSlot(string name)
            {
                var slot = AllocateSlot();
                var nameToSlotMap = EnsureNameToSlotMap();
                lock (nameToSlotMap)
                {
                    nameToSlotMap.Add(name, slot);
                }
                return slot;
            }

            public static LocalDataStoreSlot GetNamedSlot(string name)
            {
                var nameToSlotMap = EnsureNameToSlotMap();
                lock (nameToSlotMap)
                {
                    LocalDataStoreSlot slot;
                    if (!nameToSlotMap.TryGetValue(name, out slot))
                    {
                        slot = AllocateSlot();
                        nameToSlotMap[name] = slot;
                    }
                    return slot;
                }
            }

            public static void FreeNamedSlot(string name)
            {
                var nameToSlotMap = EnsureNameToSlotMap();
                lock (nameToSlotMap)
                {
                    nameToSlotMap.Remove(name);
                }
            }

            private static ThreadLocal<object> GetThreadLocal(LocalDataStoreSlot slot)
            {
                if (slot == null)
                {
                    throw new ArgumentNullException(nameof(slot));
                }

                Debug.Assert(slot.Data is ThreadLocal<object>);
                return (ThreadLocal<object>)slot.Data;
            }

            public static object GetData(LocalDataStoreSlot slot)
            {
                return GetThreadLocal(slot).Value;
            }

            public static void SetData(LocalDataStoreSlot slot, object value)
            {
                GetThreadLocal(slot).Value = value;
            }
        }
    }
}
