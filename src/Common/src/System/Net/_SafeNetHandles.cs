/*++
Copyright (c) Microsoft Corporation

Module Name:

    _SafeNetHandles.cs

Abstract:
        The file contains _all_ SafeHandles implementations for System.Net namespace.
        These handle wrappers do guarantee that OS resources get cleaned up when the app domain dies.

        All PInvoke declarations that do freeing  the  OS resources  _must_ be in this file
        All PInvoke declarations that do allocation the OS resources _must_ be in this file


Details:

        The protection from leaking OF the OS resources is based on two technologies
        1) SafeHandle class
        2) Non interuptible regions using Constrained Execution Region (CER) technology

        For simple cases SafeHandle class does all the job. The Prerequisites are:
        - A resource is able to be represented by IntPtr type (32 bits on 32 bits platforms).
        - There is a PInvoke availble that does the creation of the resource.
          That PInvoke either returns the handle value or it writes the handle into out/ref parameter.
        - The above PInvoke as part of the call does NOT free any OS resource.

        For those "simple" cases we desinged SafeHandle-derived classes that provide
        static methods to allocate a handle object.
        Each such derived class provides a handle release method that is run as non-interrupted.

        For more complicated cases we employ the support for non-interruptible methods (CERs).
        Each CER is a tree of code rooted at a catch or finally clause for a specially marked exception
        handler (preceded by the RuntimeHelpers.PrepareConstrainedRegions() marker) or the Dispose or
        ReleaseHandle method of a SafeHandle derived class. The graph is automatically computed by the
        runtime (typically at the jit time of the root method), but cannot follow virtual or interface
        calls (these must be explicitly prepared via RuntimeHelpers.PrepareMethod once the definite target
        method is known).

        An example of the top-level of a CER:

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                // Normal code
            }
            finally
            {
                // Guaranteed to get here even in low memory scenarios. Thread abort will not interrupt
                // this clause and we won't fail because of a jit allocation of any method called (modulo
                // restrictions on interface/virtual calls listed above and further restrictions listed
                // below).
            }

        Another common pattern is an empty-try (where you really just want a region of code the runtime
        won't interrupt you in):

            RuntimeHelpers.PrepareConstrainedRegions();
            try {} finally
            {
                // Non-interruptible code here
            }

        This ugly syntax will be supplanted with compiler support at some point.

        While within a CER region certain restrictions apply in order to avoid having the runtime inject
        a potential fault point into your code (and of course you're are responsible for ensuring your
        code doesn't inject any explicit fault points of its own unless you know how to tolerate them).

        A quick and dirty guide to the possible causes of fault points in CER regions:
        - Explicit allocations (though allocating a value type only implies allocation on the stack,
          which may not present an issue).
        - Boxing a value type (C# does this implicitly for you in many cases, so be careful).
        - Use of Monitor.Enter or the lock keyword.
        - Accessing a multi-dimensional array.
        - Calling any method outside your control that doesn't make a guarantee that it doesn't introduce failure points.
        - Making P/Invoke calls with non-blittable parameters types. Blittable types are:
            - SafeHandle when used as an [in] parameter
            - NON BOXED base types that fit onto a machine word
            - ref struct with blittable fields
            - class type with blittable fields
            - pinned Unicode strings using "fixed" statement
            - pointers of any kind
            - IntPtr type
        - P/Invokes should not have any CharSet attribute on it's declaration.
          Obvioulsy string types should not appear in the parameters.
        - String type MUST not appear in a field of a marshaled ref struct or class in a P?Invoke

Author:

    Alexei Vopilov    04-Sept-2002

Revision History:

--*/

