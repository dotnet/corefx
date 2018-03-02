using System.Diagnostics;
using System.Threading;

namespace System.IO.Pipelines
{
    internal class SynchronizationContextPipeScheduler : PipeScheduler
    {
        public override void Schedule<TState>(Action<TState> action, TState state)
        {
            Debug.Assert(SynchronizationContext.Current != null);

            SynchronizationContext.Current.Post(state => action(state), null);
        }
    }
}
