// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Threading.Tasks.Tests
{
    /// <summary>A scheduler that queues to the TP and tracks the number of times QueueTask and TryExecuteTaskInline are invoked.</summary>
    internal class QUWITaskScheduler : TaskScheduler
    {
        private int _queueTaskCount;
        private int _tryExecuteTaskInlineCount;

        public int QueueTaskCount { get { return _queueTaskCount; } }
        public int TryExecuteTaskInlineCount { get { return _tryExecuteTaskInlineCount; } }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return null;
        }

        protected override void QueueTask(Task task)
        {
            Interlocked.Increment(ref _queueTaskCount);
            Task.Run(() => TryExecuteTask(task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            Interlocked.Increment(ref _tryExecuteTaskInlineCount);
            return TryExecuteTask(task);
        }
    }
}
