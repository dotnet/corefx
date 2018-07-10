// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace System.Threading
{
    public partial class EventWaitHandle : WaitHandle
    {
        public EventWaitHandle(bool initialState, EventResetMode mode) :
            this(initialState, mode, null, out _)
        {
        }

        public EventWaitHandle(bool initialState, EventResetMode mode, string name) :
            this(initialState, mode, name, out _)
        {
        }

        public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
        {
            if (mode != EventResetMode.AutoReset && mode != EventResetMode.ManualReset)
                throw new ArgumentException(SR.Argument_InvalidFlag, nameof(mode));

            CreateEventCore(initialState, mode, name, out createdNew);
        }

        public static EventWaitHandle OpenExisting(string name)
        {
            EventWaitHandle result;
            switch (OpenExistingWorker(name, out result))
            {
                case OpenExistingResult.NameNotFound:
                    throw new WaitHandleCannotBeOpenedException();
                case OpenExistingResult.NameInvalid:
                    throw new WaitHandleCannotBeOpenedException(SR.Format(SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
                case OpenExistingResult.PathNotFound:
                    throw new DirectoryNotFoundException(SR.Format(SR.IO_PathNotFound_Path, name));
                default:
                    return result;
            }
        }

        public static bool TryOpenExisting(string name, out EventWaitHandle result)
        {
            return OpenExistingWorker(name, out result) == OpenExistingResult.Success;
        }
    }
}
