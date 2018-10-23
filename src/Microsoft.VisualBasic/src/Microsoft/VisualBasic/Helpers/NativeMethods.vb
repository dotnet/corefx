Imports System
Imports System.Runtime.InteropServices
Imports System.Runtime.Versioning
Imports System.Security
Imports Microsoft.VisualBasic.FileIO.FileSystem

Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Friend NotInheritable Class NativeMethods
        <SecurityCritical()>
        <ResourceExposure(ResourceScope.None)>
        <PreserveSig()>
        Friend Declare Function _
            CloseHandle _
                Lib "kernel32" (ByVal hObject As IntPtr) As Integer

        <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="MessageBox", SetLastError:=False)>
        Friend Shared Function MessageBox(hWnd As Integer, lpText As String, lpCaption As String, uType As UInteger) As Integer
        End Function

        ''' <summary>
        ''' Given a 32-bit SHFILEOPSTRUCT, call the appropriate SHFileOperation function
        ''' to perform shell file operation.
        ''' </summary>
        ''' <param name="lpFileOp">32-bit SHFILEOPSTRUCT</param>
        ''' <returns>0 if successful, non-zero otherwise.</returns>
        <SecurityCritical()>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Friend Shared Function SHFileOperation(ByRef lpFileOp As SHFILEOPSTRUCT) As Int32
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
                Dim result As Int32 = SHFileOperation64(lpFileOp64)

                ' Only need to check if any operations were aborted.
                lpFileOp.fAnyOperationsAborted = lpFileOp64.fAnyOperationsAborted

                Return result
            End If
        End Function

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
        <SecurityCritical()>
        <ResourceExposure(ResourceScope.Machine)>
        <DllImport("shell32.dll", CharSet:=CharSet.Auto, EntryPoint:="SHFileOperation", SetLastError:=True, ThrowOnUnmappableChar:=True)>
        Private Shared Function SHFileOperation32(ByRef lpFileOp As SHFILEOPSTRUCT) As Int32
        End Function

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
            Friend wFunc As UInt32
            <MarshalAs(UnmanagedType.LPTStr)> Friend pFrom As String
            <MarshalAs(UnmanagedType.LPTStr)> Friend pTo As String
            Friend fFlags As UInt16
            Friend fAnyOperationsAborted As Boolean
            Friend hNameMappings As IntPtr
            <MarshalAs(UnmanagedType.LPTStr)> Friend lpszProgressTitle As String
        End Structure

        ''' <summary>
        ''' Copies, moves, renames or deletes a file system object on 64-bit platforms.
        ''' </summary>
        <SecurityCritical()>
        <ResourceExposure(ResourceScope.Machine)>
        <DllImport("shell32.dll", CharSet:=CharSet.Auto, EntryPoint:="SHFileOperation", SetLastError:=True, ThrowOnUnmappableChar:=True)>
        Private Shared Function SHFileOperation64(ByRef lpFileOp As SHFILEOPSTRUCT64) As Integer
        End Function


        ''' <summary>
        ''' Contains information that the SHFileOperation function uses to perform file operations
        ''' on 64-bit platforms, where the structure is unpacked. VSWhidbey 421265,
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
        Private Structure SHFILEOPSTRUCT64
            Friend hwnd As IntPtr
            Friend wFunc As UInt32
            <MarshalAs(UnmanagedType.LPTStr)> Friend pFrom As String
            <MarshalAs(UnmanagedType.LPTStr)> Friend pTo As String
            Friend fFlags As UInt16
            Friend fAnyOperationsAborted As Boolean
            Friend hNameMappings As IntPtr
            <MarshalAs(UnmanagedType.LPTStr)> Friend lpszProgressTitle As String
        End Structure

        ''' <summary>
        ''' Values that indicate which file operation to perform. Used in SHFILEOPSTRUCT
        ''' </summary>
        Friend Enum SHFileOperationType As UInt32
            FO_MOVE = &H1
            FO_COPY = &H2
            FO_DELETE = &H3
            FO_RENAME = &H4
        End Enum

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
    End Class
End Namespace
