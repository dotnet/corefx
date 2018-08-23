' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Runtime.InteropServices
Imports Xunit
' Do not Imports System.IO


Namespace Microsoft.VisualBasic.Tests
    Public NotInheritable Class FileIOTests
        Sub New()
            While (Not System.Diagnostics.Debugger.IsAttached)
                System.Threading.Thread.Sleep(1000)
            End While 'End While
            Dim x As Integer = 1
        End Sub
        ReadOnly sourceData() As Char = {"a"c, "A"c, "b"c}
        ReadOnly destData() As Char = {"x"c, "X"c, "y"c}
        Private Function CreateTestFile(TestBase As FileIOTestBase, Optional TestFileNameWithPath As String = "") As String
            If TestFileNameWithPath.Length = 0 Then
                TestFileNameWithPath = TestBase.GetTestFilePath()
            End If

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(TestFileNameWithPath))
                sourceStream.Write(sourceData, 0, sourceData.Length)
            End Using

            Return TestFileNameWithPath
        End Function

        <Fact>
        Public Shared Sub CombineBadPathTest_BaseDirectory_RelativePath()
            Assert.Throws(Of ArgumentNullException)(Function() FileSystem.CombinePath("", "Test2"))
            Assert.Equal(FileSystem.CombinePath("C:\", ""), "C:\")
        End Sub

        <Fact>
        Public Shared Sub CombinePathTest_BaseDirectory_RelativePath()
            Assert.Equal(FileSystem.CombinePath("C:\", "Test2"), IO.Path.Combine("C:\", "Test2"))
        End Sub
        <Fact>
        Public Shared Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName()

        End Sub

        <Fact>
        Public Shared Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse()
            'While (Not System.Diagnostics.Debugger.IsAttached)
            '    System.Threading.Thread.Sleep(1000)
            'End While
        End Sub

        <Fact>
        Public Shared Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteTrue()
            'While (Not System.Diagnostics.Debugger.IsAttached)
            '    System.Threading.Thread.Sleep(1000)
            'End While
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName_OverwriteFalse()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = TestBase.GetTestFilePath()
            Dim testFileDest As String = TestBase.GetTestFilePath()

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(testFileSource))
                Using destStream As New IO.StreamWriter(IO.File.Create(testFileDest))
                    sourceStream.Write(sourceData, 0, sourceData.Length)
                    destStream.Write(destData, 0, destData.Length)
                End Using
            End Using
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyFile(testFileSource, testFileDest, False))

            ' Ensure copy didn't overwrite existing data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(sourceData.Length - 1) As Char
                stream.Read(readData, 0, sourceData.Length)
                Assert.Equal(destData, readData)
            End Using
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName_OverwriteTrue()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = TestBase.GetTestFilePath()
            Dim testFileDest As String = TestBase.GetTestFilePath()

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(testFileSource))
                Using destStream As New IO.StreamWriter(IO.File.Create(testFileDest))
                    sourceStream.Write(sourceData, 0, sourceData.Length)
                    destStream.Write(destData, 0, destData.Length)
                End Using
            End Using
            FileSystem.CopyFile(testFileSource, testFileDest, True)

            ' Ensure copy transferred written data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(sourceData.Length - 1) As Char
                stream.Read(readData, 0, sourceData.Length)
                Assert.Equal(sourceData, readData)
            End Using
            TestBase.Dispose()
        End Sub
        <Fact>
        Public Sub CopyFile_SourceFileName_DestinationFileName()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = TestBase.GetTestFilePath()
            Dim testFileDest As String = TestBase.GetTestFilePath()

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(testFileSource))
                Using destStream As New IO.StreamWriter(IO.File.Create(testFileDest))
                    sourceStream.Write(sourceData, 0, sourceData.Length)
                    destStream.Write(destData, 0, destData.Length)
                End Using
            End Using
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyFile(testFileSource, testFileDest))

            ' Ensure copy didn't overwrite existing data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(sourceData.Length - 1) As Char
                stream.Read(readData, 0, sourceData.Length)
                Assert.Equal(destData, readData)
            End Using

            ' Get a new destination nanme
            testFileDest = TestBase.GetTestFilePath()
            FileSystem.CopyFile(testFileSource, testFileDest)

            ' Ensure copy transferred written data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(sourceData.Length - 1) As Char
                stream.Read(readData, 0, sourceData.Length)
                Assert.Equal(sourceData, readData)
            End Using

            TestBase.Dispose()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub CopyFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub CopyFile_SourceFileName_DestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Shared Sub CreateDirectory_Directory()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory()
            Dim FullPathToNewDirectory As String = IO.Path.Combine(TestDirectory, "NewDirectory")
            Assert.False(IO.Directory.Exists(FullPathToNewDirectory))
            FileSystem.CreateDirectory(FullPathToNewDirectory)
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Shared Sub CurrentDirectoryGet()
            Dim CurrentDirectory As String = IO.Directory.GetCurrentDirectory()
            Assert.Equal(FileSystem.CurrentDirectory, CurrentDirectory)
        End Sub

        <Fact>
        Public Shared Sub CurrentDirectorySet()
            Dim SavedCurrentDirectory As String = IO.Directory.GetCurrentDirectory()
            Dim TestBase As New FileIOTestBase
            FileSystem.CurrentDirectory = TestBase.TestDirectory
            Assert.Equal(FileSystem.CurrentDirectory, TestBase.TestDirectory)
            FileSystem.CurrentDirectory = SavedCurrentDirectory
            Assert.Equal(FileSystem.CurrentDirectory, SavedCurrentDirectory)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub DeleteDirectory_Directory_DeleteAllContents()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory()
            Dim FullPathToNewDirectory As String = IO.Path.Combine(TestDirectory, "NewDirectory")
            IO.Directory.CreateDirectory(FullPathToNewDirectory)
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            Dim testFileSource As String = CreateTestFile(TestBase, IO.Path.Combine(FullPathToNewDirectory, "TestFile"))
            Assert.True(IO.File.Exists(testFileSource))
            FileSystem.DeleteDirectory(FullPathToNewDirectory, DeleteDirectoryOption.DeleteAllContents)
            Assert.False(IO.Directory.Exists(FullPathToNewDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub DeleteDirectory_Directory_ThrowIfDirectoryNonEmpty()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory()
            Dim FullPathToNewDirectory As String = IO.Path.Combine(TestDirectory, "NewDirectory")
            FileSystem.CreateDirectory(FullPathToNewDirectory)
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            Dim testFileSource As String = CreateTestFile(TestBase, IO.Path.Combine(FullPathToNewDirectory, "TestFile"))

            Assert.True(IO.File.Exists(testFileSource))
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.DeleteDirectory(FullPathToNewDirectory, DeleteDirectoryOption.ThrowIfDirectoryNonEmpty))
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            Assert.True(IO.File.Exists(testFileSource))
            TestBase.Dispose()
        End Sub
        <Fact(Skip:="Not Implemented")>
        Public Shared Sub DeleteDirectory_Directory_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub DeleteDirectory_Directory_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DeleteFile_File()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = CreateTestFile(TestBase)

            Assert.True(IO.File.Exists(testFileSource))
            FileSystem.DeleteFile(testFileSource)
            Assert.False(IO.File.Exists(testFileSource))
            TestBase.Dispose()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub DeleteFile_File_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub DeleteFile_File_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Shared Sub DirectoryExists_directory()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory()
            Assert.True(FileSystem.DirectoryExists(TestDirectory))
            Assert.False(FileSystem.DirectoryExists(IO.Path.Combine(TestDirectory, "NewDirectory")))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub FileExists_File()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = CreateTestFile(TestBase)
            Assert.True(FileSystem.FileExists(testFileSource))
            FileSystem.FileExists(testFileSource)
            IO.File.Delete(testFileSource)
            Assert.False(FileSystem.FileExists(testFileSource))
            TestBase.Dispose()
        End Sub
        <Fact>
        Public Shared Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchOption_FileWildcards()

        End Sub

        <Fact>
        Public Shared Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchType_SearchOption()

        End Sub

        <Fact>
        Public Shared Sub GetDirectories_Directory()

        End Sub

        <Fact>
        Public Shared Sub GetDirectories_Directory_SearchOption_Wildcards()

        End Sub

        <Fact>
        Public Shared Sub GetDirectoryInfo_Directory()

        End Sub

        <Fact>
        Public Shared Sub GetDriveInfo_Drive()

        End Sub

        <Fact>
        Public Shared Sub GetFileInfo_File()

        End Sub

        <Fact>
        Public Shared Sub GetFiles_Directory()

        End Sub

        <Fact>
        Public Shared Sub GetFilesDirectory_SearchOption_Wildcards()

        End Sub

        <Fact>
        Public Shared Sub GetName_Path()

        End Sub

        <Fact>
        Public Shared Sub GetParentPath_Path()

        End Sub

        <Fact>
        Public Shared Sub GetTempFileName()

        End Sub

        <Fact>
        Public Shared Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName()

        End Sub

        <Fact>
        Public Shared Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_Overwrite()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub
        <Fact>
        Public Shared Sub MoveFile_SourceFileName_DestinationFileName_Overwrite()

        End Sub

        <Fact>
        Public Shared Sub MoveFile_SourceFileNameDestinationFileName()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub MoveFile_SourceFileNameDestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Shared Sub MoveFile_SourceFileNameDestinationFileName_UIOption_UICancelOption()

        End Sub
        <Fact>
        Public Shared Sub OpenTextFieldParser_File()

        End Sub

        Public Shared Sub OpenTextFieldParser_File_Delimiters()

        End Sub

        <Fact>
        Public Shared Sub OpenTextFieldParser_File_FieldWidths()

        End Sub
        <Fact>
        Public Shared Sub OpenTextFileReader_File()

        End Sub

        <Fact>
        Public Shared Sub OpenTextFileReader_File_Encoding()

        End Sub

        <Fact>
        Public Shared Sub OpenTextFileWriter_File_Append_Encoding()

        End Sub

        <Fact>
        Public Shared Sub OpenTextFileWriter_FileAppend()

        End Sub
        <Fact>
        Public Shared Sub ReadAllBytes_File()

        End Sub

        <Fact>
        Public Shared Sub ReadAllText_File()

        End Sub

        <Fact>
        Public Shared Sub ReadAllText_File_Encoding()

        End Sub

        <Fact>
        Public Shared Sub RenameDirectory_Directory_NewName()
        End Sub

        <Fact>
        Public Shared Sub RenameFile_File_NewName()

        End Sub

        <Fact>
        Public Shared Sub WriteAllBytes_FileData_Append()

        End Sub

        <Fact>
        Public Shared Sub WriteAllText_File_Text_Append()

        End Sub

        <Fact>
        Public Shared Sub WriteAllText_File_Text_Append_Encoding()

        End Sub
    End Class
End Namespace
