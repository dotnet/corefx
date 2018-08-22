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
Imports System.Globalization
Imports System.Runtime.Versioning
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
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
        Sub New()

        End Sub

        '= PUBLIC =============================================================

        '== PROPERTIES ========================================================

        '''******************************************************************************
        ''' ;Drives
        ''' <summary>
        ''' Return the names of all available drives on the computer.
        ''' </summary>
        ''' <value>A ReadOnlyCollection(Of DriveInfo) containing all the current drives' names.</value>
        Public Shared ReadOnly Property Drives() As ObjectModel.ReadOnlyCollection(Of System.IO.DriveInfo)
            Get
                ' NOTE: Don't cache the collection since it may change without us knowing.
                ' The performance hit will be small since it's a small collection.
                ' CONSIDER: : Create a read-only collection from an array?
                Dim DriveInfoCollection As New ObjectModel.Collection(Of System.IO.DriveInfo)
                For Each DriveInfo As System.IO.DriveInfo In IO.DriveInfo.GetDrives()
                    DriveInfoCollection.Add(DriveInfo)
                Next
                Return New ObjectModel.ReadOnlyCollection(Of System.IO.DriveInfo)(DriveInfoCollection)
            End Get
        End Property

        '''**************************************************************************
        ''' ;CurrentDirectory
        ''' <summary>
        ''' Get or set the current working directory.
        ''' </summary>
        ''' <value>A String containing the path to the directory.</value>
        Public Shared Property CurrentDirectory() As String
            Get
                Return NormalizePath(IO.Directory.GetCurrentDirectory())
            End Get
            Set(ByVal value As String)
                IO.Directory.SetCurrentDirectory(value)
            End Set
        End Property

        '== FUNCTIONS =========================================================

        '''**************************************************************************
        ''' ;CombinePath
        ''' <summary>
        ''' Combines two path strings by adding a path separator.
        ''' </summary>
        ''' <param name="baseDirectory">The first part of the path.</param>
        ''' <param name="relativePath">The second part of the path, must be a relative path.</param>
        ''' <returns>A String contains the combined path.</returns>
        Public Shared Function CombinePath(ByVal baseDirectory As String, ByVal relativePath As String) As String

            ' VSWhidbey 258686.
            If baseDirectory.Length = 0 Then
                Throw ExUtils.GetArgumentNullException("baseDirectory", SR.General_ArgumentEmptyOrNothing_Name, "baseDirectory")
            End If
            If relativePath.Length = 0 Then
                Return baseDirectory
            End If

            baseDirectory = IO.Path.GetFullPath(baseDirectory) ' Throw exceptions if BaseDirectoryPath is invalid.

            Return NormalizePath(IO.Path.Combine(baseDirectory, relativePath))
        End Function

        '''**************************************************************************
        ''' ;DirectoryExists
        ''' <summary>
        '''  Determines whether the given path refers to an existing directory on disk.
        ''' </summary>
        ''' <param name="directory">The path to verify.</param>
        ''' <returns>True if DirectoryPath refers to an existing directory. Otherwise, False.</returns>
        Public Shared Function DirectoryExists(ByVal directory As String) As Boolean
            Return IO.Directory.Exists(directory)
        End Function

        '''**************************************************************************
        ''' ;FileExists
        ''' <summary>
        '''  Determines whether the given path refers to an existing file on disk.
        ''' </summary>
        ''' <param name="file">The path to verify.</param>
        ''' <returns>True if FilePath refers to an existing file on disk. Otherwise, False.</returns>
        Public Shared Function FileExists(ByVal file As String) As Boolean
            If Not String.IsNullOrEmpty(file) AndAlso
                (file.EndsWith(IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Or
                file.EndsWith(IO.Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) Then
                Return False
            End If

            Return IO.File.Exists(file)
        End Function

        '''**************************************************************************
        ''' ;FindInFiles
        ''' <summary>
        ''' Find files in the given folder that contain the given text.
        ''' </summary>
        ''' <param name="directory">The folder path to start from.</param>
        ''' <param name="containsText">The text to be found in file.</param>
        ''' <param name="ignoreCase">True to ignore case. Otherwise, False.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <returns>A string array containing the files that match the search condition.</returns>
        Public Shared Function FindInFiles(ByVal directory As String,
            ByVal containsText As String, ByVal ignoreCase As Boolean, ByVal searchType As SearchOption) As ObjectModel.ReadOnlyCollection(Of String)
            Return FindInFiles(directory, containsText, ignoreCase, searchType, Nothing)
        End Function

        '''**************************************************************************
        ''' ;FindInFiles
        ''' <summary>
        ''' Find files in the given folder that contain the given text.
        ''' </summary>
        ''' <param name="directory">The folder path to start from.</param>
        ''' <param name="containsText">The text to be found in file.</param>
        ''' <param name="ignoreCase">True to ignore case. Otherwise, False.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="fileWildcards">The search patterns to use for the file name ("*.*")</param>
        ''' <returns>A string array containing the files that match the search condition.</returns>
        ''' <exception cref="System.ArgumentNullException">If one of the pattern is Null, Empty or all-spaces string.</exception>
        Public Shared Function FindInFiles(ByVal directory As String, ByVal containsText As String, ByVal ignoreCase As Boolean,
            ByVal searchType As SearchOption, ByVal ParamArray fileWildcards() As String) As ObjectModel.ReadOnlyCollection(Of String)

            ' Find the files with matching name.
            Dim NameMatchFiles As ObjectModel.ReadOnlyCollection(Of String) = FindFilesOrDirectories(
                FileOrDirectory.File, directory, searchType, fileWildcards)

            ' Find the files containing the given text.
            If containsText.Length <> 0 Then
                Dim ContainTextFiles As New ObjectModel.Collection(Of String)
                For Each FilePath As String In NameMatchFiles
                    If (FileContainsText(FilePath, containsText, ignoreCase)) Then
                        ContainTextFiles.Add(FilePath)
                    End If
                Next
                Return New ObjectModel.ReadOnlyCollection(Of String)(ContainTextFiles)
            Else
                Return NameMatchFiles
            End If
        End Function

        '''**************************************************************************
        ''' ;GetDirectories
        ''' <summary>
        ''' Return the paths of sub directories found directly under a directory.
        ''' </summary>
        ''' <param name="directory">The directory to find the sub directories inside.</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched directories' paths.</returns>
        Public Shared Function GetDirectories(ByVal directory As String) As ObjectModel.ReadOnlyCollection(Of String)

            Return FindFilesOrDirectories(FileOrDirectory.Directory, directory, SearchOption.SearchTopLevelOnly, Nothing)
        End Function

        '''**************************************************************************
        ''' ;GetDirectories
        ''' <summary>
        ''' Return the paths of sub directories found under a directory with the specified name patterns.
        ''' </summary>
        ''' <param name="directory">The directory to find the sub directories inside.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The wildcards for the file name, for example "*.bmp", "*.txt"</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched directories' paths.</returns>
        Public Shared Function GetDirectories(ByVal directory As String, ByVal searchType As SearchOption,
            ByVal ParamArray wildcards() As String) As ObjectModel.ReadOnlyCollection(Of String)

            Return FindFilesOrDirectories(FileOrDirectory.Directory, directory, searchType, wildcards)
        End Function

        '''**************************************************************************
        ''' ;GetDirectoryInfo
        ''' <summary>
        '''  Returns the information object about the specified directory.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <returns>A DirectoryInfo object containing the information about the specified directory.</returns>
        Public Shared Function GetDirectoryInfo(ByVal directory As String) As System.IO.DirectoryInfo
            Return New System.IO.DirectoryInfo(directory)
        End Function

        '''**************************************************************************
        ''' ;GetDriveInfo
        ''' <summary>
        ''' Return the information about the specified drive.
        ''' </summary>
        ''' <param name="drive">The path to the drive.</param>
        ''' <returns>A DriveInfo object containing the information about the specified drive.</returns>
        Public Shared Function GetDriveInfo(ByVal drive As String) As System.IO.DriveInfo
            Return New System.IO.DriveInfo(drive)
        End Function

        '''**************************************************************************
        ''' ;GetFileInfo
        ''' <summary>
        '''  Returns the information about the specified file.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <returns>A FileInfo object containing the information about the specified file.</returns>
        Public Shared Function GetFileInfo(ByVal file As String) As System.IO.FileInfo
            file = NormalizeFilePath(file, "file")
            Return New System.IO.FileInfo(file)
        End Function

        '''**************************************************************************
        ''' ;GetFiles
        ''' <summary>
        ''' Return a collection of file paths found directly under a directory.
        ''' </summary>
        ''' <param name="directory">The directory to find the files inside.</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched files' paths.</returns>
        Public Shared Function GetFiles(ByVal directory As String) As ObjectModel.ReadOnlyCollection(Of String)
            Return FindFilesOrDirectories(FileOrDirectory.File, directory, SearchOption.SearchTopLevelOnly, Nothing)
        End Function

        '''**************************************************************************
        ''' ;GetFiles
        ''' <summary>
        ''' Return a collection of file paths found under a directory with the specified name patterns and containing the specified text.
        ''' </summary>
        ''' <param name="directory">The directory to find the files inside.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The wildcards for the file name, for example "*.bmp", "*.txt"</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched files' paths.</returns>
        Public Shared Function GetFiles(ByVal directory As String, ByVal searchType As SearchOption,
            ByVal ParamArray wildcards() As String) As ObjectModel.ReadOnlyCollection(Of String)

            Return FindFilesOrDirectories(FileOrDirectory.File, directory, searchType, wildcards)
        End Function

        '''**************************************************************************
        ''' ;GetName
        ''' <summary>
        ''' Return the name (and extension) from the given path string.
        ''' </summary>
        ''' <param name="path">The path string from which to obtain the file name (and extension).</param>
        ''' <returns>A String containing the name of the file or directory.</returns>
        ''' <exception cref="ArgumentException">path contains one or more of the invalid characters defined in InvalidPathChars.</exception>
        Public Shared Function GetName(ByVal path As String) As String
            Return IO.Path.GetFileName(path)
        End Function

        '''**************************************************************************
        ''' ;GetParentPath
        ''' <summary>
        ''' Returns the parent directory's path from a specified path.
        ''' </summary>
        ''' <param name="path">The path to a file or directory, this can be absolute or relative.</param>
        ''' <returns>
        ''' The path to the parent directory of that file or directory (whether absolute or relative depends on the input),
        ''' or an empty string if Path is a root directory.
        ''' </returns>
        ''' <exception cref="IO.Path.GetFullPath">See IO.Path.GetFullPath: If path is an invalid path.</exception>
        ''' <remarks>
        ''' The path will be normalized (for example: C:\Dir1////\\\Dir2 will become C:\Dir1\Dir2)
        ''' but will not be resolved (for example: C:\Dir1\Dir2\..\Dir3 WILL NOT become C:\Dir1\Dir3). Use CombinePath.
        ''' </remarks>
        Public Shared Function GetParentPath(ByVal path As String) As String
            ' Call IO.Path.GetFullPath to handle exception cases. Don't use the full path returned.
            IO.Path.GetFullPath(path)

            If IsRoot(path) Then
                Throw ExUtils.GetArgumentExceptionWithArgName("path", SR.IO_GetParentPathIsRoot_Path, path)
            Else
                Return IO.Path.GetDirectoryName(path.TrimEnd(
                    IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))
            End If
        End Function

        '''**************************************************************************
        ''' ;GetTempFileName
        ''' <summary>
        ''' Create a uniquely named zero-byte temporary file on disk and return the full path to that file.
        ''' </summary>
        ''' <returns>A String containing the name of the temporary file.</returns>
        Public Shared Function GetTempFileName() As String
            Return System.IO.Path.GetTempFileName()
        End Function

        '''**************************************************************************
        ''' ;OpenTextFieldParser
        ''' <summary>
        ''' Return an instance of a TextFieldParser for the given file.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <returns>An instance of a TextFieldParser.</returns>
        Public Shared Function OpenTextFieldParser(ByVal file As String) As TextFieldParser
            Return New TextFieldParser(file)
        End Function

        '''**************************************************************************
        ''' ;OpenTextFieldParser
        ''' <summary>
        ''' Return an instance of a TextFieldParser for the given file using the given delimiters.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <param name="delimiters">A list of delimiters.</param>
        ''' <returns>An instance of a TextFieldParser</returns>
        Public Shared Function OpenTextFieldParser(ByVal file As String, ByVal ParamArray delimiters As String()) As TextFieldParser
            Dim Result As New TextFieldParser(file)
            Result.SetDelimiters(delimiters)
            Result.TextFieldType = FieldType.Delimited
            Return Result
        End Function

        '''**************************************************************************
        ''' ;OpenTextFieldParser
        ''' <summary>
        ''' Return an instance of a TextFieldParser for the given file using the given field widths.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <param name="fieldWidths">A list of field widths.</param>
        ''' <returns>An instance of a TextFieldParser</returns>
        Public Shared Function OpenTextFieldParser(ByVal file As String, ByVal ParamArray fieldWidths As Integer()) As TextFieldParser
            Dim Result As New TextFieldParser(file)
            Result.SetFieldWidths(fieldWidths)
            Result.TextFieldType = FieldType.FixedWidth
            Return Result
        End Function

        '''**************************************************************************
        ''' ;OpenTextFieldParser
        ''' <summary>
        ''' Return a StreamReader for reading the given file using UTF-8 as preferred encoding.
        ''' </summary>
        ''' <param name="file">The file to open the StreamReader on.</param>
        ''' <returns>An instance of System.IO.StreamReader opened on the file (with FileShare.Read).</returns>
        Public Shared Function OpenTextFileReader(ByVal file As String) As IO.StreamReader
            Return OpenTextFileReader(file, Encoding.UTF8)
        End Function

        '''**************************************************************************
        ''' ;OpenTextFileReader
        ''' <summary>
        ''' Return a StreamReader for reading the given file using the given encoding as preferred encoding.
        ''' </summary>
        ''' <param name="file">The file to open the StreamReader on.</param>
        ''' <param name="Encoding">The preferred encoding that will be used if the encoding of the file could not be detected.</param>
        ''' <returns>An instance of System.IO.StreamReader opened on the file (with FileShare.Read).</returns>
        Public Shared Function OpenTextFileReader(ByVal file As String, ByVal encoding As Encoding) As IO.StreamReader

            file = NormalizeFilePath(file, "file")
            Return New IO.StreamReader(file, encoding, detectEncodingFromByteOrderMarks:=True)
        End Function

        '''**************************************************************************
        ''' ;OpenTextFileWriter
        ''' <summary>
        ''' Return a StreamWriter for writing to the given file using UTF-8 encoding.
        ''' </summary>
        ''' <param name="file">The file to write to.</param>
        ''' <param name="Append">True to append to the content of the file. False to overwrite the content of the file.</param>
        ''' <returns>An instance of StreamWriter opened on the file (with FileShare.Read).</returns>
        Public Shared Function OpenTextFileWriter(ByVal file As String, ByVal append As Boolean) As IO.StreamWriter
            Return OpenTextFileWriter(file, append, Encoding.UTF8)
        End Function

        '''**************************************************************************
        ''' ;OpenTextFileWriter
        ''' <summary>
        ''' Return a StreamWriter for writing to the given file using the given encoding.
        ''' </summary>
        ''' <param name="file">The file to write to.</param>
        ''' <param name="Append">True to append to the content of the file. False to overwrite the content of the file.</param>
        ''' <param name="Encoding">The encoding to use to write to the file.</param>
        ''' <returns>An instance of StreamWriter opened on the file (with FileShare.Read).</returns>
        Public Shared Function OpenTextFileWriter(ByVal file As String, ByVal append As Boolean,
            ByVal encoding As Encoding) As IO.StreamWriter

            file = NormalizeFilePath(file, "file")
            Return New IO.StreamWriter(file, append, encoding)
        End Function

        '''**************************************************************************
        ''' ;ReadAllBytes
        ''' <summary>
        ''' Read the whole content of a file into a byte array.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <returns>A byte array contains the content of the file.</returns>
        ''' <exception cref="IO.IOException">If the length of the file is larger than Integer.MaxValue (~2GB).</exception>
        ''' <exception cref="IO.FileStream">See FileStream constructor and Read: for other exceptions.</exception>
        Public Shared Function ReadAllBytes(ByVal file As String) As Byte()
            Return IO.File.ReadAllBytes(file)
        End Function

        '''**************************************************************************
        ''' ;ReadAllText
        ''' <summary>
        ''' Read the whole content of a text file into a string using UTF-8 encoding.
        ''' </summary>
        ''' <param name="file">The path to the text file.</param>
        ''' <returns>A String contains the content of the given file.</returns>
        ''' <exception cref="IO.StreamReader">See StreamReader constructor and ReadToEnd.</exception>
        Public Shared Function ReadAllText(ByVal file As String) As String
            Return IO.File.ReadAllText(file)
        End Function

        '''**************************************************************************
        ''' ;ReadAllText
        ''' <summary>
        ''' Read the whole content of a text file into a string using the given encoding.
        ''' </summary>
        ''' <param name="file">The path to the text file.</param>
        ''' <param name="encoding">The character encoding to use if the encoding was not detected.</param>
        ''' <returns>A String contains the content of the given file.</returns>
        ''' <exception cref="IO.StreamReader">See StreamReader constructor and ReadToEnd.</exception>
        Public Shared Function ReadAllText(ByVal file As String, ByVal encoding As Encoding) As String
            Return IO.File.ReadAllText(file, encoding)
        End Function

        '== METHODS ===========================================================

        '''**************************************************************************
        ''' ;CopyDirectory
        ''' <summary>
        ''' Copy an existing directory to a new directory,
        ''' throwing exception if there are existing files with the same name.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub CopyDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String)
            CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName,
                False, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;CopyDirectory
        ''' <summary>
        ''' Copy an existing directory to a new directory,
        ''' overwriting existing files with the same name if specified.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="overwrite">True to overwrite existing files with the same name. Otherwise False.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub CopyDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal overwrite As Boolean)
            CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName,
                overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub
#If HaveUI Then

        '''**************************************************************************
        ''' ;CopyDirectory
        ''' <summary>
        ''' Copy an existing directory to a new directory,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' throwing exception if user cancels the operation (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub CopyDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal showUI As UIOption)
            CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName,
                False, ToUIOptionInternal(showUI), UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;CopyDirectory
        ''' <summary>
        ''' Copy an existing directory to a new directory,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' throwing exception if user cancels the operation if specified. (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub CopyDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal showUI As UIOption, ByVal onUserCancel As UICancelOption)
            CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName,
                False, ToUIOptionInternal(showUI), onUserCancel)
        End Sub
#End If

        '''**************************************************************************
        ''' ;CopyFile
        ''' <summary>
        ''' Copy an existing file to a new file. Overwriting a file of the same name is not allowed.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub CopyFile(ByVal sourceFileName As String, ByVal destinationFileName As String)
            CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName,
                False, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;CopyFile
        ''' <summary>
        ''' Copy an existing file to a new file. Overwriting a file of the same name if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="overwrite">True to overwrite existing file with the same name. Otherwise False.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub CopyFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal overwrite As Boolean)
            CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName,
                overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

#If HaveUI Then

        '''**************************************************************************
        ''' ;CopyFile
        ''' <summary>
        ''' Copy an existing file to a new file,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' will throw exception if user cancels the operation.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub CopyFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal showUI As UIOption)
            CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName,
                False, ToUIOptionInternal(showUI), UICancelOption.ThrowException)
        End Sub


        '''**************************************************************************
        ''' ;CopyFile
        ''' <summary>
        ''' Copy an existing file to a new file,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' will throw exception if user cancels the operation if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.</param>
        ''' <remarks>onUserCancel will be ignored if showUI = HideDialogs.</remarks>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub CopyFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal showUI As UIOption, ByVal onUserCancel As UICancelOption)
            CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName,
                False, ToUIOptionInternal(showUI), onUserCancel)
        End Sub
