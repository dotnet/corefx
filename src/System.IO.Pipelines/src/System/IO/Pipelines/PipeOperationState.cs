// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    [DebuggerDisplay("State: {_state}")]
    internal struct PipeOperationState
    {
        private State _state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginRead()
        {
            if ((_state & State.BeginRead) == State.BeginRead)
            {
                ThrowHelper.ThrowInvalidOperationException_AlreadyReading();
            }

            _state &= ~State.EndRead;
            _state |= State.BeginRead;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginReadTentative()
        {
            if ((_state & State.BeginRead) == State.BeginRead)
            {
                ThrowHelper.ThrowInvalidOperationException_AlreadyReading();
            }

            _state &= ~State.EndRead;
            _state |= State.BeginReadTenative;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndRead()
        {
            if ((_state & State.EndRead) == State.EndRead)
            {
                ThrowHelper.ThrowInvalidOperationException_NoReadToComplete();
            }

            _state &= ~(State.BeginRead | State.BeginReadTenative);
            _state |= State.EndRead;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginWrite()
        {
            _state |= State.WritingActive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndWrite()
        {
            _state &= ~State.WritingActive;
        }

        public bool IsWritingActive => (_state & State.WritingActive) == State.WritingActive;

        public bool IsReadingActive => (_state & State.BeginRead) == State.BeginRead;

        [Flags]
        internal enum State
        {
            BeginRead = 1,
            BeginReadTenative = 2,
            EndRead = 4,
            WritingActive = 8
        }
    }
}
