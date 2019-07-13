// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    internal class Gate : IValueTaskSource
    {
        private readonly static Action<object> s_cancelledEvent = (o) => ((Gate)o).OnCancel();

        private ManualResetValueTaskSourceCore<VoidResult> _mrvts;
        private CancellationTokenRegistration _cancellationRegistration;
        private CancellationToken _cancellationToken;

        public ValueTask WaitAsync(CancellationToken cancellationToken)
        {
            _cancellationRegistration = cancellationToken.UnsafeRegister(s_cancelledEvent, this);
            _cancellationToken = cancellationToken;

            return new ValueTask(this, _mrvts.Version);
        }

        public void Release()
            => _mrvts.SetResult(default);

        private void OnCancel()
            => _mrvts.SetException(new OperationCanceledException(_cancellationToken));

        public void GetResult(short token)
        {
            _mrvts.GetResult(token);
            _mrvts.Reset();
            _cancellationRegistration.Dispose();
            _cancellationToken = default;
        }

        public ValueTaskSourceStatus GetStatus(short token)
            => _mrvts.GetStatus(token);

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
            => _mrvts.OnCompleted(continuation, state, token, flags);

        private struct VoidResult
        {
        }
    }
}