#End If


        '''**************************************************************************
        ''' ;CreateDirectory
        ''' <summary>
        '''  Creates a directory from the given path (including all parent directories).
        ''' </summary>
        ''' <param name="directory">The path to create the directory at.</param>
        Public Shared Sub CreateDirectory(ByVal directory As String)
            ' Get the full path. GetFullPath will throw if invalid path.
            directory = IO.Path.GetFullPath(directory)

            If IO.File.Exists(directory) Then ' CONSIDER: : Pending on VSWhidbey 104049.
                Throw ExUtils.GetIOException(SR.IO_FileExists_Path, directory)
            End If

            ' CreateDirectory will create the full structure and not throw if directory exists.
            System.IO.Directory.CreateDirectory(directory)
        End Sub


        '''**************************************************************************
        ''' ;DeleteDirectory
        ''' <summary>
        ''' Delete the given directory, with options to recursively delete.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="onDirectoryNotEmpty">DeleteAllContents to delete everything. ThrowIfDirectoryNonEmpty to throw exception if the directory is not empty.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub DeleteDirectory(ByVal directory As String, ByVal onDirectoryNotEmpty As DeleteDirectoryOption)
            DeleteDirectoryInternal(directory, onDirectoryNotEmpty,
                UIOptionInternal.NoUI, RecycleOption.DeletePermanently, UICancelOption.ThrowException)
        End Sub

