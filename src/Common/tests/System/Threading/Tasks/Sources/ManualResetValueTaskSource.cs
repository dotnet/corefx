// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Threading.Tasks.Sources
{
    public sealed class ManualResetValueTaskSource<T> : IValueTaskSource<T>, IValueTaskSource
    {
        private ManualResetValueTaskSourceLogic<T> _logic; // mutable struct; do not make this readonly

        public bool RunContinuationsAsynchronously { get => _logic.RunContinuationsAsynchronously; set => _logic.RunContinuationsAsynchronously = value; }
        public short Version => _logic.Version;
        public void Reset() => _logic.Reset();
        public void SetResult(T result) => _logic.SetResult(result);
        public void SetException(Exception error) => _logic.SetException(error);

        public T GetResult(short token) => _logic.GetResult(token);
        void IValueTaskSource.GetResult(short token) => _logic.GetResult(token);
        public ValueTaskSourceStatus GetStatus(short token) => _logic.GetStatus(token);
        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags) => _logic.OnCompleted(continuation, state, token, flags);
    }
}
