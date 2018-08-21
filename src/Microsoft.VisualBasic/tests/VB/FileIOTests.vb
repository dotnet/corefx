' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports Xunit

Namespace Microsoft.VisualBasic.Tests
    Public Class FileIOTests
        <Fact>
        Public Sub CombinePathTest_BaseDirectory_RelativePath()

        End Sub

        <Fact>
        Public Sub Copy_FileSource_FileName_DestinationFileName_Overwrite()

        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName()
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_Overwrite()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CopyFile_SourceFileName_DestinationFileName()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CreateDirectory_Directory()

        End Sub

        <Fact>
        Public Sub CreateDirectoryTest()
            Dim TestBase As New FileIOTestBase
        End Sub

        <Fact>
        Public Sub CurrentDirectoryTest()

        End Sub
        <Fact>
        Public Sub DeleteDirectory_Directory_DeleteDirectory_Option()

        End Sub

        <Fact>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DeleteFile_File()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteFile_File_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteFile_File_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DirectoryExists_directory()

        End Sub

        <Fact>
        Public Sub FileExists_File()

        End Sub

        <Fact>
        Public Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchOption_FileWildcards()

        End Sub

        <Fact>
        Public Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchType_SearchOption()

        End Sub

        <Fact>
        Public Sub GetDirectories_Directory()

        End Sub

        <Fact>
        Public Sub GetDirectories_Directory_SearchOption_Wildcards()

        End Sub

        <Fact>
        Public Sub GetDirectoryInfo_Directory()

        End Sub

        <Fact>
        Public Sub GetDriveInfo_Drive()

        End Sub

        <Fact>
        Public Sub GetFileInfo_File()

        End Sub

        <Fact>
        Public Sub GetFiles_Directory()

        End Sub

        <Fact>
        Public Sub GetFilesDirectory_SearchOption_Wildcards()

        End Sub

        <Fact>
        Public Sub GetName_Path()

        End Sub

        <Fact>
        Public Sub GetParentPath_Path()

        End Sub

        <Fact>
        Public Sub GetTempFileName()

        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName()

        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_Overwrite()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub
        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName_Overwrite()

        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileNameDestinationFileName()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveFile_SourceFileNameDestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveFile_SourceFileNameDestinationFileName_UIOption_UICancelOption()

        End Sub
        <Fact>
        Public Sub OpenTextFieldParser_File()

        End Sub

        Public Sub OpenTextFieldParser_File_Delimiters()

        End Sub

        <Fact>
        Public Sub OpenTextFieldParser_File_FieldWidths()

        End Sub
        <Fact>
        Public Sub OpenTextFileReader_File()

        End Sub

        <Fact>
        Public Sub OpenTextFileReader_File_Encoding()

        End Sub

        <Fact>
        Public Sub OpenTextFileWriter_File_Append_Encoding()

        End Sub

        <Fact>
        Public Sub OpenTextFileWriter_FileAppend()

        End Sub
        <Fact>
        Public Sub ReadAllBytes_File()

        End Sub

        <Fact>
        Public Sub ReadAllText_File()

        End Sub

        <Fact>
        Public Sub ReadAllText_File_Encoding()

        End Sub

        <Fact>
        Public Sub RenameDirectory_Directory_NewName()
        End Sub

        <Fact>
        Public Sub RenameFile_File_NewName()

        End Sub

        <Fact>
        Public Sub WriteAllBytes_FileData_Append()

        End Sub

        <Fact>
        Public Sub WriteAllText_File_Text_Append()

        End Sub

        <Fact>
        Public Sub WriteAllText_File_Text_Append_Encoding()

        End Sub
    End Class
End Namespace