#If HaveUI Then
        '''**************************************************************************
        ''' ;DeleteDirectory
        ''' <summary>
        ''' Delete the given directory, with options to recursively delete, show progress UI, send file to Recycle Bin; throwing exception if user cancels.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="showUI">True to shows progress window. Otherwise, False.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub DeleteDirectory(ByVal directory As String, ByVal showUI As UIOption, ByVal recycle As RecycleOption)
            DeleteDirectoryInternal(directory, DeleteDirectoryOption.DeleteAllContents,
                ToUIOptionInternal(showUI), recycle, UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;DeleteDirectory
        ''' <summary>
        ''' Delete the given directory, with options to recursively delete, show progress UI, send file to Recycle Bin, and whether to throw exception if user cancels.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="onUserCancel">Throw exception when user cancel the UI operation or not.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub DeleteDirectory(ByVal directory As String,
            ByVal showUI As UIOption, ByVal recycle As RecycleOption, ByVal onUserCancel As UICancelOption)
            DeleteDirectoryInternal(directory, DeleteDirectoryOption.DeleteAllContents,
                ToUIOptionInternal(showUI), recycle, onUserCancel)
        End Sub
#End If

        '''**************************************************************************
        ''' ;DeleteFile
        ''' <summary>
        ''' Delete the given file.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub DeleteFile(ByVal file As String)
            DeleteFileInternal(file, UIOptionInternal.NoUI, RecycleOption.DeletePermanently, UICancelOption.ThrowException)
        End Sub

#If HaveUI Then

        '''**************************************************************************
        ''' ;DeleteFile
        ''' <summary>
        ''' Delete the given file, with options to show progress UI, delete to recycle bin.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub DeleteFile(ByVal file As String, ByVal showUI As UIOption, ByVal recycle As RecycleOption)
            DeleteFileInternal(file, ToUIOptionInternal(showUI), recycle, UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;DeleteFile
        ''' <summary>
        ''' Delete the given file, with options to show progress UI, delete to recycle bin, and whether to throw exception if user cancels.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="onUserCancel">Throw exception when user cancel the UI operation or not.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath() exceptions: if FilePath is invalid.</exception>
        ''' <exception cref="IO.FileNotFoundException">if a file does not exist at FilePath</exception>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub DeleteFile(ByVal file As String, ByVal showUI As UIOption, ByVal recycle As RecycleOption,
            ByVal onUserCancel As UICancelOption)

            DeleteFileInternal(file, ToUIOptionInternal(showUI), recycle, onUserCancel)
        End Sub
#End If

        '''**************************************************************************
        ''' ;MoveDirectory
        ''' <summary>
        ''' Move an existing directory to a new directory,
        ''' throwing exception if there are existing files with the same name.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub MoveDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String)
            CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName,
                False, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;MoveDirectory
        ''' <summary>
        ''' Move an existing directory to a new directory,
        ''' overwriting existing files with the same name if specified.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>        ''' <param name="overwrite">True to overwrite existing files with the same name. Otherwise False.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub MoveDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal overwrite As Boolean)
            CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName,
                overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

