' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Security.Permissions

Namespace Microsoft.VisualBasic.FileIO
    Public Class FileIOPermission
        Private ReadOnly access As FileIOPermissionAccess
        Private ReadOnly fullDirectoryPath As String

        Public Sub New(access As FileIOPermissionAccess, fullDirectoryPath As String)
            Me.access = access
            Me.fullDirectoryPath = fullDirectoryPath
        End Sub
        Public Sub Demand()
        End Sub
    End Class
End Namespace
