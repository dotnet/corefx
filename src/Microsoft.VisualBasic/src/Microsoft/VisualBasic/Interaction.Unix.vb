' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Namespace Microsoft.VisualBasic
    Partial Public Module Interaction
        Public Function MsgBox(ByVal Prompt As Object, Optional ByVal Buttons As MsgBoxStyle = MsgBoxStyle.OkOnly, Optional ByVal Title As Object = Nothing) As MsgBoxResult
            Throw New PlatformNotSupportedException("MsgBox")
        End Function
    End Module
End Namespace
