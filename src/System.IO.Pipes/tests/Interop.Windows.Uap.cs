// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes.Tests
{
    /// <summary>
    /// The class contains interop declarations and helpers methods for them.
    /// </summary>
    internal static partial class Interop
    {
        private static bool TryHandleGetImpersonationUserNameError(int error, int userNameMaxLength, out string userName)
        {
            // Uap does not allow loading libraries
            userName = string.Empty;
            return false;
        }
    }
}
