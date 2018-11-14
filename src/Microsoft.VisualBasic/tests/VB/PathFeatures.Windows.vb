' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.Tests.VB
    Partial Public Module PathFeatures
        <DllImport("ntdll", ExactSpelling:=True)>
        Private Function RtlAreLongPathsEnabled() As Boolean
        End Function
    End Module
End Namespace
