// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    internal sealed class ThreadPoolScheduler : PipeScheduler
    {
        public override void Schedule<TState>(Action<TState> action, TState state)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(state =>
            {
                ((ActionObjectAsWaitCallback<TState>)state).Run();
            }, 
            new ActionObjectAsWaitCallback<TState>(action, state));
        }

        private sealed class ActionObjectAsWaitCallback<TState>
        {
            private Action<TState> _action;
            private TState _state;

            public ActionObjectAsWaitCallback(Action<TState> action, TState state)
            {
                _action = action;
                _state = state;
            }

            public void Run() => _action(_state);
        }
    }
}
