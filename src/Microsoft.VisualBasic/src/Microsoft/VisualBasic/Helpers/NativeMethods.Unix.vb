' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Friend NotInheritable Class NativeMethods
        Friend Shared Function MoveFileEx(
             ByVal lpExistingFileName As String,
             ByVal lpNewFileName As String,
             ByVal dwFlags As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
            Throw New PlatformNotSupportedException()
        End Function
    End Class
End Namespace
