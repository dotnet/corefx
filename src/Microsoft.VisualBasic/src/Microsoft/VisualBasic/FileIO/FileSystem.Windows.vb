Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports Microsoft.VisualBasic.CompilerServices
#Const PLATFORM_WINDOWS = True
Namespace Microsoft.VisualBasic.FileIO
    Partial Public Class FileSystem
#If PLATFORM_WINDOWS Then
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

        '''**************************************************************************
        ''' ShellDelete
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

        '''**************************************************************************
        ''' ShellFileOperation
        ''' <summary>
        ''' Calls NativeMethods.SHFileOperation with the given SHFILEOPSTRUCT, notifies the shell of change,
        ''' and throw exceptions if needed.
        ''' </summary>
        ''' <param name="OperationType">Value from SHFileOperationType, specifying Copy / Move / Delete</param>
        ''' <param name="OperationFlags">Value from ShFileOperationFlags, specifying overwrite, recycle bin, etc...</param>
        ''' <param name="FullSource">The full path to the source.</param>
        ''' <param name="FullTarget">The full path to the target. Nothing if this is a Delete operation.</param>
        ''' <param name="OnUserCancel">Value from UICancelOption, specifying to throw or not when user cancels the operation.</param>
        ''' <remarks>
        ''' !!!!! SECURITY WARNING !!!!
        ''' Demand appropriate FileIOPermission on FullSource and FullTarget before calling into this method.
        ''' </remarks>
        Private Shared Sub ShellFileOperation(ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
            ByVal FullSource As String, ByVal FullTarget As String, ByVal OnUserCancel As UICancelOption, ByVal FileOrDirectory As FileOrDirectory)

            ' Apply HostProtectionAttribute(UI = true) to indicate this function belongs to UI type.
            ' http://devdiv/SpecTool/Documents/Whidbey/CLR/CurrentSpecs/SQLHost/hPA%20Guidance.doc

            Debug.Assert(System.Enum.IsDefined(GetType(SHFileOperationType), OperationType))
            Debug.Assert(OperationType <> SHFileOperationType.FO_RENAME, "Don't call Shell to rename!!!")
            Debug.Assert(FullSource.Length <> 0 AndAlso IO.Path.IsPathRooted(FullSource), "Invalid FullSource path!!!")
            Debug.Assert(OperationType = SHFileOperationType.FO_DELETE OrElse (FullTarget.Length <> 0 AndAlso IO.Path.IsPathRooted(FullTarget)), "Invalid FullTarget path!!!")

            ' FullSource might end with '\*' (for copying and moving) so normalize the path to the correct format to demand the permission.
            Dim CheckPermissionPath As String = FullSource
            If (OperationType = SHFileOperationType.FO_COPY OrElse OperationType = SHFileOperationType.FO_MOVE) _
                    AndAlso CheckPermissionPath.EndsWith("*", StringComparison.Ordinal) Then
                CheckPermissionPath = RemoveEndingSeparator(FullSource.TrimEnd("*"c))
            End If

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
                        System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error())
            End Select
        End Sub
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
#End If
    End Class
End Namespace