#If HaveUI Then
        '''**************************************************************************
        ''' ;MoveDirectory
        ''' <summary>
        ''' Move an existing directory to a new directory,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' throwing exception if user cancels the operation (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub MoveDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal showUI As UIOption)
            CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName,
                False, ToUIOptionInternal(showUI), UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;MoveDirectory
        ''' <summary>
        ''' Move an existing directory to a new directory,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' throwing exception if user cancels the operation if specified. (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub MoveDirectory(ByVal sourceDirectoryName As String, ByVal destinationDirectoryName As String, ByVal showUI As UIOption, ByVal onUserCancel As UICancelOption)
            CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName,
                False, ToUIOptionInternal(showUI), onUserCancel)
        End Sub
#End If

        '''**************************************************************************
        ''' ;MoveFile
        ''' <summary>
        ''' Move an existing file to a new file. Overwriting a file of the same name is not allowed.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub MoveFile(ByVal sourceFileName As String, ByVal destinationFileName As String)
            CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName,
                False, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub


        '''**************************************************************************
        ''' ;MoveFile
        ''' <summary>
        ''' Move an existing file to a new file. Overwriting a file of the same name if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="overwrite">True to overwrite existing file with the same name. Otherwise False.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Shared Sub MoveFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal overwrite As Boolean)
            CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName,
                overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException)
        End Sub