namespace System.Net {
    using Microsoft.Win32.SafeHandles;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    
#if DEBUG
    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugSafeHandle: SafeHandleZeroOrMinusOneIsInvalid {
        string m_Trace;

        protected DebugSafeHandle(bool ownsHandle): base(ownsHandle) {
            Trace();
        }

        protected DebugSafeHandle(IntPtr invalidValue, bool ownsHandle): base(ownsHandle) {
            SetHandle(invalidValue);
            Trace();
        }

        private void Trace() {
            m_Trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be excplicitly closed) \r\n";
#if TRAVE
            string stacktrace = Environment.StackTrace;
            m_Trace += stacktrace;
#endif //TRAVE
        }

        ~DebugSafeHandle() {
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            GlobalLog.Print(m_Trace);
        }
    }

    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugCriticalHandleMinusOneIsInvalid : CriticalHandleMinusOneIsInvalid {
        string m_Trace;

        protected DebugCriticalHandleMinusOneIsInvalid(): base() {
            Trace();
        }

        private void Trace() {
            m_Trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be excplicitly closed) \r\n";
            GlobalLog.Print("Creating SafeHandle, type = " + this.GetType().FullName);
#if TRAVE
            string stacktrace = Environment.StackTrace;
            m_Trace += stacktrace;
#endif //TRAVE
        }

        ~DebugCriticalHandleMinusOneIsInvalid() {
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            GlobalLog.Print(m_Trace);
        }
    }

    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugSafeHandleMinusOneIsInvalid : SafeHandleMinusOneIsInvalid {
        string m_Trace;

        protected DebugSafeHandleMinusOneIsInvalid(bool ownsHandle): base(ownsHandle) {
            Trace();
        }

        private void Trace() {
            m_Trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be excplicitly closed) \r\n";
            GlobalLog.Print("Creating SafeHandle, type = " + this.GetType().FullName);
#if TRAVE
            string stacktrace = Environment.StackTrace;
            m_Trace += stacktrace;
#endif //TRAVE
        }

        ~DebugSafeHandleMinusOneIsInvalid() {
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            GlobalLog.Print(m_Trace);
        }
    }

    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugCriticalHandleZeroOrMinusOneIsInvalid : CriticalHandleZeroOrMinusOneIsInvalid {
        string m_Trace;

        protected DebugCriticalHandleZeroOrMinusOneIsInvalid(): base() {
            Trace();
        }

        private void Trace() {
            m_Trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be excplicitly closed) \r\n";
            GlobalLog.Print("Creating SafeHandle, type = " + this.GetType().FullName);
#if TRAVE
            string stacktrace = Environment.StackTrace;
            m_Trace += stacktrace;
#endif //TRAVE
        }

        ~DebugCriticalHandleZeroOrMinusOneIsInvalid() {
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            GlobalLog.Print(m_Trace);
        }
    }
#endif // DEBUG


#if !PROJECTN

    ///////////////////////////////////////////////////////////////
    //
    // This is implementation of Safe AllocHGlobal which is turned out
    // to be LocalAlloc down in CLR
    //
    ///////////////////////////////////////////////////////////////
#if DEBUG
    internal sealed class SafeLocalFree : DebugSafeHandle {
#else
    internal sealed class SafeLocalFree : SafeHandleZeroOrMinusOneIsInvalid {
#endif
        private const int LMEM_FIXED = 0;
        private const int NULL = 0;

        // This returned handle cannot be modified by the application.
        public static SafeLocalFree Zero = new SafeLocalFree(false);

        private SafeLocalFree() : base(true) {}

        private SafeLocalFree(bool ownsHandle) : base(ownsHandle) {}

        public static SafeLocalFree LocalAlloc(int cb) {
            SafeLocalFree result = UnsafeCommonNativeMethods.LocalAlloc(LMEM_FIXED, (UIntPtr) cb);
            if (result.IsInvalid) {
                result.SetHandleAsInvalid();
                throw new OutOfMemoryException();
            }
            return result;
        }

        override protected bool ReleaseHandle()
        {
            return UnsafeCommonNativeMethods.LocalFree(handle) == IntPtr.Zero;
        }
    }
#endif //!PROJECTN
}
