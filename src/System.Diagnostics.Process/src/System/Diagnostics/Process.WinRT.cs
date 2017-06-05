// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class Process : IDisposable
    {
        private bool StartCore(ProcessStartInfo startInfo)
        {
            return startInfo.UseShellExecute
                ? throw new PlatformNotSupportedException(SR.UseShellExecuteNotSupported)
                : StartWithCreateProcess(startInfo);
        }

        private string GetMainWindowTitle()
        {
            throw new PlatformNotSupportedException();
        }

        public IntPtr MainWindowHandle
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        private bool CloseMainWindowCore()
        {
            throw new PlatformNotSupportedException();
        }

        public string MainWindowTitle
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        private bool IsRespondingCore()
        {
            throw new PlatformNotSupportedException();
        }

        public bool Responding
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        private bool WaitForInputIdleCore(int milliseconds)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
