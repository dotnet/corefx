// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    public static class GlobalLog
    {
        public static void Assert(string message)
        {
        }

        public static void Print(string message)
        {
        }

        internal static void SetThreadSource(ThreadKinds source)
        {
        }

        public static bool IsEnabled { get { return false; } }
    }

    [Flags]
    internal enum ThreadKinds
    {
        Unknown = 0x0000,

        // Mutually exclusive.
        User = 0x0001,     // Thread has entered via an API.
        System = 0x0002,     // Thread has entered via a system callback (e.g. completion port) or is our own thread.

        // Mutually exclusive.
        Sync = 0x0004,     // Thread should block.
        Async = 0x0008,     // Thread should not block.

        // Mutually exclusive, not always known for a user thread.  Never changes.
        Timer = 0x0010,     // Thread is the timer thread.  (Can't call user code.)
        CompletionPort = 0x0020,     // Thread is a ThreadPool completion-port thread.
        Worker = 0x0040,     // Thread is a ThreadPool worker thread.
        Finalization = 0x0080,     // Thread is the finalization thread.
        Other = 0x0100,     // Unknown source.

        OwnerMask = User | System,
        SyncMask = Sync | Async,
        SourceMask = Timer | CompletionPort | Worker | Finalization | Other,

        // Useful "macros"
        SafeSources = SourceMask & ~(Timer | Finalization),  // Methods that "unsafe" sources can call must be explicitly marked.
        ThreadPool = CompletionPort | Worker,               // Like Thread.CurrentThread.IsThreadPoolThread
    }
}
