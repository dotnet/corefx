' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text
Imports Microsoft.VisualBasic.CompilerServices
Namespace Microsoft.VisualBasic.FileIO
    Partial Public Class FileSystem
        ''' <summary>
        ''' Sets relevant flags on the SHFILEOPSTRUCT and calls SHFileOperation to copy move file / directory.
        ''' </summary>
        ''' <param name="Operation">Copy or move.</param>
        ''' <param name="TargetType">The target is a file or directory?</param>
        ''' <param name="FullSourcePath">Full path to source directory / file.</param>
        ''' <param name="FullTargetPath">Full path to target directory / file.</param>
        ''' <param name="ShowUI">Show all dialogs or just the error dialogs.</param>
        ''' <param name="OnUserCancel">Throw exception or ignore if user cancels the operation.</param>
        ''' <remarks>
        ''' Copy/MoveFile will call this directly. Copy/MoveDirectory will call ShellCopyOrMoveDirectory first
        ''' to change the path if needed.
        ''' </remarks>
        Private Shared Sub ShellCopyOrMove(ByVal Operation As CopyOrMove, ByVal TargetType As FileOrDirectory,
            ByVal FullSourcePath As String, ByVal FullTargetPath As String, ByVal ShowUI As UIOptionInternal, ByVal OnUserCancel As UICancelOption)
            Debug.Assert(System.Enum.IsDefined(GetType(CopyOrMove), Operation))
            Debug.Assert(System.Enum.IsDefined(GetType(FileOrDirectory), TargetType))
            Debug.Assert(FullSourcePath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullSourcePath), "Invalid FullSourcePath!!!")
            Debug.Assert(FullTargetPath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullTargetPath), "Invalid FullTargetPath!!!")
            Debug.Assert(ShowUI <> UIOptionInternal.NoUI, "Why call ShellDelete if ShowUI is NoUI???")

            ' Set operation type.
            Dim OperationType As SHFileOperationType
            If Operation = CopyOrMove.Copy Then
                OperationType = SHFileOperationType.FO_COPY
            Else
                OperationType = SHFileOperationType.FO_MOVE
            End If

            ' Set operation details.
            Dim OperationFlags As ShFileOperationFlags = GetOperationFlags(ShowUI)

            ' *** Special action for Directory only. ***
            Dim FinalSourcePath As String = FullSourcePath
            If TargetType = FileOrDirectory.Directory Then
                ' Shell behavior: If target does not exist, create target and copy / move source CONTENT into target.
                '                 If target exists, copy / move source into target.
                ' To have our behavior:
                '   If target does not exist, create target parent (or shell will throw) and call ShellCopyOrMove.
                '   If target exists, attach "\*" to FullSourcePath and call ShellCopyOrMove.
                ' In case of Move, since moving the directory, just create the target parent.
                If IO.Directory.Exists(FullTargetPath) Then
                    FinalSourcePath = IO.Path.Combine(FullSourcePath, "*")
                Else
                    IO.Directory.CreateDirectory(GetParentPath(FullTargetPath))
                End If
            End If

            'Call into ShellFileOperation.
            ShellFileOperation(OperationType, OperationFlags, FinalSourcePath, FullTargetPath, OnUserCancel, TargetType)

            ' *** Special action for Directory only. ***
            ' In case target does exist, and it's a move, we actually move content and leave the source directory.
            ' Clean up here.
            If Operation = CopyOrMove.Move AndAlso TargetType = FileOrDirectory.Directory Then
                If IO.Directory.Exists(FullSourcePath) Then
                    If IO.Directory.GetDirectories(FullSourcePath).Length = 0 _
                        AndAlso IO.Directory.GetFiles(FullSourcePath).Length = 0 Then
                        IO.Directory.Delete(FullSourcePath, recursive:=False)
                    End If
                End If
            End If

        End Sub

        ''' <summary>
        ''' Sets relevant flags on the SHFILEOPSTRUCT and calls into SHFileOperation to delete file / directory.
        ''' </summary>
        ''' <param name="FullPath">Full path to the file / directory.</param>
        ''' <param name="ShowUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="OnUserCancel">Throw exception or not if the operation was canceled (by user or errors in the system).</param>
        ''' <remarks>
        ''' We don't need to consider Recursive flag here since we already verify that in DeleteDirectory.
        ''' </remarks>
        Private Shared Sub ShellDelete(ByVal FullPath As String,
            ByVal ShowUI As UIOptionInternal, ByVal recycle As RecycleOption, ByVal OnUserCancel As UICancelOption, ByVal FileOrDirectory As FileOrDirectory)

            Debug.Assert(FullPath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullPath), "FullPath must be a full path!!!")
            Debug.Assert(ShowUI <> UIOptionInternal.NoUI, "Why call ShellDelete if ShowUI is NoUI???")

            ' Set fFlags to control the operation details.
            Dim OperationFlags As ShFileOperationFlags = GetOperationFlags(ShowUI)
            If (recycle = RecycleOption.SendToRecycleBin) Then
                OperationFlags = OperationFlags Or ShFileOperationFlags.FOF_ALLOWUNDO
            End If

            ShellFileOperation(SHFileOperationType.FO_DELETE, OperationFlags, FullPath, Nothing, OnUserCancel, FileOrDirectory)
        End Sub

        ''' <summary>
        ''' Calls NativeMethods.SHFileOperation with the given SHFILEOPSTRUCT, notifies the shell of change,
        ''' and throw exceptions if needed.
        ''' </summary>
        ''' <param name="OperationType">Value from SHFileOperationType, specifying Copy / Move / Delete</param>
        ''' <param name="OperationFlags">Value from ShFileOperationFlags, specifying overwrite, recycle bin, etc...</param>
        ''' <param name="FullSource">The full path to the source.</param>
        ''' <param name="FullTarget">The full path to the target. Nothing if this is a Delete operation.</param>
        ''' <param name="OnUserCancel">Value from UICancelOption, specifying to throw or not when user cancels the operation.</param>
        Private Shared Sub ShellFileOperation(ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
            ByVal FullSource As String, ByVal FullTarget As String, ByVal OnUserCancel As UICancelOption, ByVal FileOrDirectory As FileOrDirectory)

            Debug.Assert(System.Enum.IsDefined(GetType(SHFileOperationType), OperationType))
            Debug.Assert(OperationType <> SHFileOperationType.FO_RENAME, "Don't call Shell to rename!!!")
            Debug.Assert(FullSource.Length <> 0 AndAlso IO.Path.IsPathRooted(FullSource), "Invalid FullSource path!!!")
            Debug.Assert(OperationType = SHFileOperationType.FO_DELETE OrElse (FullTarget.Length <> 0 AndAlso IO.Path.IsPathRooted(FullTarget)), "Invalid FullTarget path!!!")


            ' Get the SHFILEOPSTRUCT
            Dim OperationInfo As SHFILEOPSTRUCT = GetShellOperationInfo(OperationType, OperationFlags, FullSource, FullTarget)

            Dim Result As Integer
            Try
                Result = NativeMethods.SHFileOperation(OperationInfo)
                ' Notify the shell in case some changes happened.
                'NativeMethods.SHChangeNotify(SHChangeEventTypes.SHCNE_DISKEVENTS, SHChangeEventParameterFlags.SHCNF_DWORD, IntPtr.Zero, IntPtr.Zero)
            Catch
                Throw
            Finally
            End Try

            ' If the operation was canceled, check OnUserCancel and throw OperationCanceledException if needed.
            ' Otherwise, check the result and throw the appropriate exception if there is an error code.
            If OperationInfo.fAnyOperationsAborted Then
                If OnUserCancel = UICancelOption.ThrowException Then
                    Throw New OperationCanceledException()
                End If
            ElseIf Result <> 0 Then
                ThrowWinIOError(Result)
            End If
        End Sub
        ''' <summary>
        ''' Returns an SHFILEOPSTRUCT used by SHFileOperation based on the given parameters.
        ''' </summary>
        ''' <param name="OperationType">One of the SHFileOperationType value: copy, move or delete.</param>
        ''' <param name="OperationFlags">Combination SHFileOperationFlags values: details of the operation.</param>
        ''' <param name="SourcePath">The source file / directory path.</param>
        ''' <param name="TargetPath">The target file / directory path. Nothing in case of delete.</param>
        ''' <returns>A fully initialized SHFILEOPSTRUCT.</returns>
        Private Shared Function GetShellOperationInfo(
                            ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
                            ByVal SourcePath As String, Optional ByVal TargetPath As String = Nothing) As SHFILEOPSTRUCT
            Debug.Assert(SourcePath.Length <> 0 AndAlso IO.Path.IsPathRooted(SourcePath), "Invalid SourcePath!!!")

            Return GetShellOperationInfo(OperationType, OperationFlags, New String() {SourcePath}, TargetPath)
        End Function

        ''' <summary>
        ''' Returns an SHFILEOPSTRUCT used by SHFileOperation based on the given parameters.
        ''' </summary>
        ''' <param name="OperationType">One of the SHFileOperationType value: copy, move or delete.</param>
        ''' <param name="OperationFlags">Combination SHFileOperationFlags values: details of the operation.</param>
        ''' <param name="SourcePaths">A string array containing the paths of source files. Must not be empty.</param>
        ''' <param name="TargetPath">The target file / directory path. Nothing in case of delete.</param>
        ''' <returns>A fully initialized SHFILEOPSTRUCT.</returns>
        Private Shared Function GetShellOperationInfo(
                            ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
                            ByVal SourcePaths() As String, Optional ByVal TargetPath As String = Nothing) As SHFILEOPSTRUCT
            Debug.Assert(System.Enum.IsDefined(GetType(SHFileOperationType), OperationType), "Invalid OperationType!!!")
            Debug.Assert(String.IsNullOrEmpty(TargetPath) OrElse IO.Path.IsPathRooted(TargetPath), "Invalid TargetPath!!!")
            Debug.Assert(SourcePaths IsNot Nothing AndAlso SourcePaths.Length > 0, "Invalid SourcePaths!!!")

            Dim OperationInfo As SHFILEOPSTRUCT

            ' Set wFunc - the operation.
            OperationInfo.wFunc = CType(OperationType, UInteger)

            ' Set fFlags - the operation details.
            OperationInfo.fFlags = CType(OperationFlags, UShort)

            ' Set pFrom and pTo - the paths.
            OperationInfo.pFrom = GetShellPath(SourcePaths)
            If TargetPath Is Nothing Then
                OperationInfo.pTo = Nothing
            Else
                OperationInfo.pTo = GetShellPath(TargetPath)
            End If

            ' Set other fields.
            OperationInfo.hNameMappings = IntPtr.Zero
            ' Try to set hwnd to the process's MainWindowHandle. If exception occurs, use IntPtr.Zero, which is desktop.
            Try
                OperationInfo.hwnd = Process.GetCurrentProcess.MainWindowHandle
            Catch ex As Exception
                If TypeOf (ex) Is SecurityException OrElse
                    TypeOf (ex) Is InvalidOperationException OrElse
                    TypeOf (ex) Is NotSupportedException Then
                    ' GetCurrentProcess can throw SecurityException. MainWindowHandle can throw InvalidOperationException or NotSupportedException.
                    OperationInfo.hwnd = IntPtr.Zero
                Else
                    Throw
                End If
            End Try
            OperationInfo.lpszProgressTitle = String.Empty ' We don't set this since we don't have any FOF_SIMPLEPROGRESS.

            Return OperationInfo
        End Function

        ''' <summary>
        ''' Return the ShFileOperationFlags based on the ShowUI option.
        ''' </summary>
        ''' <param name="ShowUI">UIOptionInternal value.</param>
        Private Shared Function GetOperationFlags(ByVal ShowUI As UIOptionInternal) As ShFileOperationFlags
            Dim OperationFlags As ShFileOperationFlags = m_SHELL_OPERATION_FLAGS_BASE
            If (ShowUI = UIOptionInternal.OnlyErrorDialogs) Then
                OperationFlags = OperationFlags Or m_SHELL_OPERATION_FLAGS_HIDE_UI
            End If
            Return OperationFlags
        End Function

        ''' <summary>
        ''' Returns the special path format required for pFrom and pTo of SHFILEOPSTRUCT. See NativeMethod.
        ''' </summary>
        ''' <param name="FullPath">The full path to be converted.</param>
        ''' <returns>A string in the required format.</returns>
        Private Shared Function GetShellPath(ByVal FullPath As String) As String
            Debug.Assert(FullPath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullPath), "Must be full path!!!")

            Return GetShellPath(New String() {FullPath})
        End Function

        ''' <summary>
        ''' Returns the special path format required for pFrom and pTo of SHFILEOPSTRUCT. See NativeMethod.
        ''' </summary>
        ''' <param name="FullPaths">A string array containing the paths for the operation.</param>
        ''' <returns>A string in the required format.</returns>
        Private Shared Function GetShellPath(ByVal FullPaths() As String) As String
            ' #If Debug Then
            Debug.Assert(FullPaths IsNot Nothing, "FullPaths is NULL!!!")
            Debug.Assert(FullPaths.Length > 0, "FullPaths() is empty array!!!")
            For Each FullPath As String In FullPaths
                Debug.Assert(FullPath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullPath), FullPath)
            Next
            ' #End If
            ' Each path will end with a Null character.
            Dim MultiString As New StringBuilder()
            For Each FullPath As String In FullPaths
                MultiString.Append(FullPath & ControlChars.NullChar)
            Next
            ' Don't need to append another Null character since String always end with Null character by default.
            Debug.Assert(MultiString.ToString.EndsWith(ControlChars.NullChar, StringComparison.Ordinal))

            Return MultiString.ToString()
        End Function


        ''' <summary>
        ''' Given an error code from winerror.h, throw the appropriate exception.
        ''' </summary>
        ''' <param name="errorCode">An error code from winerror.h.</param>
        ''' <remarks>
        ''' - This method is based on sources\ndp\clr\src\BCL\System\IO\_Error.cs::WinIOError, except the following.
        ''' - Exception message does not contain the path since at this point it is normalized.
        ''' - Instead of using PInvoke of GetMessage and MakeHRFromErrorCode, use managed code.
        ''' </remarks>
        Private Shared Sub ThrowWinIOError(ByVal errorCode As Integer)
            Select Case errorCode
                Case NativeTypes.ERROR_FILE_NOT_FOUND
                    Throw New IO.FileNotFoundException()
                Case NativeTypes.ERROR_PATH_NOT_FOUND
                    Throw New IO.DirectoryNotFoundException()
                Case NativeTypes.ERROR_ACCESS_DENIED
                    Throw New UnauthorizedAccessException()
                Case NativeTypes.ERROR_FILENAME_EXCED_RANGE
                    Throw New IO.PathTooLongException()
                Case NativeTypes.ERROR_INVALID_DRIVE
                    Throw New IO.DriveNotFoundException()
                Case NativeTypes.ERROR_OPERATION_ABORTED, NativeTypes.ERROR_CANCELLED
                    Throw New OperationCanceledException()
                Case Else
                    ' Including these from _Error.cs::WinIOError.
                    'Case NativeTypes.ERROR_ALREADY_EXISTS
                    'Case NativeTypes.ERROR_INVALID_PARAMETER
                    'Case NativeTypes.ERROR_SHARING_VIOLATION
                    'Case NativeTypes.ERROR_FILE_EXISTS
                    Throw New IO.IOException((New Win32Exception(errorCode)).Message,
                        Runtime.InteropServices.Marshal.GetHRForLastWin32Error())
            End Select
        End Sub

        ''' <summary>
        ''' Contains information that the SHFileOperation function uses to perform file operations on 32-bit platforms.
        ''' </summary>
        ''' <remarks>
        ''' * For detail documentation: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/shellcc/platform/shell/reference/structures/shfileopstruct.asp.
        ''' Members:
        '''   hwnd: Window handle to the dialog box to display information about the status of the operation.
        '''   wFunc: Value indicates which operation (copy, move, rename, delete) to perform.
        '''   pFrom: Buffer for 1 or more source file names. Each name ends with a NULL separator + additional NULL at the end.
        '''   pTo: Buffer for destination name(s). Same rule as pFrom.
        '''   fFlags: Flags that control details of the operation.
        '''   fAnyOperationsAborted: Out param. TRUE if user aborted any file operations. Otherwise, FALSE.
        '''   hNameMappings: Handle to name mapping object containing old and new names of renamed files (not used).
        '''   lpszProgressTitle: Address of a string to use as title of progress dialog box. (not used).
        ''' typedef struct _SHFILEOPSTRUCT {
        '''    HWND hwnd;
        '''    UINT wFunc;
        '''    LPCTSTR pFrom;
        '''    LPCTSTR pTo;
        '''    FILEOP_FLAGS fFlags; (WORD)
        '''    BOOL fAnyOperationsAborted;
        '''    LPVOID hNameMappings;
        '''    LPCTSTR lpszProgressTitle;
        ''' } SHFILEOPSTRUCT, *LPSHFILEOPSTRUCT;
        '''   If no steps are taken, the last 3 variables will not be passed correctly. Hence the Pack:=1.
        ''' </remarks>
        <StructLayout(LayoutKind.Sequential, Pack:=1, CharSet:=CharSet.Auto)>
        Friend Structure SHFILEOPSTRUCT
            Friend hwnd As IntPtr
            Friend wFunc As UInteger
            <MarshalAs(UnmanagedType.LPTStr)> Friend pFrom As String
            <MarshalAs(UnmanagedType.LPTStr)> Friend pTo As String
            Friend fFlags As UShort
            Friend fAnyOperationsAborted As Boolean
            Friend hNameMappings As IntPtr
            <MarshalAs(UnmanagedType.LPTStr)> Friend lpszProgressTitle As String
        End Structure

        ' Base operation flags used in shell IO operation.
        ' - DON'T move connected files as a group.
        ' - DON'T confirm directory creation - our silent copy / move do not.
        Private Const m_SHELL_OPERATION_FLAGS_BASE As ShFileOperationFlags =
            ShFileOperationFlags.FOF_NO_CONNECTED_ELEMENTS Or
            ShFileOperationFlags.FOF_NOCONFIRMMKDIR

        ' Hide UI operation flags for Delete.
        ' - DON'T show progress bar.
        ' - DON'T confirm (answer yes to everything). NOTE: In exception cases (read-only file), shell still asks.
        Private Const m_SHELL_OPERATION_FLAGS_HIDE_UI As ShFileOperationFlags =
            ShFileOperationFlags.FOF_SILENT Or
            ShFileOperationFlags.FOF_NOCONFIRMATION

        ' When calling MoveFileEx, set the following flags:
        ' - Simulate CopyFile and DeleteFile if copied to a different volume.
        ' - Replace contents of existing target with the contents of source file.
        ' - Do not return until the file has actually been moved on the disk.
        Private Const m_MOVEFILEEX_FLAGS As Integer = CInt(
            MoveFileExFlags.MOVEFILE_COPY_ALLOWED Or
            MoveFileExFlags.MOVEFILE_REPLACE_EXISTING Or
            MoveFileExFlags.MOVEFILE_WRITE_THROUGH)

        Private NotInheritable Class NativeTypes
            ' Error code from public\sdk\inc\winerror.h
            Friend Const ERROR_FILE_NOT_FOUND As Integer = 2
            Friend Const ERROR_PATH_NOT_FOUND As Integer = 3
            Friend Const ERROR_ACCESS_DENIED As Integer = 5
            Friend Const ERROR_ALREADY_EXISTS As Integer = 183
            Friend Const ERROR_FILENAME_EXCED_RANGE As Integer = 206
            Friend Const ERROR_INVALID_DRIVE As Integer = 15
            Friend Const ERROR_INVALID_PARAMETER As Integer = 87
            Friend Const ERROR_SHARING_VIOLATION As Integer = 32
            Friend Const ERROR_FILE_EXISTS As Integer = 80
            Friend Const ERROR_OPERATION_ABORTED As Integer = 995
            Friend Const ERROR_CANCELLED As Integer = 1223
        End Class
    End Class
End Namespace
