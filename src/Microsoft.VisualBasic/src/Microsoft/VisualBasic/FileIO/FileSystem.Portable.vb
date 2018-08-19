' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.Utils
Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Globalization
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Resources.ResourceManager
Imports System.Runtime
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Threading

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

'''' IMPORTANT: Changes made to public interface of FileSystem should be reflected in FileSystemProxy.vb.

Namespace Microsoft.VisualBasic.FileIO

    '''******************************************************************************
    ''' ;FileSystem
    ''' <summary>
    '''  This class represents the file system on a computer. It allows browsing the existing drives, special directories;
    '''  and also contains some commonly use methods for IO tasks.
    ''' </summary>
    '<HostProtection(Resources:=HostProtectionResource.ExternalProcessMgmt)>
    Partial Public Class FileSystem
        ' When calling MoveFileEx, set the following flags:
        ' - Simulate CopyFile and DeleteFile if copied to a different volume.
        ' - Replace contents of existing target with the contents of source file.
        ' - Do not return until the file has actually been moved on the disk.
        Private Const m_MOVEFILEEX_FLAGS As Integer = CInt(
            MoveFileExFlags.MOVEFILE_COPY_ALLOWED Or
            MoveFileExFlags.MOVEFILE_REPLACE_EXISTING Or
            MoveFileExFlags.MOVEFILE_WRITE_THROUGH)

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

        ' Array containing all the path separator chars. Used to verify that input is a name, not a path.
        Private ReadOnly m_SeparatorChars() As Char = {
            IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar, IO.Path.VolumeSeparatorChar}

        '''**************************************************************************
        ''' ;CopyOrMove
        ''' <summary>
        ''' Private enumeration: The operation is a Copy or Move.
        ''' </summary>
        Private Enum CopyOrMove
            Copy
            Move
        End Enum

        '''**************************************************************************
        ''' ;FileOrDirectory
        ''' <summary>
        ''' Private enumeration: Target of the operation is a File or Directory.
        ''' </summary>
        ''' <remarks></remarks>
        Private Enum FileOrDirectory
            File
            Directory
        End Enum

        '''**************************************************************************
        ''' ;SHFILEOPSTRUCT
        ''' <summary>
        ''' Contains information that the SHFileOperation function uses to perform file operations
        ''' on 32-bit platforms.
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
        '''    FILEOP_FLAGS fFlags;                           (WORD)
        '''    BOOL fAnyOperationsAborted;
        '''    LPVOID hNameMappings;
        '''    LPCTSTR lpszProgressTitle;
        ''' } SHFILEOPSTRUCT, *LPSHFILEOPSTRUCT;
        ''' * VSWhidbey 321345, 321346: From KB151799 - The SHFILEOPSTRUCT is not double-word aligned.
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

        '''**************************************************************************
        ''' ;MoveFileExFlags
        ''' <summary>
        ''' Flags for MoveFileEx.
        ''' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/movefileex.asp
        ''' and public\sdk\inc\winbase.h.
        ''' </summary>
        <Flags>
        Friend Enum MoveFileExFlags As Integer
            MOVEFILE_REPLACE_EXISTING = &H1
            MOVEFILE_COPY_ALLOWED = &H2
            MOVEFILE_DELAY_UNTIL_REBOOT = &H4
            MOVEFILE_WRITE_THROUGH = &H8
        End Enum


        '''**************************************************************************
        ''' ;UIOptionInternal
        ''' <summary>
        ''' Private enumeration: Indicate the options of ShowUI to use internally.
        ''' This includes NoUI so that we can base the decision on 1 variable.
        ''' </summary>
        ''' <remarks></remarks>
        Private Enum UIOptionInternal
            OnlyErrorDialogs = UIOption.OnlyErrorDialogs
            AllDialogs = UIOption.AllDialogs
            NoUI
        End Enum

        '''**************************************************************************
        ''' ;ShFileOperationFlags
        ''' <summary>
        ''' Flags that control the file operation. Used in SHFILEOPSTRUCT.
        ''' </summary>
        <Flags>
        Friend Enum ShFileOperationFlags As UShort

            ' The pTo member specifies multiple destination files (one for each source file)
            ' rather than one directory where all source files are to be deposited.
            FOF_MULTIDESTFILES = &H1

            ' Not currently used.
            FOF_CONFIRMMOUSE = &H2

            ' Do not display a progress dialog box.
            FOF_SILENT = &H4

            ' Give the file being operated on a new name in a move, copy, or rename operation
            ' if a file with the target name already exists.
            FOF_RENAMEONCOLLISION = &H8

            ' Respond with "Yes to All" for any dialog box that is displayed.
            FOF_NOCONFIRMATION = &H10

            ' If FOF_RENAMEONCOLLISION is specified and any files were renamed,
            ' assign a name mapping object containing their old and new names to the hNameMappings member.
            FOF_WANTMAPPINGHANDLE = &H20

            ' Preserve Undo information, if possible. Undone can only be done from the same process.
            ' If pFrom does not contain fully qualified path and file names, this flag is ignored.
            ' NOTE: Not setting this flag will let the file be deleted permanently, unlike the doc says.
            FOF_ALLOWUNDO = &H40

            ' Perform the operation on files only if a wildcard file name (*.*) is specified.
            FOF_FILESONLY = &H80

            ' Display a progress dialog box but do not show the file names.
            FOF_SIMPLEPROGRESS = &H100

            ' Do not confirm the creation of a new directory if the operation requires one to be created.
            FOF_NOCONFIRMMKDIR = &H200

            ' Do not display a user interface if an error occurs.
            FOF_NOERRORUI = &H400

            ' Do not copy the security attributes of the file.
            FOF_NOCOPYSECURITYATTRIBS = &H800

            ' Only operate in the local directory. Don't operate recursively into subdirectories.
            FOF_NORECURSION = &H1000

            ' Do not move connected files as a group. Only move the specified files.
            FOF_NO_CONNECTED_ELEMENTS = &H2000

            ' Send a warning if a file is being destroyed during a delete operation rather than recycled.
            ' This flag partially overrides FOF_NOCONFIRMATION.
            FOF_WANTNUKEWARNING = &H4000

            ' Treat reparse points as objects, not containers.
            FOF_NORECURSEREPARSE = &H8000

        End Enum

        '''**************************************************************************
        ''' ;SHFileOperationType
        ''' <summary>
        ''' Values that indicate which file operation to perform. Used in SHFILEOPSTRUCT
        ''' </summary>
        Friend Enum SHFileOperationType As UInteger
            FO_MOVE = &H1
            FO_COPY = &H2
            FO_DELETE = &H3
            FO_RENAME = &H4
        End Enum

        '''**************************************************************************
        ''' ;CopyOrMoveDirectory
        ''' <summary>
        ''' Handles exception cases and calls shell or framework to copy / move directory.
        ''' </summary>
        ''' <param name="operation">select Copy or Move operation.</param>
        ''' <param name="sourceDirectoryName">the source directory</param>
        ''' <param name="destinationDirectoryName">the target directory</param>
        ''' <param name="overwrite">overwrite files</param>
        ''' <param name="showUI">calls into shell to copy / move directory</param>
        ''' <param name="onUserCancel">throw exception if user cancels the operation or not.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath exceptions: If SourceDirectoryPath or TargetDirectoryPath is invalid.
        '''     Or if NewName contains path information.</exception>
        ''' <exception cref="System.ArgumentException">If Source or Target is device path (\\.\).</exception>
        ''' <exception cref="IO.DirectoryNotFoundException">Source directory does not exist as a directory.</exception>
        ''' <exception cref="System.ArgumentNullException">If NewName.Length = 0.</exception>
        ''' <exception cref="IO.IOException">SourceDirectoryPath and TargetDirectoryPath are the same.
        '''     IOException: Target directory is under source directory - cyclic operation.
        '''     IOException: TargetDirectoryPath points to an existing file.
        '''     IOException: Some files and directories can not be copied.</exception>
        Private Sub CopyOrMoveDirectory(ByVal operation As CopyOrMove,
            ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String,
            ByVal overwrite As Boolean, ByVal showUI As UIOptionInternal, ByVal onUserCancel As UICancelOption)

            Debug.Assert(System.Enum.IsDefined(GetType(CopyOrMove), operation), "Invalid Operation!!!")

            ' Verify enums.
            VerifyUICancelOption("onUserCancel", onUserCancel)

            ' Get the full path and remove any separators at the end. This will handle invalid path exceptions.
            ' IMPORTANT: sourceDirectoryName and destinationDirectoryName should be used for exception throwing ONLY.
            Dim SourceDirectoryFullPath As String = NormalizePath(sourceDirectoryName)
            Dim TargetDirectoryFullPath As String = NormalizePath(destinationDirectoryName)

            ' Demand FileIOPermission on the given path. See CopyOrMoveFile for reason why we don't wait for Framework to demand.
            Dim sourceAccess As FileIOPermissionAccess = FileIOPermissionAccess.Read
            If operation = CopyOrMove.Move Then
                sourceAccess = sourceAccess Or FileIOPermissionAccess.Write
            End If
            DemandDirectoryPermission(SourceDirectoryFullPath, sourceAccess)
            ' Copy / Move Directory will create the target directory.
            ' Therefore we also demand Read permission on target path. This will also fix VSWhidbey 421158.
            DemandDirectoryPermission(TargetDirectoryFullPath, FileIOPermissionAccess.Read Or FileIOPermissionAccess.Write)

            ' Throw if device path.
            ThrowIfDevicePath(SourceDirectoryFullPath)
            ThrowIfDevicePath(TargetDirectoryFullPath)

            ' Throw if source directory does not exist.
            If Not IO.Directory.Exists(SourceDirectoryFullPath) Then
                Throw ExUtils.GetDirectoryNotFoundException(SR.IO_DirectoryNotFound_Path, sourceDirectoryName)
            End If

            ' Throw if source directory is a root directory.
            If IsRoot(SourceDirectoryFullPath) Then
                Throw ExUtils.GetIOException(SR.IO_DirectoryIsRoot_Path, sourceDirectoryName)
            End If

            ' Throw if there's a file at TargetDirectoryFullPath.
            If IO.File.Exists(TargetDirectoryFullPath) Then
                Throw ExUtils.GetIOException(SR.IO_FileExists_Path, destinationDirectoryName)
            End If

            ' Throw if source and target are the same.
            If TargetDirectoryFullPath.Equals(SourceDirectoryFullPath, StringComparison.OrdinalIgnoreCase) Then
                Throw ExUtils.GetIOException(SR.IO_SourceEqualsTargetDirectory)
            End If

            ' Throw if cyclic operation (target is under source). A sample case is
            '   Source = C:\Dir1\Dir2
            '   Target = C:\Dir1\Dir2\Dir3\Dir4.
            ' NOTE: Do not use StartWith since it does not allow specifying InvariantCultureIgnoreCase.
            If TargetDirectoryFullPath.Length > SourceDirectoryFullPath.Length AndAlso
                TargetDirectoryFullPath.Substring(0, SourceDirectoryFullPath.Length).Equals(
                SourceDirectoryFullPath, StringComparison.OrdinalIgnoreCase) Then
                Debug.Assert(TargetDirectoryFullPath.Length > SourceDirectoryFullPath.Length, "Target path should be longer!!!")

                ' Bug fix VSWhidbey 74249: don't throw in case Target = C:\Dir1\Dir2SomethingElse.
                If TargetDirectoryFullPath.Chars(SourceDirectoryFullPath.Length) = IO.Path.DirectorySeparatorChar Then
                    Throw ExUtils.GetInvalidOperationException(SR.IO_CyclicOperation)
                End If
            End If

            ' NOTE: Decision to create target directory is different for Shell and Framework call.

            If showUI <> UIOptionInternal.NoUI AndAlso Environment.UserInteractive Then
