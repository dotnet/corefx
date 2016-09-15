// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Threading
{
    public sealed partial class Timer : System.IDisposable
    {
        public Timer(System.Threading.TimerCallback callback) { }
        public Timer(System.Threading.TimerCallback callback, object state, int dueTime, int period) { }
        public Timer(System.Threading.TimerCallback callback, object state, long dueTime, long period) { }
        public Timer(System.Threading.TimerCallback callback, object state, System.TimeSpan dueTime, System.TimeSpan period) { }
        [System.CLSCompliantAttribute(false)]
        public Timer(System.Threading.TimerCallback callback, object state, uint dueTime, uint period) { }
        public bool Change(int dueTime, int period) { return default(bool); }
        public bool Change(long dueTime, long period) { return default(bool); }
        public bool Change(System.TimeSpan dueTime, System.TimeSpan period) { return default(bool); }
        [System.CLSCompliantAttribute(false)]
        public bool Change(uint dueTime, uint period) { return default(bool); }
        public void Dispose() { }
        public bool Dispose(System.Threading.WaitHandle notifyObject) { return default(bool); }
    }
}
