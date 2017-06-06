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

        private string GetMainWindowTitle() => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        public IntPtr MainWindowHandle => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        private bool CloseMainWindowCore() => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        public string MainWindowTitle => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        private bool IsRespondingCore() => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        public bool Responding => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);

        private bool WaitForInputIdleCore(int milliseconds) => throw new PlatformNotSupportedException(SR.Win32WindowsNotSupported);
    }
}