#If HaveUI Then
                ' If ShowUI AND UserInteractive (VSWhidbey 230265), attempt to call Shell function.
                ShellCopyOrMove(operation, FileOrDirectory.Directory, SourceDirectoryFullPath, TargetDirectoryFullPath, showUI, onUserCancel)
#Else
                Throw New PlatformNotSupportedException("No UI for ShellCopyOrMove(operation, FileOrDirectory.Directory, SourceDirectoryFullPath, TargetDirectoryFullPath, showUI, onUserCancel)")
#End If
            Else
                ' Otherwise, copy the directory using System.IO.
                FxCopyOrMoveDirectory(operation, SourceDirectoryFullPath, TargetDirectoryFullPath, overwrite)
            End If
        End Sub

        '''******************************************************************************
        ''' ;CopyOrMoveDirectoryNode
        ''' <summary>
        ''' Given a directory node, copy or move that directory tree.
        ''' </summary>
        ''' <param name="Operation">Specify whether to move or copy the directories.</param>
        ''' <param name="SourceDirectoryNode">The source node. Only copy / move directories contained in the source node.</param>
        ''' <param name="Overwrite">True to overwrite sub-files. Otherwise False.</param>
        ''' <param name="Exceptions">The list of accumulated exceptions while doing the copy / move</param>
        Private Sub CopyOrMoveDirectoryNode(ByVal Operation As CopyOrMove,
            ByVal SourceDirectoryNode As DirectoryNode, ByVal Overwrite As Boolean, ByVal Exceptions As ListDictionary)

            Debug.Assert(System.Enum.IsDefined(GetType(CopyOrMove), Operation), "Invalid Operation!!!")
            Debug.Assert(Exceptions IsNot Nothing, "Null exception list!!!")
            Debug.Assert(SourceDirectoryNode IsNot Nothing, "Null source node!!!")

            ' Create the target directory. If we encounter known exceptions, add the exception to the exception list and quit.
            Try
                If Not IO.Directory.Exists(SourceDirectoryNode.TargetPath) Then
                    IO.Directory.CreateDirectory(SourceDirectoryNode.TargetPath)
                End If
            Catch ex As Exception
                If (TypeOf ex Is IO.IOException OrElse
                    TypeOf ex Is UnauthorizedAccessException OrElse
                    TypeOf ex Is IO.DirectoryNotFoundException OrElse
                    TypeOf ex Is NotSupportedException OrElse
                    TypeOf ex Is SecurityException) Then
                    Exceptions.Add(SourceDirectoryNode.Path, ex.Message)
                    Exit Sub
                Else
                    Throw
                End If
            End Try
            Debug.Assert(IO.Directory.Exists(SourceDirectoryNode.TargetPath), "TargetPath should have existed or exception should be thrown!!!")
            If Not IO.Directory.Exists(SourceDirectoryNode.TargetPath) Then
                Exceptions.Add(SourceDirectoryNode.TargetPath, ExUtils.GetDirectoryNotFoundException(SR.IO_DirectoryNotFound_Path, SourceDirectoryNode.TargetPath))
                Exit Sub
            End If

            ' Copy / move all the files under this directory to target directory.
            For Each SubFilePath As String In IO.Directory.GetFiles(SourceDirectoryNode.Path)
                Try
                    CopyOrMoveFile(Operation, SubFilePath, IO.Path.Combine(SourceDirectoryNode.TargetPath, IO.Path.GetFileName(SubFilePath)),
                        Overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException)
                Catch ex As Exception
                    If (TypeOf ex Is IO.IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse
                        TypeOf ex Is SecurityException OrElse TypeOf ex Is NotSupportedException) Then
                        Exceptions.Add(SubFilePath, ex.Message)
                    Else
                        Throw
                    End If
                End Try
            Next

            ' Copy / move all the sub directories under this directory to target directory.
            For Each SubDirectoryNode As DirectoryNode In SourceDirectoryNode.SubDirs
                CopyOrMoveDirectoryNode(Operation, SubDirectoryNode, Overwrite, Exceptions)
            Next

            ' If this is a move, try to delete the current directory.
            ' Using recursive:=False since we expect the content should be emptied by now.
            If Operation = CopyOrMove.Move Then
                Try
                    IO.Directory.Delete(SourceDirectoryNode.Path, recursive:=False)
                Catch ex As Exception
                    If (TypeOf ex Is IO.IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse
                        TypeOf ex Is SecurityException OrElse TypeOf ex Is IO.DirectoryNotFoundException) Then
                        Exceptions.Add(SourceDirectoryNode.Path, ex.Message)
                    Else
                        Throw
                    End If
                End Try
            End If
        End Sub

        Private Sub CopyOrMoveFile(ByVal operation As CopyOrMove,
                    ByVal sourceFileName As String, ByVal destinationFileName As String,
                    ByVal overwrite As Boolean, ByVal showUI As UIOptionInternal, ByVal onUserCancel As UICancelOption
                )
            Debug.Assert(System.Enum.IsDefined(GetType(CopyOrMove), operation), "Invalid Operation!!!")

            ' Verify enums.
            VerifyUICancelOption("onUserCancel", onUserCancel)

            ' Get the full path and remove any separator at the end. This will handle invalid path exceptions.
            ' IMPORTANT: sourceFileName and destinationFileName should be used for throwing user exceptions ONLY.
            Dim sourceFileFullPath As String = NormalizeFilePath(sourceFileName, "sourceFileName")
            Dim destinationFileFullPath As String = NormalizeFilePath(destinationFileName, "destinationFileName")

            ' Demand FileIOPermission on the given paths. (CONSIDER: Performance?)
            ' We do it here instead of waiting for System.IO.File.Copy/Move to demand because
            ' - Framework's code demand FileIOPermission firstly.
            ' - We add the ability to move with overwrite flag, which leads to different behavior
            '   if we don't demand (VSWhidbey 421322).
            ' - Security reason.
            ' SECURITY NOTE: We assert UnmanagedCodePermission to call MoveFileEx later if needed so make sure
            '   we demand the appropriate FileIOPermission.
            Dim sourceAccess As FileIOPermissionAccess = FileIOPermissionAccess.Read
            If operation = CopyOrMove.Move Then
                sourceAccess = sourceAccess Or FileIOPermissionAccess.Write
            End If
            Call (New FileIOPermission(sourceAccess, sourceFileFullPath)).Demand()
            Call (New FileIOPermission(FileIOPermissionAccess.Write, destinationFileFullPath)).Demand()

            ' Throw if device path.
            ThrowIfDevicePath(sourceFileFullPath)
            ThrowIfDevicePath(destinationFileFullPath)

            ' Throw exception if SourceFilePath does not exist.
            If Not IO.File.Exists(sourceFileFullPath) Then
                Throw ExUtils.GetFileNotFoundException(sourceFileName, SR.IO_FileNotFound_Path, sourceFileName)
            End If

            ' Throw exception if TargetFilePath is an existing directory.
            If IO.Directory.Exists(destinationFileFullPath) Then
                Throw ExUtils.GetIOException(SR.IO_DirectoryExists_Path, destinationFileName)
            End If

            ' Always create the target's parent directory(s).
            IO.Directory.CreateDirectory(GetParentPath(destinationFileFullPath))

            ' If ShowUI, attempt to call Shell function.
            If showUI <> UIOptionInternal.NoUI AndAlso System.Environment.UserInteractive Then
#If HaveUI Then
                ShellCopyOrMove(operation, FileOrDirectory.File, sourceFileFullPath, destinationFileFullPath, showUI, onUserCancel)
#Else
                Throw New PlatformNotSupportedException("No UI for ShellCopyOrMove(operation, FileOrDirectory.File, sourceFileFullPath, destinationFileFullPath, showUI, onUserCancel)")
#End If
                Exit Sub
            End If

            ' Use Framework.
            If operation = CopyOrMove.Copy OrElse
                sourceFileFullPath.Equals(destinationFileFullPath, StringComparison.OrdinalIgnoreCase) Then
                ' Call IO.File.Copy if this is a copy operation.
                ' In addition, if sourceFileFullPath is the same as destinationFileFullPath,
                '   IO.File.Copy will throw, IO.File.Move will not (VSWhidbey334759).
                '   Whatever overwrite flag is passed in, IO.File.Move should throw exception,
                '   so call IO.File.Copy to get the exception as well.
                IO.File.Copy(sourceFileFullPath, destinationFileFullPath, overwrite)
            Else ' MoveFile with support for overwrite flag.
                If overwrite Then ' User wants to overwrite destination.
                    ' Why not checking for destination existence: user may not have read permission / ACL,
                    ' but have write permission / ACL thus cannot see but can delete / overwrite destination.

                    'If Environment.OSVersion.Platform = PlatformID.Win32NT Then ' Platforms supporting MoveFileEx.
                    '    ' SECURITY NOTE: We already demand permission at the start.
                    '    'Call (New SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Assert()
                    '    Try
                    '        IO.File.Move(sourceFileName:=sourceFileFullPath, destFileName:=destinationFileFullPath)
                    '        ' GetLastWin32Error has to be close to PInvoke call. FxCop rule.
                    '        If Not succeed Then
                    '            ThrowWinIOError(System.Runtime.InteropServices.Marshal.GetLastWin32Error())
                    '        End If
                    '    Catch
                    '        Throw
                    '    Finally
                    '    End Try
                    'Else ' Win95, Win98, WinME.
                    ' IO.File.Delete will not throw if destinationFileFullPath does not exist
                    ' (user may not have permission to discover this, but have permission to overwrite),
                    ' so always delete the destination.
                    IO.File.Delete(destinationFileFullPath)
                    IO.File.Move(sourceFileFullPath, destinationFileFullPath)
                    'End If
                Else ' Overwrite = False, call Framework.
                    IO.File.Move(sourceFileFullPath, destinationFileFullPath)
                End If ' Overwrite
            End If
        End Sub

        Private Sub DeleteDirectoryInternal(ByVal directory As String, ByVal onDirectoryNotEmpty As DeleteDirectoryOption,
                    ByVal showUI As UIOptionInternal, ByVal recycle As RecycleOption, ByVal onUserCancel As UICancelOption)

            ' Verify enums. VSWhidbey 522083.
            VerifyDeleteDirectoryOption("onDirectoryNotEmpty", onDirectoryNotEmpty)
            VerifyRecycleOption("recycle", recycle)
            VerifyUICancelOption("onUserCancel", onUserCancel)

            ' Get the full path. This will handle invalid paths exceptions.
            Dim directoryFullPath As String = IO.Path.GetFullPath(directory)

            ' Demand Write permission for security reason (see CopyOrMoveFile / CopyOrMoveDirectory).
            DemandDirectoryPermission(directoryFullPath, FileIOPermissionAccess.Write)

            ' Throw if device path.
            ThrowIfDevicePath(directoryFullPath)

            If Not IO.Directory.Exists(directoryFullPath) Then
                Throw ExUtils.GetDirectoryNotFoundException(SR.IO_DirectoryNotFound_Path, directory)
            End If

            ' VsWhidbey 319809: Throw exception if deleting root directory.
            If IsRoot(directoryFullPath) Then
                Throw ExUtils.GetIOException(SR.IO_DirectoryIsRoot_Path, directory)
            End If

            ' If user want shell features (Progress, Recycle Bin), call shell operation.
            ' We don't need to consider onDirectoryNotEmpty here (VSWhidbey 283409).
            If (showUI <> UIOptionInternal.NoUI) AndAlso Environment.UserInteractive Then
#If HaveUI Then
                ShellDelete(directoryFullPath, showUI, recycle, onUserCancel, FileOrDirectory.Directory)
#Else
                Throw New PlatformNotSupportedException("No UI for ShellDelete(directoryFullPath, showUI, recycle, onUserCancel, FileOrDirectory.Directory)")
#End If
                Exit Sub
            End If

            ' Otherwise, call Framework's method.
            IO.Directory.Delete(directoryFullPath, onDirectoryNotEmpty = DeleteDirectoryOption.DeleteAllContents)
        End Sub

        Private Sub DeleteFileInternal(ByVal file As String, ByVal showUI As UIOptionInternal, ByVal recycle As RecycleOption,
                    ByVal onUserCancel As UICancelOption)

            ' Verify enums
            VerifyRecycleOption("recycle", recycle)
            VerifyUICancelOption("onUserCancel", onUserCancel)

            ' Get the full path. This will handle invalid path exceptions.
            Dim fileFullPath As String = NormalizeFilePath(file, "file")

            ' Demand Write permission for security reason (see CopyOrMoveFile / CopyOrMoveDirectory).
            Call (New FileIOPermission(FileIOPermissionAccess.Write, fileFullPath)).Demand()

            ' Throw if device path.
            ThrowIfDevicePath(fileFullPath)

            If Not IO.File.Exists(fileFullPath) Then
                Throw ExUtils.GetFileNotFoundException(file, SR.IO_FileNotFound_Path, file)
            End If

            ' If user want shell features (Progress, Recycle Bin), call shell operation.
            If (showUI <> UIOptionInternal.NoUI) AndAlso Environment.UserInteractive Then
#If HaveUI Then
                ShellDelete(fileFullPath, showUI, recycle, onUserCancel, FileOrDirectory.File)
#Else
                Throw New PlatformNotSupportedException("No UI for ShellDelete(fileFullPath, showUI, recycle, onUserCancel, FileOrDirectory.File)")
#End If
                Exit Sub
            End If

            IO.File.Delete(fileFullPath)
        End Sub

        Private Sub DemandDirectoryPermission(ByVal fullDirectoryPath As String, ByVal access As FileIOPermissionAccess)
            Debug.Assert(NormalizePath(fullDirectoryPath).Equals(fullDirectoryPath, StringComparison.OrdinalIgnoreCase),
                "fullDirectoryPath must be normalized before calling this method.")

            ' Add a directory separator character to the end if needed to demand permission on the whole directory.
            If Not (fullDirectoryPath.EndsWith(IO.Path.DirectorySeparatorChar, StringComparison.Ordinal) Or
                fullDirectoryPath.EndsWith(IO.Path.AltDirectorySeparatorChar, StringComparison.Ordinal)) Then

                fullDirectoryPath &= IO.Path.DirectorySeparatorChar
            End If

            Dim fileIOPerm As New FileIOPermission(access, fullDirectoryPath)
            fileIOPerm.Demand()
        End Sub

        '''**************************************************************************
        ''' ;CopyOrMoveFile
        ''' <summary>
        ''' Copies or move files. This will be called from CopyFile and MoveFile.
        ''' </summary>
        ''' <param name="Operation">Copy or Move.</param>
        ''' <param name="sourceFileName">Path to source file.</param>
        ''' <param name="destinationFileName">Path to target file.</param>
        ''' <param name="Overwrite">True = Overwrite. This flag will be ignored if ShowUI.</param>
        ''' <param name="ShowUI">Hide or show the UIDialogs.</param>
        ''' <param name="OnUserCancel">Throw exception in case user cancel using UI or not.</param>
        ''' <exception cref="IO.Path.GetFullPath">
        '''   IO.Path.GetFullPath exceptions: If SourceFilePath or TargetFilePath is invalid.
        '''   ArgumentException: If Source or Target is device path (\\.\).
        '''   FileNotFoundException: If SourceFilePath does not exist (including pointing to an existing directory).
        '''   IOException: If TargetFilePath points to an existing directory.
        '''   ArgumenNullException: If NewName.
        '''   ArgumentException: If NewName contains path information.
        ''' </exception>
        '''**************************************************************************
        ''' ;DeleteDirectory
        ''' <summary>
        ''' Delete the given directory, with options to recursively delete, show progress UI, send file to Recycle Bin, and whether to throw exception if user cancels.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="onDirectoryNotEmpty">DeleteAllContents to delete everything. ThrowIfDirectoryNonEmpty to throw exception if the directory is not empty.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="onUserCancel">Throw exception when user cancel the UI operation or not.</param>
        ''' <remarks>If user wants shell features, onDirectoryNotEmpty is ignored. (VSWhidbey 283409)</remarks>
        '''**************************************************************************
        ''' ;DeleteFileInternal
        ''' <summary>
        ''' Delete the given file, with options to show progress UI, send file to Recycle Bin, throw exception if user cancels.
        ''' </summary>
        ''' <param name="file">the path to the file</param>
        ''' <param name="showUI">AllDialogs, OnlyErrorDialogs, or NoUI</param>
        ''' <param name="recycle">DeletePermanently or SendToRecycleBin</param>
        ''' <param name="onUserCancel">DoNothing or ThrowException</param>
        ''' <remarks></remarks>
        '''**************************************************************************
        ''' ;DemandDirectoryPermission
        ''' <summary>
        ''' Given a full directory path, demand the given access using FileIOPermission.
        ''' </summary>
        ''' <param name="fullDirectoryPath">The full path to the directory. This must be normalized.</param>
        ''' <param name="access">FileIOPermissionAccess value.</param>
        ''' <remarks>We add a \ to the end if needed to demand permission on the entire directory.</remarks>
        '''******************************************************************************
        ''' ;EnsurePathNotExist
        ''' <summary>
        ''' Verify that a path does not refer to an existing directory or file. Throw exception otherwise.
        ''' </summary>
        ''' <param name="Path">The path to verify.</param>
        ''' <remarks>This is used for RenameFile and RenameDirectory.</remarks>
        Private Sub EnsurePathNotExist(ByVal Path As String)
            If IO.File.Exists(Path) Then
                Throw ExUtils.GetIOException(SR.IO_FileExists_Path, Path)
            End If

            If IO.Directory.Exists(Path) Then
                Throw ExUtils.GetIOException(SR.IO_DirectoryExists_Path, Path)
            End If
        End Sub

        '''******************************************************************************
        ''' ;FileContainsText
        ''' <summary>
        ''' Determines if the given file in the path contains the given text.
        ''' </summary>
        ''' <param name="FilePath">The file to check for.</param>
        ''' <param name="Text">The text to search for.</param>
        ''' <returns>True if the file contains the text. Otherwise False.</returns>
        Private Function FileContainsText(ByVal FilePath As String, ByVal Text As String, ByVal IgnoreCase As Boolean) _
            As Boolean

            Debug.Assert(FilePath.Length <> 0 AndAlso IO.Path.IsPathRooted(FilePath), FilePath)
            Debug.Assert(Text.Length <> 0, "Empty text!!!")

            ' To support different encoding (UTF-8, ASCII).
            ' Read the file in byte, then use Decoder classes to get a string from those bytes and compare.
            ' Decoder class maintains state between the conversion, allowing it to correctly decode
            ' byte sequences that span adjacent blocks. (sources\ndp\clr\src\BCL\System\Text\Decoder.cs).

            Dim DEFAULT_BUFFER_SIZE As Integer = 1024 ' default buffer size to read each time.
            Dim FileStream As IO.FileStream = Nothing

            Try
                ' Open the file with ReadWrite share, least possibility to fail.
                FileStream = New IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)

                ' Read in a byte buffer with default size, then open a StreamReader to detect the encoding of the file.
                Dim DetectedEncoding As System.Text.Encoding = System.Text.Encoding.Default ' Use Default encoding as the fall back.
                Dim ByteBuffer(DEFAULT_BUFFER_SIZE - 1) As Byte
                Dim ByteCount As Integer = 0
                ByteCount = FileStream.Read(ByteBuffer, 0, ByteBuffer.Length)
                If ByteCount > 0 Then
                    ' Only take the number of bytes returned to avoid false detection. VSWhidbey 518893.
                    Dim MemoryStream As New IO.MemoryStream(ByteBuffer, 0, ByteCount)
                    Dim StreamReader As New IO.StreamReader(MemoryStream, DetectedEncoding, detectEncodingFromByteOrderMarks:=True)
                    StreamReader.ReadLine()
                    DetectedEncoding = StreamReader.CurrentEncoding
                End If

                ' Calculate the real buffer size to read in each time to ensure read in at least a character array
                ' as long as or longer than the given text.
                ' 1. Calculate the maximum number of bytes required to encode the given text in the detected encoding.
                ' 2. If it's larger than DEFAULT_BUFFER_SIZE, use it. Otherwise, use DEFAULT_BUFFER_SIZE.
                Dim MaxByteDetectedEncoding As Integer = DetectedEncoding.GetMaxByteCount(Text.Length)
                Dim BufferSize As Integer = Math.Max(MaxByteDetectedEncoding, DEFAULT_BUFFER_SIZE)

                ' Dim up the byte buffer and the search helpers (See TextSearchHelper).
                Dim SearchHelper As New TextSearchHelper(DetectedEncoding, Text, IgnoreCase)

                ' If the buffer size is larger than DEFAULT_BUFFER_SIZE, read more from the file stream
                ' to fill up the byte buffer.
                If BufferSize > DEFAULT_BUFFER_SIZE Then
                    ReDim Preserve ByteBuffer(BufferSize - 1)
                    ' Read maximum ByteBuffer.Length - ByteCount (from the initial read) bytes from the stream
                    ' into the ByteBuffer, starting at ByteCount position.
                    Dim AdditionalByteCount As Integer = FileStream.Read(ByteBuffer, ByteCount, ByteBuffer.Length - ByteCount)
                    ByteCount += AdditionalByteCount ' The total byte count now is ByteCount + AdditionalByteCount
                    Debug.Assert(ByteCount <= ByteBuffer.Length)
                End If

                ' Start the search and read until end of file.
                Do
                    If ByteCount > 0 Then
                        If SearchHelper.IsTextFound(ByteBuffer, ByteCount) Then
                            Return True
                        End If
                    End If
                    ByteCount = FileStream.Read(ByteBuffer, 0, ByteBuffer.Length)
                Loop While (ByteCount > 0)

                Return False
            Catch ex As Exception

                ' We don't expect the following types of exceptions, so we'll re-throw it together with Yukon's exceptions.
                Debug.Assert(Not (TypeOf ex Is ArgumentException OrElse TypeOf ex Is ArgumentOutOfRangeException OrElse
                    TypeOf ex Is ArgumentNullException OrElse TypeOf ex Is IO.DirectoryNotFoundException OrElse
                    TypeOf ex Is IO.FileNotFoundException OrElse TypeOf ex Is ObjectDisposedException OrElse
                    TypeOf ex Is RankException OrElse TypeOf ex Is ArrayTypeMismatchException OrElse
                    TypeOf ex Is InvalidCastException), "Unexpected exception: " & ex.ToString())

                ' These exceptions may happen and we'll return False here.
                If TypeOf ex Is IO.IOException OrElse
                    TypeOf ex Is NotSupportedException OrElse
                    TypeOf ex Is SecurityException OrElse
                    TypeOf ex Is UnauthorizedAccessException Then

                    Return False
                Else
                    ' Re-throw Yukon's exceptions, PathTooLong exception (linked directory) and others.
                    Throw
                End If
            Finally
                If FileStream IsNot Nothing Then
                    FileStream.Close()
                End If
            End Try
        End Function

        '''**************************************************************************
        ''' ;FindFilesOrDirectories
        ''' <summary>
        ''' Find files or directories in a directory and return them in a string collection.
        ''' </summary>
        ''' <param name="FileOrDirectory">Specify to search for file or directory.</param>
        ''' <param name="directory">The directory path to start from.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The search patterns to use for the file name ("*.*")</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the files that match the search condition.</returns>
        ''' <exception cref="System.ArgumentException">ArgumentNullException: If one of the pattern is Null, Empty or all-spaces string.</exception>
        Private Function FindFilesOrDirectories(ByVal FileOrDirectory As FileOrDirectory, ByVal directory As String,
            ByVal searchType As SearchOption, ByVal wildcards() As String) As ObjectModel.ReadOnlyCollection(Of String)

            Dim Results As New ObjectModel.Collection(Of String)
            FindFilesOrDirectories(FileOrDirectory, directory, searchType, wildcards, Results)

            Return New ObjectModel.ReadOnlyCollection(Of String)(Results)
        End Function

        '''**************************************************************************
        ''' ;FindFilesOrDirectories
        ''' <summary>
        ''' Find files or directories in a directory and return them in a string collection.
        ''' </summary>
        ''' <param name="FileOrDirectory">Specify to search for file or directory.</param>
        ''' <param name="directory">The directory path to start from.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The search patterns to use for the file name ("*.*")</param>
        ''' <param name="Results">A ReadOnlyCollection(Of String) containing the files that match the search condition.</param>
        Private Sub FindFilesOrDirectories(ByVal FileOrDirectory As FileOrDirectory, ByVal directory As String,
            ByVal searchType As SearchOption, ByVal wildcards() As String, ByVal Results As ObjectModel.Collection(Of String))
            Debug.Assert(Results IsNot Nothing, "Results is NULL!!!")

            ' Verify enums.
            VerifySearchOption("searchType", searchType)

            directory = NormalizePath(directory)

            ' Verify wild cards. Only TrimEnd since empty space is allowed at the start of file / directory name.
            If wildcards IsNot Nothing Then
                For Each wildcard As String In wildcards
                    ' Throw if empty string or Nothing.
                    If wildcard.TrimEnd().length = 0 Then
                        Throw ExUtils.GetArgumentNullException("wildcards", SR.IO_GetFiles_NullPattern)
                    End If
                Next
            End If

            ' Search for files / directories directly under given directory (based on wildcards).
            If wildcards Is Nothing OrElse wildcards.Length = 0 Then
                AddToStringCollection(Results, FindPaths(FileOrDirectory, directory, Nothing))
            Else
                For Each wildcard As String In wildcards
                    AddToStringCollection(Results, FindPaths(FileOrDirectory, directory, wildcard))
                Next
            End If

            ' Search in sub directories if specified.
            If searchType = SearchOption.SearchAllSubDirectories Then
                For Each SubDirectoryPath As String In IO.Directory.GetDirectories(directory)
                    FindFilesOrDirectories(FileOrDirectory, SubDirectoryPath, searchType, wildcards, Results)
                Next
            End If
        End Sub

        ''' **************************************************************************
        ''' ;FindPaths
        ''' <summary>
        ''' Given a directory, a pattern, find the files or directories directly under the given directory that match the pattern.
        ''' </summary>
        ''' <param name="FileOrDirectory">Specify whether to find files or directories.</param>
        ''' <param name="directory">The directory to look under.</param>
        ''' <param name="wildCard">*.bmp, *.txt, ... Nothing to search for every thing.</param>
        ''' <returns>An array of String containing the paths found.</returns>
        Private Function FindPaths(ByVal FileOrDirectory As FileOrDirectory, ByVal directory As String, ByVal wildCard As String) As String()
            If FileOrDirectory = FileSystem.FileOrDirectory.Directory Then
                If wildCard.Length = 0 Then
                    Return IO.Directory.GetDirectories(directory)
                Else
                    Return IO.Directory.GetDirectories(directory, wildCard)
                End If
            Else
                If wildCard.Length = 0 Then
                    Return IO.Directory.GetFiles(directory)
                Else
                    Return IO.Directory.GetFiles(directory, wildCard)
                End If
            End If
        End Function

        '''******************************************************************************
        ''' ;FxCopyOrMoveDirectory
        ''' <summary>
        ''' Copies or moves the directory using Framework.
        ''' </summary>
        ''' <param name="operation">Copy or Move.</param>
        ''' <param name="sourceDirectoryPath">Source path - must be full path.</param>
        ''' <param name="targetDirectoryPath">Target path - must be full path.</param>
        ''' <param name="Overwrite">True to overwrite the files. Otherwise, False.</param>
        ''' <exception cref="IO.IOException">Some files or directories cannot be copied or moved.</exception>
        Private Sub FxCopyOrMoveDirectory(ByVal operation As CopyOrMove,
            ByVal sourceDirectoryPath As String, ByVal targetDirectoryPath As String, ByVal overwrite As Boolean)

            Debug.Assert(System.Enum.IsDefined(GetType(CopyOrMove), operation), "Invalid Operation!!!")
            Debug.Assert(sourceDirectoryPath.Length <> 0 AndAlso IO.Path.IsPathRooted(sourceDirectoryPath), "Invalid Source!!!")
            Debug.Assert(targetDirectoryPath.Length <> 0 AndAlso IO.Path.IsPathRooted(targetDirectoryPath), "Invalid Target!!!")

            ' Special case for moving: If target directory does not exist, AND both directories are on same drive,
            '   use IO.Directory.Move for performance gain (not copying).
            If operation = CopyOrMove.Move AndAlso Not IO.Directory.Exists(targetDirectoryPath) And
                IsOnSameDrive(sourceDirectoryPath, targetDirectoryPath) Then

                ' Create the target's parent. IO.Directory.CreateDirectory won't throw if it exists.
                IO.Directory.CreateDirectory(GetParentPath(targetDirectoryPath))

                Try
                    IO.Directory.Move(sourceDirectoryPath, targetDirectoryPath)
                    Exit Sub
                Catch ex As IO.IOException
                Catch ex As UnauthorizedAccessException
                    ' VSWhidbey 289537: Ignore IO.Directory.Move specific exceptions here. Try to do as much as possible later.
                End Try
            End If

            ' Create the target, create the root node, and call the recursive function.
            System.IO.Directory.CreateDirectory(targetDirectoryPath)
            Debug.Assert(IO.Directory.Exists(targetDirectoryPath), "Should be able to create Target Directory!!!")

            Dim SourceDirectoryNode As New DirectoryNode(sourceDirectoryPath, targetDirectoryPath)
            Dim Exceptions As New ListDictionary
            CopyOrMoveDirectoryNode(operation, SourceDirectoryNode, overwrite, Exceptions)

            ' Throw the final exception if there were exceptions during copy / move.
            If Exceptions.Count > 0 Then
                Dim IOException As New IO.IOException(GetResourceString(SR.IO_CopyMoveRecursive))
                For Each Entry As DictionaryEntry In Exceptions
                    IOException.Data.Add(Entry.Key, Entry.Value)
                Next
                Throw IOException
            End If
        End Sub

        '''******************************************************************************
        ''' ;GetFullPathFromNewName
        ''' <summary>
        ''' Returns the fullpath from a directory path and a new name. Throws exception if the new name contains path information.
        ''' </summary>
        ''' <param name="Path">The directory path.</param>
        ''' <param name="NewName">The new name to combine to the directory path.</param>
        ''' <param name="ArgumentName">The argument name to throw in the exception.</param>
        ''' <returns>A String contains the full path.</returns>
        ''' <remarks>This function is used for CopyFile, RenameFile and RenameDirectory.</remarks>
        Private Function GetFullPathFromNewName(ByVal Path As String,
            ByVal NewName As String, ByVal ArgumentName As String) As String
            Debug.Assert(Path.Length <> 0 AndAlso IO.Path.IsPathRooted(Path), Path)
            Debug.Assert(Path.Equals(IO.Path.GetFullPath(Path)), Path)
            Debug.Assert(NewName.Length <> 0, "Null NewName!!!")
            Debug.Assert(ArgumentName.Length <> 0, "Null argument name!!!")

            ' In copy file, rename file and rename directory, the new name must be a name only.
            ' Enforce that by combine the path, normalize it, then compare the new parent directory with the old parent directory.
            ' These two directories must be the same.

            ' Throw exception if NewName contains any separator characters.
            If NewName.IndexOfAny(m_SeparatorChars) >= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(ArgumentName, SR.IO_ArgumentIsPath_Name_Path, ArgumentName, NewName)
            End If

            ' Call GetFullPath again to catch invalid characters in NewName.
            Dim FullPath As String = RemoveEndingSeparator(IO.Path.GetFullPath(IO.Path.Combine(Path, NewName)))

            ' If the new parent directory path does not equal the parent directory passed in, throw exception.
            ' Use this to check for cases like "..", checking for separators will not block this case.
            If Not GetParentPath(FullPath).Equals(Path, StringComparison.OrdinalIgnoreCase) Then
                Throw ExUtils.GetArgumentExceptionWithArgName(ArgumentName, SR.IO_ArgumentIsPath_Name_Path, ArgumentName, NewName)
            End If

            Return FullPath
        End Function

        '''******************************************************************************
        ''' ;GetLongPath
        ''' <summary>
        '''  Returns the given path in long format (v.s 8.3 format) if the path exists.
        ''' </summary>
        ''' <param name="FullPath">The path to resolve to long format.</param>
        ''' <returns>The given path in long format if the path exists.</returns>
        ''' <remarks>
        '''  GetLongPathName is a PInvoke call and requires unmanaged code permission.
        '''  Use DirectoryInfo.GetFiles and GetDirectories (which call FindFirstFile) so that we always have permission.
        '''</remarks>
        Private Function GetLongPath(ByVal FullPath As String) As String
            Debug.Assert(FullPath.Length = 0 AndAlso IO.Path.IsPathRooted(FullPath), "Must be full path!!!")

            Try
                ' If root path, return itself. UNC path do not recognize 8.3 format in root path, so this is fine.
                If IsRoot(FullPath) Then
                    Return FullPath
                End If

                ' DirectoryInfo.GetFiles and GetDirectories call FindFirstFile which resolves 8.3 path.
                ' Get the DirectoryInfo (user must have code permission or access permission).
                Dim DInfo As New IO.DirectoryInfo(GetParentPath(FullPath))

                If IO.File.Exists(FullPath) Then
                    Debug.Assert(DInfo.GetFiles(IO.Path.GetFileName(FullPath)).Length = 1, "Must found exactly 1!!!")
                    Return DInfo.GetFiles(IO.Path.GetFileName(FullPath))(0).FullName
                ElseIf IO.Directory.Exists(FullPath) Then
                    Debug.Assert(DInfo.GetDirectories(IO.Path.GetFileName(FullPath)).Length = 1,
                        "Must found exactly 1!!!")
                    Return DInfo.GetDirectories(IO.Path.GetFileName(FullPath))(0).FullName
                Else
                    Return FullPath ' Path does not exist, cannot resolve.
                End If
            Catch ex As Exception
                ' Ignore these type of exceptions and return FullPath. These type of exceptions should either be caught by calling functions
                ' or indicate that caller does not have enough permission and should get back the 8.3 path.
                If TypeOf ex Is ArgumentException OrElse
                    TypeOf ex Is ArgumentNullException OrElse
                    TypeOf ex Is IO.PathTooLongException OrElse
                    TypeOf ex Is NotSupportedException OrElse
                    TypeOf ex Is IO.DirectoryNotFoundException OrElse
                    TypeOf ex Is SecurityException OrElse
                    TypeOf ex Is UnauthorizedAccessException Then

                    Debug.Assert(Not (TypeOf ex Is ArgumentException OrElse
                        TypeOf ex Is ArgumentNullException OrElse
                        TypeOf ex Is IO.PathTooLongException OrElse
                        TypeOf ex Is NotSupportedException), "These exceptions should be caught above!!!")

                    Return FullPath
                Else
                    Throw
                End If
            End Try
        End Function

        '''******************************************************************************
        ''' ;GetOperationFlags
        ''' <summary>
        ''' Return the ShFileOperationFlags based on the ShowUI option.
        ''' </summary>
        ''' <param name="ShowUI">UIOptionInternal value.</param>
        Private Function GetOperationFlags(ByVal ShowUI As UIOptionInternal) As ShFileOperationFlags
            Dim OperationFlags As ShFileOperationFlags = m_SHELL_OPERATION_FLAGS_BASE
            If (ShowUI = UIOptionInternal.OnlyErrorDialogs) Then
                OperationFlags = OperationFlags Or m_SHELL_OPERATION_FLAGS_HIDE_UI
            End If
            Return OperationFlags
        End Function

        Private Function GetShellOperationInfo(
                    ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
                    ByVal SourcePath As String, Optional ByVal TargetPath As String = Nothing) As SHFILEOPSTRUCT
            Debug.Assert(SourcePath.Length <> 0 AndAlso IO.Path.IsPathRooted(SourcePath), "Invalid SourcePath!!!")

            Return GetShellOperationInfo(OperationType, OperationFlags, New String() {SourcePath}, TargetPath)
        End Function

        Private Function GetShellOperationInfo(
                    ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
                    ByVal SourcePaths() As String, Optional ByVal TargetPath As String = Nothing) As SHFILEOPSTRUCT
            Debug.Assert(System.Enum.IsDefined(GetType(SHFileOperationType), OperationType), "Invalid OperationType!!!")
            Debug.Assert(TargetPath.Length = 0 OrElse IO.Path.IsPathRooted(TargetPath), "Invalid TargetPath!!!")
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

        '''******************************************************************************
        ''' ;GetShellOperationInfo
        ''' <summary>
        ''' Returns an SHFILEOPSTRUCT used by SHFileOperation based on the given parameters.
        ''' </summary>
        ''' <param name="OperationType">One of the SHFileOperationType value: copy, move or delete.</param>
        ''' <param name="OperationFlags">Combination SHFileOperationFlags values: details of the operation.</param>
        ''' <param name="SourcePath">The source file / directory path.</param>
        ''' <param name="TargetPath">The target file / directory path. Nothing in case of delete.</param>
        ''' <returns>A fully initialized SHFILEOPSTRUCT.</returns>
        '''******************************************************************************
        ''' ;GetShellOperationInfo
        ''' <summary>
        ''' Returns an SHFILEOPSTRUCT used by SHFileOperation based on the given parameters.
        ''' </summary>
        ''' <param name="OperationType">One of the SHFileOperationType value: copy, move or delete.</param>
        ''' <param name="OperationFlags">Combination SHFileOperationFlags values: details of the operation.</param>
        ''' <param name="SourcePaths">A string array containing the paths of source files. Must not be empty.</param>
        ''' <param name="TargetPath">The target file / directory path. Nothing in case of delete.</param>
        ''' <returns>A fully initialized SHFILEOPSTRUCT.</returns>
        '''******************************************************************************
        ''' ;GetShellPath
        ''' <summary>
        ''' Returns the special path format required for pFrom and pTo of SHFILEOPSTRUCT. See NativeMethod.
        ''' </summary>
        ''' <param name="FullPath">The full path to be converted.</param>
        ''' <returns>A string in the required format.</returns>
        Private Function GetShellPath(ByVal FullPath As String) As String
            Debug.Assert(FullPath.Length <> 0 AndAlso IO.Path.IsPathRooted(FullPath), "Must be full path!!!")

            Return GetShellPath(New String() {FullPath})
        End Function

        '''******************************************************************************
        ''' ;GetShellPath
        ''' <summary>
        ''' Returns the special path format required for pFrom and pTo of SHFILEOPSTRUCT. See NativeMethod.
        ''' </summary>
        ''' <param name="FullPaths">A string array containing the paths for the operation.</param>
        ''' <returns>A string in the required format.</returns>
        Private Function GetShellPath(ByVal FullPaths() As String) As String
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

        '''******************************************************************************
        ''' ;IsOnSameDrive
        ''' <summary>
        ''' Checks to see if the two paths is on the same drive.
        ''' </summary>
        ''' <param name="Path1"></param>
        ''' <param name="Path2"></param>
        ''' <returns>True if the 2 paths is on the same drive. False otherwise.</returns>
        ''' <remarks>Just a string comparison.</remarks>
        Private Function IsOnSameDrive(ByVal Path1 As String, ByVal Path2 As String) As Boolean
            ' Remove any separators at the end for the same reason in IsRoot.
            Path1 = Path1.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
            Path2 = Path2.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
            Return String.Compare(IO.Path.GetPathRoot(Path1), IO.Path.GetPathRoot(Path2),
                    StringComparison.OrdinalIgnoreCase) = 0
        End Function

        '''**************************************************************************
        ''' ;IsRoot
        ''' <summary>
        ''' Checks if the full path is a root path.
        ''' </summary>
        ''' <param name="Path">The path to check.</param>
        ''' <returns>True if FullPath is a root path, False otherwise.</returns>
        ''' <remarks>
        '''   IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\machine\share -> \\machine\share,
        '''           BUT \\machine\share\ -> \\machine\share (No separator here).
        '''   Therefore, remove any separators at the end to have correct result.
        ''' </remarks>
        Private Function IsRoot(ByVal Path As String) As Boolean
            ' This function accepts a relative path since GetParentPath will call this,
            ' and GetParentPath accept relative paths.
            If Not IO.Path.IsPathRooted(Path) Then
                Return False
            End If

            Path = Path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
            Return String.Compare(Path, IO.Path.GetPathRoot(Path),
                    StringComparison.OrdinalIgnoreCase) = 0
        End Function

        '''**************************************************************************
        ''' ;RemoveEndingSeparator
        ''' <summary>
        ''' Removes all directory separators at the end of a path.
        ''' </summary>
        ''' <param name="Path">a full or relative path.</param>
        ''' <returns>If Path is a root path, the same value. Otherwise, removes any directory separators at the end.</returns>
        ''' <remarks>We decided not to return path with separators at the end (VsWhidbey 54741).</remarks>
        Private Function RemoveEndingSeparator(ByVal Path As String) As String
            If IO.Path.IsPathRooted(Path) Then
                ' If the path is rooted, attempt to check if it is a root path.
                ' Note: IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\myshare\mydir -> \\myshare\mydir
                ' BUT \\myshare\mydir\ -> \\myshare\mydir!!! This function will remove the ending separator of
                ' \\myshare\mydir\ as well. Do not use IsRoot here.
                If Path.Equals(IO.Path.GetPathRoot(Path), StringComparison.OrdinalIgnoreCase) Then
                    Return Path
                End If
            End If

            ' Otherwise, remove all separators at the end.
            Return Path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
        End Function
#If HaveUI Then

        '''******************************************************************************
        ''' ;ShellCopyOrMove
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
        ''' !!!!! SECURITY WARNING !!!!
        ''' Demand appropriate FileIOPermission on FullSource and FullTarget before calling into this method.
        ''' </remarks>
        Private Sub ShellCopyOrMove(ByVal Operation As CopyOrMove, ByVal TargetType As FileOrDirectory,
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
        ''' ;ShellDelete
        ''' <summary>
        ''' Sets relevant flags on the SHFILEOPSTRUCT and calls into SHFileOperation to delete file / directory.
        ''' </summary>
        ''' <param name="FullPath">Full path to the file / directory.</param>
        ''' <param name="ShowUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="OnUserCancel">Throw exception or not if the operation was canceled (by user or errors in the system).</param>
        ''' <remarks>
        ''' We don't need to consider Recursive flag here since we already verify that in DeleteDirectory.
        ''' !!!!! SECURITY WARNING !!!!
        ''' Demand appropriate FileIOPermission on FullSource and FullTarget before calling into this method.
        ''' </remarks>
        Private Sub ShellDelete(ByVal FullPath As String,
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
        ''' ;ShellFileOperation
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
        Private Sub ShellFileOperation(ByVal OperationType As SHFileOperationType, ByVal OperationFlags As ShFileOperationFlags,
            ByVal FullSource As String, ByVal FullTarget As String, ByVal OnUserCancel As UICancelOption, ByVal FileOrDirectory As FileOrDirectory)

            ' Apply HostProtectionAttribute(UI = true) to indicate this function belongs to UI type.
            ' http://devdiv/SpecTool/Documents/Whidbey/CLR/CurrentSpecs/SQLHost/hPA%20Guidance.doc

            Debug.Assert(System.Enum.IsDefined(GetType(SHFileOperationType), OperationType))
            Debug.Assert(OperationType <> SHFileOperationType.FO_RENAME, "Don't call Shell to rename!!!")
            Debug.Assert(FullSource.Length <> 0 AndAlso IO.Path.IsPathRooted(FullSource), "Invalid FullSource path!!!")
            Debug.Assert(OperationType = SHFileOperationType.FO_DELETE OrElse (FullTarget.Length <> 0 AndAlso IO.Path.IsPathRooted(FullTarget)), "Invalid FullTarget path!!!")

            ' Demand the necessary permissions: UIPermission.
            Dim UIPermission As New UIPermission(UIPermissionWindow.SafeSubWindows)
            UIPermission.Demand()

            ' Demand FileIOPermission, defense in depth. VSWhidbey 427776. CONSIDER: Perf hit?
            ' Demand permission on source path. Get the correct permission access based on operation type.
            Dim SourceIOPermissionAccess As FileIOPermissionAccess = FileIOPermissionAccess.NoAccess
            If OperationType = SHFileOperationType.FO_COPY Then
                SourceIOPermissionAccess = FileIOPermissionAccess.Read
            ElseIf OperationType = SHFileOperationType.FO_MOVE Then
                SourceIOPermissionAccess = FileIOPermissionAccess.Read Or FileIOPermissionAccess.Write
            ElseIf OperationType = SHFileOperationType.FO_DELETE Then
                SourceIOPermissionAccess = FileIOPermissionAccess.Write
            End If
            ' FullSource might end with '\*' (for copying and moving) so normalize the path to the correct format to demand the permission.
            Dim CheckPermissionPath As String = FullSource
            If (OperationType = SHFileOperationType.FO_COPY OrElse OperationType = SHFileOperationType.FO_MOVE) _
                    AndAlso CheckPermissionPath.EndsWith("*", StringComparison.Ordinal) Then
                CheckPermissionPath = RemoveEndingSeparator(FullSource.TrimEnd("*"c))
            End If
            ' Demand the permission on source file or directory.
            If FileOrDirectory = FileSystem.FileOrDirectory.Directory Then
                DemandDirectoryPermission(CheckPermissionPath, SourceIOPermissionAccess)
            Else
                Call (New FileIOPermission(SourceIOPermissionAccess, CheckPermissionPath)).Demand()
            End If
            ' Demand permission on target file or directory. Only in copy / move.
            If OperationType <> SHFileOperationType.FO_DELETE Then
                If FileOrDirectory = FileSystem.FileOrDirectory.Directory Then
                    DemandDirectoryPermission(FullTarget, FileIOPermissionAccess.Write)
                Else
                    Call (New FileIOPermission(FileIOPermissionAccess.Write, FullTarget)).Demand()
                End If
            End If

            ' Get the SHFILEOPSTRUCT
            Dim OperationInfo As SHFILEOPSTRUCT = GetShellOperationInfo(OperationType, OperationFlags, FullSource, FullTarget)

            Dim Result As Integer

            ' Assert UnmanagedCodePermission to call Win32 methods.
            'Call (New SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Assert()

            Try
                Result = NativeMethods.SHFileOperation(OperationInfo)
                ' Notify the shell in case some changes happened.
                NativeMethods.SHChangeNotify(SHChangeEventTypes.SHCNE_DISKEVENTS,
                    SHChangeEventParameterFlags.SHCNF_DWORD, IntPtr.Zero, IntPtr.Zero)
            Catch
                Throw
            Finally
                CodeAccessPermission.RevertAssert()
            End Try

            ' If the operation was canceled, check OnUserCancel and throw OperationCanceledException if needed.
            ' Otherwise, check the result and throw the appropriate exception if there is an error code.
            ' VSWhidbey 253382, 468577.
            If OperationInfo.fAnyOperationsAborted Then
                If OnUserCancel = UICancelOption.ThrowException Then
                    Throw New OperationCanceledException()
                End If
            ElseIf Result <> 0 Then
                ThrowWinIOError(Result)
            End If
        End Sub
#End If

        '''**************************************************************************
        ''' ;ThrowIfDevicePath
        ''' <summary>
        ''' Throw an argument exception if the given path starts with "\\.\" (device path).
        ''' </summary>
        ''' <param name="path">The path to check.</param>
        ''' <remarks>
        ''' VSWhidbey 230286.
        ''' FileStream already throws exception with device path, so our code only check for device path in Copy / Move / Delete / Rename.
        ''' </remarks>
        Private Sub ThrowIfDevicePath(ByVal path As String)
            If path.StartsWith("\\.\", StringComparison.Ordinal) Then
                Throw ExceptionUtils.GetArgumentExceptionWithArgName("path", SR.IO_DevicePath)
            End If
        End Sub

        ''''**************************************************************************
        '''' ;ThrowWinIOError
        '''' <summary>
        '''' Given an error code from winerror.h, throw the appropriate exception.
        '''' </summary>
        '''' <param name="errorCode">An error code from winerror.h.</param>
        '''' <remarks>
        '''' - This method is based on sources\ndp\clr\src\BCL\System\IO\_Error.cs::WinIOError, except the following.
        '''' - Exception message does not contain the path since at this point it is normalized.
        '''' - Instead of using PInvoke of GetMessage and MakeHRFromErrorCode, use managed code.
        '''' </remarks>

        'Private Sub ThrowWinIOError(ByVal errorCode As Integer)
        '    Select Case errorCode
        '        Case NativeTypes.ERROR_FILE_NOT_FOUND
        '            Throw New IO.FileNotFoundException()
        '        Case NativeTypes.ERROR_PATH_NOT_FOUND
        '            Throw New IO.DirectoryNotFoundException()
        '        Case NativeTypes.ERROR_ACCESS_DENIED
        '            Throw New UnauthorizedAccessException()
        '        Case NativeTypes.ERROR_FILENAME_EXCED_RANGE
        '            Throw New IO.PathTooLongException()
        '        Case NativeTypes.ERROR_INVALID_DRIVE
        '            Throw New IO.DriveNotFoundException()
        '        Case NativeTypes.ERROR_OPERATION_ABORTED, NativeTypes.ERROR_CANCELLED
        '            Throw New OperationCanceledException()
        '        Case Else
        '            ' Including these from _Error.cs::WinIOError.
        '            'Case NativeTypes.ERROR_ALREADY_EXISTS
        '            'Case NativeTypes.ERROR_INVALID_PARAMETER
        '            'Case NativeTypes.ERROR_SHARING_VIOLATION
        '            'Case NativeTypes.ERROR_FILE_EXISTS
        '            Throw New IO.IOException((New ComponentModel.Win32Exception(errorCode)).Message,
        '                System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error())
        '    End Select
        'End Sub

        '''**************************************************************************
        ''' ;ToUIOptionInternal
        ''' <summary>
        ''' Convert UIOption to UIOptionInternal to use internally.
        ''' </summary>
        ''' <remarks>
        ''' To fix common issues of VSWhidbey 474856, 499359; only accept valid UIOption values.
        ''' </remarks>
        Private Function ToUIOptionInternal(ByVal showUI As UIOption) As UIOptionInternal
            Select Case showUI
                Case FileIO.UIOption.AllDialogs
                    Return UIOptionInternal.AllDialogs
                Case FileIO.UIOption.OnlyErrorDialogs
                    Return UIOptionInternal.OnlyErrorDialogs
                Case Else
                    Throw New System.ComponentModel.InvalidEnumArgumentException("showUI", showUI, GetType(UIOption))
            End Select
        End Function

        '''**************************************************************************
        ''' ;VerifyDeleteDirectoryOption
        ''' <summary>
        ''' Verify that the given argument value is a valid DeleteDirectoryOption. If not, throw InvalidEnumArgumentException.
        ''' </summary>
        ''' <param name="argName">The argument name.</param>
        ''' <param name="argValue">The argument value.</param>
        ''' <remarks>VSWhidbey 522083.</remarks>
        Private Sub VerifyDeleteDirectoryOption(ByVal argName As String, ByVal argValue As DeleteDirectoryOption)
            If argValue = FileIO.DeleteDirectoryOption.DeleteAllContents OrElse
                argValue = FileIO.DeleteDirectoryOption.ThrowIfDirectoryNonEmpty Then
                Exit Sub
            End If

            Throw New InvalidEnumArgumentException(argName, argValue, GetType(DeleteDirectoryOption))
        End Sub

        '''**************************************************************************
        ''' ;VerifyRecycleOption
        ''' <summary>
        ''' Verify that the given argument value is a valid RecycleOption. If not, throw InvalidEnumArgumentException.
        ''' </summary>
        ''' <param name="argName">The argument name.</param>
        ''' <param name="argValue">The argument value.</param>
        Private Sub VerifyRecycleOption(ByVal argName As String, ByVal argValue As RecycleOption)
            If argValue = RecycleOption.DeletePermanently OrElse
                argValue = RecycleOption.SendToRecycleBin Then
                Exit Sub
            End If

            Throw New InvalidEnumArgumentException(argName, argValue, GetType(RecycleOption))
        End Sub

        '''**************************************************************************
        ''' ;VerifySearchOption
        ''' <summary>
        ''' Verify that the given argument value is a valid SearchOption. If not, throw InvalidEnumArgumentException.
        ''' </summary>
        ''' <param name="argName">The argument name.</param>
        ''' <param name="argValue">The argument value.</param>
        Private Sub VerifySearchOption(ByVal argName As String, ByVal argValue As SearchOption)
            If argValue = SearchOption.SearchAllSubDirectories OrElse
                argValue = SearchOption.SearchTopLevelOnly Then
                Exit Sub
            End If

            Throw New InvalidEnumArgumentException(argName, argValue, GetType(SearchOption))
        End Sub

        '''**************************************************************************
        ''' ;VerifyUICancelOption
        ''' <summary>
        ''' Verify that the given argument value is a valid UICancelOption. If not, throw InvalidEnumArgumentException.
        ''' </summary>
        ''' <param name="argName">The argument name.</param>
        ''' <param name="argValue">The argument value.</param>
        Private Sub VerifyUICancelOption(ByVal argName As String, ByVal argValue As UICancelOption)
            If argValue = UICancelOption.DoNothing OrElse
                argValue = UICancelOption.ThrowException Then
                Exit Sub
            End If

            Throw New InvalidEnumArgumentException(argName, argValue, GetType(UICancelOption))
        End Sub

        'FileOperationFlags

    End Class

End Namespace