#If HaveUI Then

        '''**************************************************************************
        ''' ;MoveFile
        ''' <summary>
        ''' Move an existing file to a new file,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' will throw exception if user cancels the operation.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub MoveFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal showUI As UIOption)
            CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName,
                False, ToUIOptionInternal(showUI), UICancelOption.ThrowException)
        End Sub

        '''**************************************************************************
        ''' ;MoveFile
        ''' <summary>
        ''' Move an existing file to a new file,
        ''' displaying progress dialog and confirmation dialogs if specified,
        ''' will throw exception if user cancels the operation if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.</param>
        ''' <remarks>onUserCancel will be ignored if showUI = HideDialogs.</remarks>
        <ResourceExposure(ResourceScope.Machine)>
        <ResourceConsumption(ResourceScope.Machine)>
        Public Sub MoveFile(ByVal sourceFileName As String, ByVal destinationFileName As String, ByVal showUI As UIOption, ByVal onUserCancel As UICancelOption)
            CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName,
                False, ToUIOptionInternal(showUI), onUserCancel)
        End Sub
#End If

        '''**************************************************************************
        ''' ;RenameDirectory
        ''' <summary>
        ''' Rename a directory, does not act like a move.
        ''' </summary>
        ''' <param name="directory">The path of the directory to be renamed.</param>
        ''' <param name="newName">The new name to change to. This must not contain path information.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath exceptions: If directory is invalid.</exception>
        ''' <exception cref="System.ArgumentException">If newName is Nothing or Empty String or contains path information.</exception>
        ''' <exception cref="IO.FileNotFoundException">If directory does not point to an existing directory.</exception>
        ''' <exception cref="IO.IOException">If directory points to a root directory.
        '''     Or if there's an existing directory or an existing file with the same name.</exception>
        Public Shared Sub RenameDirectory(ByVal directory As String, ByVal newName As String)
            ' Get the full path. This will handle invalid path exceptions.
            directory = IO.Path.GetFullPath(directory)
            ' Throw if device path.
            ThrowIfDevicePath(directory)

            ' Directory is a root directory. This does not require IO access so it's cheaper up front.
            If IsRoot(directory) Then
                Throw ExUtils.GetIOException(SR.IO_DirectoryIsRoot_Path, directory)
            End If

            ' Throw if directory does not exist.
            If Not IO.Directory.Exists(directory) Then
                Throw ExUtils.GetDirectoryNotFoundException(SR.IO_DirectoryNotFound_Path, directory)
            End If

            ' Verify newName is not null.
            If newName.Length = 0 Then
                Throw ExUtils.GetArgumentNullException(
                    "newName", SR.General_ArgumentEmptyOrNothing_Name, "newName")
            End If

            ' Calculate new path. GetFullPathFromNewName will verify newName is only a name.
            Dim FullNewPath As String = GetFullPathFromNewName(GetParentPath(directory), newName, "newName")
            Debug.Assert(GetParentPath(FullNewPath).Equals(GetParentPath(directory),
                StringComparison.OrdinalIgnoreCase), "Invalid FullNewPath!!!")

            ' Verify that the new path does not conflict.
            EnsurePathNotExist(FullNewPath)

            IO.Directory.Move(directory, FullNewPath)
        End Sub


        '''**************************************************************************
        ''' ;RenameFile
        ''' <summary>
        ''' Renames a file, does not change the file location.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="newName">The new name to change to. This must not contain path information.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath exceptions: If file is invalid.</exception>
        ''' <exception cref="System.ArgumentException">If newName is Nothing or Empty String or contains path information.</exception>
        ''' <exception cref="IO.FileNotFoundException">If file does not point to an existing file.</exception>
        ''' <exception cref="IO.IOException">If there's an existing directory or an existing file with the same name.</exception>
        Public Shared Sub RenameFile(ByVal file As String, ByVal newName As String)
            ' Get the full path. This will handle invalid path exceptions.
            file = NormalizeFilePath(file, "file")
            ' Throw if device path.
            ThrowIfDevicePath(file)

            ' Throw if file does not exist.
            If Not IO.File.Exists(file) Then
                Throw ExUtils.GetFileNotFoundException(file, SR.IO_FileNotFound_Path, file)
            End If

            ' Verify newName is not null.
            If newName.Length = 0 Then
                Throw ExUtils.GetArgumentNullException(
                    "newName", SR.General_ArgumentEmptyOrNothing_Name, "newName")
            End If

            ' Calculate new path. GetFullPathFromNewName will verify that newName is only a name.
            Dim FullNewPath As String = GetFullPathFromNewName(GetParentPath(file), newName, "newName")
            Debug.Assert(GetParentPath(FullNewPath).Equals(GetParentPath(file),
                StringComparison.OrdinalIgnoreCase), "Invalid FullNewPath!!!")

            ' Verify that the new path does not conflict.
            EnsurePathNotExist(FullNewPath)

            IO.File.Move(file, FullNewPath)
        End Sub


        '''**************************************************************************
        ''' ;WriteAllBytes
        ''' <summary>
        ''' Overwrites or appends the specified byte array to the specified file,
        ''' creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="data">The byte array to write to the file.</param>
        ''' <param name="append">True to append the text to the existing content. False to overwrite the existing content.</param>
        ''' <exception cref="IO.FileStream">See FileStream constructor and Write: For other exceptions.</exception>
        Public Shared Sub WriteAllBytes(ByVal file As String, ByVal data() As Byte, ByVal append As Boolean)

            ' VSWhidbey 445570: Cannot call through IO.File.WriteAllBytes (since they don't support append)
            ' so only check for trailing separator as specified in VSWhidbey 372980.
            CheckFilePathTrailingSeparator(file, NameOf(file))

            Dim FileStream As IO.FileStream = Nothing
            Try
                Dim IOFileMode As IO.FileMode
                If append Then
                    IOFileMode = IO.FileMode.Append
                Else
                    IOFileMode = IO.FileMode.Create ' CreateNew or Truncate.
                End If

                FileStream = New IO.FileStream(file,
                    mode:=IOFileMode, access:=IO.FileAccess.Write, share:=IO.FileShare.Read)
                FileStream.Write(data, 0, data.Length)
            Finally
                If Not FileStream Is Nothing Then
                    FileStream.Close()
                End If
            End Try
        End Sub


        '''**************************************************************************
        ''' ;WriteAllText
        ''' <summary>
        ''' Overwrites or appends the given text using UTF-8 encoding to the given file,
        ''' creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="text">The text to write to the file.</param>
        ''' <param name="append">True to append the text to the existing content. False to overwrite the existing content.</param>
        ''' <exception cref="IO.StreamWriter">See StreamWriter constructor and Write: For other exceptions.</exception>
        Public Shared Sub WriteAllText(ByVal file As String, ByVal text As String, ByVal append As Boolean)
            WriteAllText(file, text, append, Encoding.UTF8)
        End Sub

        '''**************************************************************************
        ''' ;WriteAllText
        ''' <summary>
        ''' Overwrites or appends the given text using the given encoding to the given file,
        ''' creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="text">The text to write to the file.</param>
        ''' <param name="append">True to append the text to the existing content. False to overwrite the existing content.</param>
        ''' <param name="encoding">The encoding to use.</param>
        ''' <exception cref="IO.StreamWriter">See StreamWriter constructor and Write: For other exceptions.</exception>
        Public Shared Sub WriteAllText(ByVal file As String, ByVal text As String, ByVal append As Boolean,
            ByVal encoding As Encoding)

            ' VSWhidbey 445570: Cannot call through IO.File.WriteAllText (since they don't support: append, prefer current encoding than specified one)
            ' so only check for trailing separator as specified in VSWhidbey 372980.
            CheckFilePathTrailingSeparator(file, NameOf(file))

            Dim StreamWriter As IO.StreamWriter = Nothing
            Try
                ' If appending to a file and it exists, attempt to detect the current encoding and use it (VSWhidbey 199224).
                If append AndAlso IO.File.Exists(file) Then
                    Dim StreamReader As IO.StreamReader = Nothing
                    Try
                        StreamReader = New IO.StreamReader(file, encoding, detectEncodingFromByteOrderMarks:=True)
                        Dim Chars(10 - 1) As Char
                        StreamReader.Read(Chars, 0, 10) ' Read the next 10 characters to activate auto detect encoding.
                        encoding = StreamReader.CurrentEncoding ' Set encoding to the detected encoding.
                    Catch ex As IO.IOException
                        ' Ignore IOException.
                    Finally
                        If StreamReader IsNot Nothing Then
                            StreamReader.Close()
                        End If
                    End Try
                End If

                ' StreamWriter uses FileShare.Read by default.
                StreamWriter = New IO.StreamWriter(file, append, encoding)
                StreamWriter.Write(text)
            Finally
                If Not StreamWriter Is Nothing Then
                    StreamWriter.Close()
                End If
            End Try
        End Sub

        '= FRIEND =============================================================

        '''**************************************************************************
        ''' ;NormalizeFilePath
        ''' <summary>
        ''' Normalize the path, but throw exception if the path ends with separator.
        ''' </summary>
        ''' <param name="Path">The input path.</param>
        ''' <param name="ParamName">The parameter name to include in the exception if one is raised.</param>
        ''' <returns>The normalized path.</returns>
        ''' <remarks>VSWhidbey 372980.</remarks>
        Friend Shared Function NormalizeFilePath(ByVal Path As String, ByVal ParamName As String) As String
            CheckFilePathTrailingSeparator(Path, ParamName)
            Return NormalizePath(Path)
        End Function

        '''**************************************************************************
        ''' ;NormalizePath
        ''' <summary>
        ''' Get full path, get long format, and remove any pending separator.
        ''' </summary>
        ''' <param name="Path">The path to be normalized.</param>
        ''' <returns>The normalized path.</returns>
        ''' <exception cref="IO.Path.GetFullPath">See IO.Path.GetFullPath for possible exceptions.</exception>
        ''' <remarks>Keep this function since we might change the implementation / behavior later.</remarks>
        Friend Shared Function NormalizePath(ByVal Path As String) As String
            Return GetLongPath(RemoveEndingSeparator(IO.Path.GetFullPath(Path)))
        End Function

        '''**************************************************************************
        ''' ;CheckFilePathTrailingSeparator
        ''' <summary>
        ''' Throw ArgumentException if the file path ends with a separator. (VSWhidbey 372980).
        ''' </summary>
        ''' <param name="path">The file path.</param>
        ''' <param name="paramName">The parameter name to include in ArgumentException.</param>
        Friend Shared Sub CheckFilePathTrailingSeparator(ByVal path As String, ByVal paramName As String)
            If path.Length = 0 Then ' Check for argument null - VSWhidbey 452078.
                Throw ExUtils.GetArgumentNullException(paramName)
            End If
            If path.EndsWith(IO.Path.DirectorySeparatorChar, StringComparison.Ordinal) Or
                path.EndsWith(IO.Path.AltDirectorySeparatorChar, StringComparison.Ordinal) Then
                Throw ExUtils.GetArgumentExceptionWithArgName(paramName, SR.IO_FilePathException)
            End If
        End Sub

        '= PRIVATE ============================================================

        ''' **************************************************************************
        ''' ;AddToStringCollection
        ''' <summary>
        ''' Add an array of string into a Generic Collection of String.
        ''' </summary>
        Private Shared Sub AddToStringCollection(ByVal StrCollection As ObjectModel.Collection(Of String), ByVal StrArray() As String)
            ' CONSIDER: : BCL to support adding an array of string directly into a generic string collection?
            Debug.Assert(StrCollection IsNot Nothing, "StrCollection is NULL!!!")

            If StrArray IsNot Nothing Then
                For Each Str As String In StrArray
                    If Not StrCollection.Contains(Str) Then
                        StrCollection.Add(Str)
                    End If
                Next
            End If
        End Sub

        '''**************************************************************************
        ''' ;DirectoryNode
        ''' <summary>
        ''' A simple tree node to build up the directory structure used for a snapshot in Copy / Move Directory.
        ''' </summary>
        Private Class DirectoryNode

            '''**************************************************************************
            ''' ;New
            ''' <summary>
            ''' Given a DirectoryPath, create the node and add the sub-directory nodes.
            ''' </summary>
            ''' <param name="DirectoryPath">Path to the directory. NOTE: must exist.</param>
            ''' <param name="TargetDirectoryPath">Path to the target directory of the move / copy. NOTE: must be a full path.</param>
            Friend Sub New(ByVal DirectoryPath As String, ByVal TargetDirectoryPath As String)
                Debug.Assert(IO.Directory.Exists(DirectoryPath), "Directory does not exist!!!")
                Debug.Assert(TargetDirectoryPath.Length <> 0 AndAlso IO.Path.IsPathRooted(TargetDirectoryPath), "Invalid TargetPath!!!")

                Path = DirectoryPath
                TargetPath = TargetDirectoryPath
                SubDirs = New ObjectModel.Collection(Of DirectoryNode)
                For Each SubDirPath As String In IO.Directory.GetDirectories(Path)
                    Dim SubTargetDirPath As String = IO.Path.Combine(TargetPath, IO.Path.GetFileName(SubDirPath))
                    SubDirs.Add(New DirectoryNode(SubDirPath, SubTargetDirPath))
                Next
            End Sub

            '''**************************************************************************
            ''' ;Path
            ''' <summary>
            ''' Return the Path of the current node.
            ''' </summary>
            ''' <value>A String containing the Path of the current node.</value>
            Friend ReadOnly Property Path() As String

            '''**************************************************************************
            ''' ;TargetPath
            ''' <summary>
            ''' Return the TargetPath for copy / move.
            ''' </summary>
            ''' <value>A String containing the copy / move target path of the current node.</value>
            Friend ReadOnly Property TargetPath() As String

            '''**************************************************************************
            ''' ;SubDirs
            ''' <summary>
            ''' Return the sub directories of the current node.
            ''' </summary>
            ''' <value>A Collection(Of DirectoryNode) containing the sub-directory nodes.</value>
            Friend ReadOnly Property SubDirs() As ObjectModel.Collection(Of DirectoryNode)

        End Class 'Private Class DirectoryNode

        '''**************************************************************************
        ''' ;TextSearchHelper
        ''' <summary>
        ''' Helper class to search for text in an array of byte using a specific Decoder.
        ''' </summary>
        ''' <remarks>
        ''' To search for text that might exist in an encoding, construct this class with the text and Decoder.
        '''      Then call IsTextFound() and pass in byte arrays.
        ''' This class will take care of text spanning byte arrays by caching a part of the array and use it in
        '''      the next IsTextFound() call.
        ''' </remarks>
        Private Class TextSearchHelper

            '''**************************************************************************
            ''' ;New
            ''' <summary>
            ''' Constructs a new helper with a given encoding and a text to search for.
            ''' </summary>
            ''' <param name="Encoding">The Encoding to use to convert byte to text.</param>
            ''' <param name="Text">The text to search for in subsequent byte array.</param>
            Friend Sub New(ByVal Encoding As Text.Encoding, ByVal Text As String, ByVal IgnoreCase As Boolean)
                Debug.Assert(Encoding IsNot Nothing, "Null Decoder!!!")
                Debug.Assert(Text.Length <> 0, "Empty Text!!!")

                m_Decoder = Encoding.GetDecoder
                m_Preamble = Encoding.GetPreamble
                m_IgnoreCase = IgnoreCase

                ' If use wants to ignore case, convert search text to lower case.
                If m_IgnoreCase Then
                    m_SearchText = Text.ToUpper(CultureInfo.CurrentCulture)
                Else
                    m_SearchText = Text
                End If
            End Sub

            '''**************************************************************************
            ''' ;IsTextFound
            ''' <summary>
            ''' Determines whether the text is found in the given byte array.
            ''' </summary>
            ''' <param name="ByteBuffer">The byte array to find the text in</param>
            ''' <param name="Count">The number of valid bytes in the byte array</param>
            ''' <returns>True if the text is found. Otherwise, False.</returns>
            Friend Function IsTextFound(ByVal ByteBuffer() As Byte, ByVal Count As Integer) As Boolean
                Debug.Assert(ByteBuffer IsNot Nothing, "Null ByteBuffer!!!")
                Debug.Assert(Count > 0, Count.ToString(CultureInfo.InvariantCulture))
                Debug.Assert(m_Decoder IsNot Nothing, "Null Decoder!!!")
                Debug.Assert(m_Preamble IsNot Nothing, "Null Preamble!!!")

                Dim ByteBufferStartIndex As Integer = 0 ' If need to handle BOM, ByteBufferStartIndex will increase.

                ' Check for the preamble the first time IsTextFound is called. If find it, shrink ByteBuffer.
                If m_CheckPreamble Then
                    If BytesMatch(ByteBuffer, m_Preamble) Then
                        ByteBufferStartIndex = m_Preamble.Length
                        Count -= m_Preamble.Length ' Reduce the valid byte count if ByteBuffer was shrinked (VSWhidbey 361409).
                    End If
                    m_CheckPreamble = False
                    ' In case of an empty file with BOM at the beginning (VSWhidbey 518893), return FALSE.
                    If Count <= 0 Then
                        Return False
                    End If
                End If

                ' Get the number of characters in the byte array.
                Dim ExpectedCharCount As Integer = m_Decoder.GetCharCount(ByteBuffer, ByteBufferStartIndex, Count)

                ' The character buffer used to search will be a combination of the cached buffer and the current one.
                Dim CharBuffer(m_PreviousCharBuffer.Length + ExpectedCharCount - 1) As Char
                ' Start the buffer with the cached buffer.
                Array.Copy(sourceArray:=m_PreviousCharBuffer, sourceIndex:=0,
                    destinationArray:=CharBuffer, destinationIndex:=0, length:=m_PreviousCharBuffer.Length)
                ' And fill the rest with the ones from byte array.
                Dim CharCount As Integer = m_Decoder.GetChars(
                    bytes:=ByteBuffer, byteIndex:=ByteBufferStartIndex, byteCount:=Count,
                    chars:=CharBuffer, charIndex:=m_PreviousCharBuffer.Length)
                Debug.Assert(CharCount = ExpectedCharCount, "Should read all characters!!!")

                ' Refresh the cached buffer for the possible next search.
                If CharBuffer.Length > m_SearchText.Length Then
                    If m_PreviousCharBuffer.Length <> m_SearchText.Length Then
                        ReDim m_PreviousCharBuffer(m_SearchText.Length - 1)
                    End If
                    Array.Copy(sourceArray:=CharBuffer, sourceIndex:=(CharBuffer.Length - m_SearchText.Length),
                        destinationArray:=m_PreviousCharBuffer, destinationIndex:=0, length:=m_SearchText.Length)
                Else
                    m_PreviousCharBuffer = CharBuffer
                End If

                ' If user wants to ignore case, convert new string to lower case. m_SearchText was converted in constructor.
                If m_IgnoreCase Then
                    Return New String(CharBuffer).ToUpper(CultureInfo.CurrentCulture).Contains(m_SearchText)
                Else
                    Return New String(CharBuffer).Contains(m_SearchText)
                End If
            End Function

            '''**************************************************************************
            ''' ;New
            ''' <summary>
            ''' No default constructor.
            ''' </summary>
            Private Sub New()
            End Sub

            '''**************************************************************************
            ''' <summary>
            ''' Returns whether the big buffer starts with the small buffer.
            ''' </summary>
            ''' <param name="BigBuffer"></param>
            ''' <param name="SmallBuffer"></param>
            ''' <returns>True if BigBuffer starts with SmallBuffer.Otherwise, False.</returns>
            Private Shared Function BytesMatch(ByVal BigBuffer() As Byte, ByVal SmallBuffer() As Byte) As Boolean
                Debug.Assert(BigBuffer.Length > SmallBuffer.Length, "BigBuffer should be longer!!!")
                If BigBuffer.Length < SmallBuffer.Length OrElse SmallBuffer.Length = 0 Then
                    Return False
                End If
                For i As Integer = 0 To SmallBuffer.Length - 1
                    If BigBuffer(i) <> SmallBuffer(i) Then
                        Return False
                    End If
                Next
                Return True
            End Function

            Private m_SearchText As String ' The text to search.
            Private ReadOnly m_IgnoreCase As Boolean ' Should we ignore case?
            Private m_Decoder As Text.Decoder ' The Decoder to use.
            Private m_PreviousCharBuffer() As Char = Array.Empty(Of Char)() ' The cached character array from previous call to IsTextExist.
            Private m_CheckPreamble As Boolean = True ' True to check for preamble. False otherwise.
            Private m_Preamble() As Byte ' The byte order mark we need to consider.
        End Class 'Private Class TextSearchHelper

    End Class

    '''**************************************************************************
    ''' ;DeleteDirectoryOption
    ''' <summary>
    ''' Specify the action to do when deleting a directory and it is not empty.
    ''' </summary>
    ''' <remarks>
    ''' Again, avoid Integer values that VB Compiler will convert Boolean to (0 and -1). VSWhidbey 522083.
    ''' IMPORTANT: Change VerifyDeleteDirectoryOption if this enum is changed.
    ''' Also, values in DeleteDirectoryOption must be different from UIOption. VSWhidbey 491042.
    ''' </remarks>
    Public Enum DeleteDirectoryOption As Integer
        ThrowIfDirectoryNonEmpty = 4
        DeleteAllContents = 5
    End Enum

    '''**************************************************************************
    ''' ;RecycleOption
    ''' <summary>
    ''' Specify whether to delete a file / directory to Recycle Bin or not.
    ''' </summary>
    Public Enum RecycleOption As Integer
        DeletePermanently = 2
        SendToRecycleBin = 3
    End Enum

    '''**************************************************************************
    ''' ;SearchOption
    ''' <summary>
    ''' Specify whether to perform the search for files/directories recursively or not.
    ''' </summary>
    Public Enum SearchOption As Integer
        SearchTopLevelOnly = 2
        SearchAllSubDirectories = 3
    End Enum

    '''**************************************************************************
    ''' ;UICancelOption
    ''' <summary>
    ''' Defines option whether to throw exception when user cancels a UI operation or not.
    ''' </summary>
    Public Enum UICancelOption As Integer
        DoNothing = 2
        ThrowException = 3
    End Enum

    '''**************************************************************************
    ''' ;UIOption
    ''' <summary>
    ''' Specify which UI dialogs to show.
    ''' </summary>
    ''' <remarks>
    ''' To fix common issues of VSWhidbey 474856, 499359; avoid Integer values that VB Compiler
    ''' will convert Boolean to (0 and -1).
    ''' </remarks>
    Public Enum UIOption As Integer
        OnlyErrorDialogs = 2
        AllDialogs = 3
    End Enum

End Namespace

' NOTE:
' - All path returned by us will NOT have the Directory Separator Character ('\') at the end. (VSWhidbey 54741).
' - All path accepted by us will NOT consider the meaning of Directory Separator Character ('\') at the end.
' - Parameter accepting path will accept both relative and absolute paths unless specified.
'       Relative paths will be resolved using the current working directory.
' - IO.Path.GetFullPath is used to normalized the path. It will only throw in case of not well-formed path.
' - Hidden Files and Directories will be moved / copied by Framework code.
'
' - On both Read and Write, we use the default Share mode that FX uses for the StreamReader/Writer, which is Share.Read.
'   Details on what share mode means:
'       When a call is made to open the file, the share mode not only means that the caller wants to restrict every call
'       afterwards, but also every call before as well, which means that the caller will fail if any calls before it
'       already obtained a conflict right.
'   For example: if this call succeeds,
'           Open(FileA, OpenMode.Write, ShareMode.Read)
'       Although it is sharing FileA for reading, if the 2nd call is
'           Open(FileA, OpenMode.Read, ShareMode.Read)
'       the 2nd call will fail since it wants to restrict everybody else to read only, but 1st caller has already obtained
'       write access.
'   So the default behavior is fine since novice Mort can't run into trouble using it.
'
' - All IO functions involving ShowUI have dependency on Windows Shell and sometimes have different behavior.
' - CopyDirectory will attempt to copy all the files in the directory. If there are files or sub-directories
'       that cause exception, CopyDirectory will not stop, since that will leave the result in unknown state.
'       Instead, an exception will be thrown at the end containing a list of exception files in Data property.
' - MoveDirectory behaves the same so MoveDirectory is not equal to calling CopyDirectory and DeleteDirectory.
' - Overwrite in directory case means overwrite sub files. Sub directories will always be merged.
'
' - 2004/08/09: Including the Overwrite option and ShowUI in one method is confusing
'       since there are cases Shell methods will ask questions, even with NOCONFIRMATION flag on.
'       We made changes to separate methods containing Overwrite and ShowUI. UE should notice this.

' Shell behavior in exception cases:
'   - Copy / Move File
'       . Existing target:
'           Overwrite = True:   Overwrite target.
'           Overwrite = False:  Dialog  Yes:    Overwrite target.
'                                       No:     Error code 7. ERROR_ARENA_TRASHED
'       . Existing target and Read-Only (Framework will throw).
'           Always ask. No: Error code 7. ERROR_ARENA_TRASHED
'       . OS access denied: Error code 1223. ERROR_CANCELLED
'   - Copy / Move Directory Existing target:
'       . Has an existing file:
'               Overwrite = True:   Overwrite file.
'               Overwrite = False:  Dialog  Yes / Yes to all :      Overwrite target.
'                                           No:                     Leave and copy the rest.
'                                           Cancel:                 Error code 2. ERROR_FILE_NOT_FOUND.
'       . Has an existing file and Read-Only (Framework will throw).
'               Behave as when Overwrite = False.
'       . File in source same name with directory in target:
'           * Copy: Error code 1223 ERROR_CANCELLED.
'           * Move:     Overwrite = True:   Error code 183. ERROR_ALREADY_EXISTS.
'                       Overwrite = False:  Ask question    Yes:    Error code 183.
'                                                           Cancel: Error code 2.
'       . Directory in source same name with file in target:
'           Error code 183 in all cases.
'
' NOTE: Some different behavior when deleting files / directories.
' ShowUI        RecycleBin          Normal file.                Read-only file.
'   F               F                   Gone                        Exception.              *
'   T               F                   Question + UI + Gone        Question + UI + Gone
'   F               T                   Bin                         Question + Bin          *
'   T               T                   Question + UI + Bin         Question + UI + Bin
