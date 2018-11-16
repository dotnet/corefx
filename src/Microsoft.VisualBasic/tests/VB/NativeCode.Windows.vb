' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On
Imports System.Reflection
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.Tests.VB
    Public Module NativeCode
        <DllImport("ntdll", ExactSpelling:=True)>
        Public Function RtlAreLongPathsEnabled() As Boolean
        End Function

        Public Function HasLegacyIoBehavior(propertyName As String) As Boolean
            Dim t As Type = GetType(Object).Assembly.GetType("System.AppContextSwitches")
            Dim p = t.GetProperty(propertyName, BindingFlags.Static Or BindingFlags.Public)
            ' If the switch actually exists use it, otherwise we predate the switch and are effectively on
            Return CBool(If(p?.GetValue(Nothing), True))
        End Function
    End Module
End Namespace
