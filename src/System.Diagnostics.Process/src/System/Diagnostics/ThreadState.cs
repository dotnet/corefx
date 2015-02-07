// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Diagnostics
{
    /// <devdoc>
    ///     Specifies the execution state of a thread.
    /// </devdoc>
    public enum ThreadState
    {
        /// <devdoc>
        ///     The thread has been initialized, but has not started yet.
        /// </devdoc>
        Initialized,

        /// <devdoc>
        ///     The thread is in ready state.
        /// </devdoc>
        Ready,

        /// <devdoc>
        ///     The thread is running.
        /// </devdoc>
        Running,

        /// <devdoc>
        ///     The thread is in standby state.
        /// </devdoc>
        Standby,

        /// <devdoc>
        ///     The thread has exited.
        /// </devdoc>
        Terminated,

        /// <devdoc>
        ///     The thread is waiting.
        /// </devdoc>
        Wait,

        /// <devdoc>
        ///     The thread is transitioning between states.
        /// </devdoc>
        Transition,

        /// <devdoc>
        ///     The thread state is unknown.
        /// </devdoc>
        Unknown
    }
}
