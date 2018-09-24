﻿Imports System
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.FileIO.FileSystem

#Const PLATFORM_WINDOWS = True
Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Partial Friend NotInheritable Class NativeMethods
        ''' <summary>
        ''' Copies, moves, renames or deletes a file system object on 32-bit platforms.
        ''' </summary>
        ''' <param name="lpFileOp">Pointer to an SHFILEOPSTRUCT structure that contains information this function needs
        '''       to carry out the specified operation. This parameter must contain a valid value that is not NULL.
        '''       You are responsible for validating the value. If you do not, you will experience unexpected result.</param>
        ''' <returns>Returns zero if successful, non zero otherwise.</returns>
        ''' <remarks>
        ''' You should use fully-qualified path names with this function. Using it with relative path names is not thread safe.
        ''' You cannot use SHFileOperation to move special folders My Documents and My Pictures from a local drive to a remote computer.
        ''' File deletion is recursive unless you set the FOF_NORECURSION flag.
        ''' </remarks>
        <DllImport("shell32.dll", CharSet:=CharSet.Auto, EntryPoint:="SHFileOperation", SetLastError:=True, ThrowOnUnmappableChar:=True)>
        Private Shared Function SHFileOperation32(ByRef lpFileOp As SHFILEOPSTRUCT) As Integer
        End Function

        ''' <summary>
        ''' Copies, moves, renames or deletes a file system object on 64-bit platforms.
        ''' </summary>
        <DllImport("shell32.dll", CharSet:=CharSet.Auto, EntryPoint:="SHFileOperation", SetLastError:=True, ThrowOnUnmappableChar:=True)>
        Private Shared Function SHFileOperation64(ByRef lpFileOp As SHFILEOPSTRUCT64) As Integer
        End Function

        ''' <summary>
        ''' Given a 32-bit SHFILEOPSTRUCT, call the appropriate SHFileOperation function
        ''' to perform shell file operation.
        ''' </summary>
        ''' <param name="lpFileOp">32-bit SHFILEOPSTRUCT</param>
        ''' <returns>0 if successful, non-zero otherwise.</returns>
        Friend Shared Function SHFileOperation(ByRef lpFileOp As SHFILEOPSTRUCT) As Integer
            If (IntPtr.Size = 4) Then ' 32-bit platforms
                Return SHFileOperation32(lpFileOp)
            Else ' 64-bit plaforms
                ' Create a new SHFILEOPSTRUCT64. The only difference is the packing, so copy all fields.
                Dim lpFileOp64 As New SHFILEOPSTRUCT64 With {
                    .hwnd = lpFileOp.hwnd,
                    .wFunc = lpFileOp.wFunc,
                    .pFrom = lpFileOp.pFrom,
                    .pTo = lpFileOp.pTo,
                    .fFlags = lpFileOp.fFlags,
                    .fAnyOperationsAborted = lpFileOp.fAnyOperationsAborted,
                    .hNameMappings = lpFileOp.hNameMappings,
                    .lpszProgressTitle = lpFileOp.lpszProgressTitle
                }

                ' P/Invoke SHFileOperation with the 64 bit structure.
                Dim result As Integer = SHFileOperation64(lpFileOp64)

                ' Only need to check if any operations were aborted.
                lpFileOp.fAnyOperationsAborted = lpFileOp64.fAnyOperationsAborted

                Return result
            End If
        End Function

        ''' <summary>
        ''' Contains information that the SHFileOperation function uses to perform file operations
        ''' on 64-bit platforms, where the structure is unpacked. VSWhidbey 421265, 
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
        Private Structure SHFILEOPSTRUCT64
            Friend hwnd As IntPtr
            Friend wFunc As UInteger
            <MarshalAs(UnmanagedType.LPTStr)> Friend pFrom As String
            <MarshalAs(UnmanagedType.LPTStr)> Friend pTo As String
            Friend fFlags As UInteger
            Friend fAnyOperationsAborted As Boolean
            Friend hNameMappings As IntPtr
            <MarshalAs(UnmanagedType.LPTStr)> Friend lpszProgressTitle As String
        End Structure
    End Class
End Namespace
