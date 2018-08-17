' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Imports System.Runtime.InteropServices

Namespace System.Security.Permissions

    ' Summary:
    '     Specifies the type of file access requested.
    <Serializable>
    <Flags>
    <ComVisible(True)>
    Public Enum FileIOPermissionAccess
        '
        ' Summary:
        '     No access to a file or directory. System.Security.Permissions.FileIOPermissionAccess.NoAccess
        '     represents no valid System.Security.Permissions.FileIOPermissionAccess values
        '     and causes an System.ArgumentException when used as the parameter for System.Security.Permissions.FileIOPermission.GetPathList(System.Security.Permissions.FileIOPermissionAccess),
        '     which expects a single value.
        NoAccess = 0
        '
        ' Summary:
        '     Access to read from a file or directory.
        Read = 1
        '
        ' Summary:
        '     Access to write to or delete a file or directory. System.Security.Permissions.FileIOPermissionAccess.Write
        '     access includes deleting and overwriting files or directories.
        Write = 2
        '
        ' Summary:
        '     Access to append material to a file or directory. System.Security.Permissions.FileIOPermissionAccess.Append
        '     access includes the ability to create a new file or directory.
        Append = 4
        '
        ' Summary:
        '     Access to the information in the path itself. This helps protect sensitive information
        '     in the path, such as user names, as well as information about the directory structure
        '     revealed in the path. This value does not grant access to files or folders represented
        '     by the path.
        PathDiscovery = 8
        '
        ' Summary:
        '     System.Security.Permissions.FileIOPermissionAccess.Append, System.Security.Permissions.FileIOPermissionAccess.Read,
        '     System.Security.Permissions.FileIOPermissionAccess.Write, and System.Security.Permissions.FileIOPermissionAccess.PathDiscovery
        '     access to a file or directory. System.Security.Permissions.FileIOPermissionAccess.AllAccess
        '     represents multiple System.Security.Permissions.FileIOPermissionAccess values
        '     and causes an System.ArgumentException when used as the access parameter for
        '     the System.Security.Permissions.FileIOPermission.GetPathList(System.Security.Permissions.FileIOPermissionAccess)
        '     method, which expects a single value.
        AllAccess = 15
    End Enum

End Namespace
