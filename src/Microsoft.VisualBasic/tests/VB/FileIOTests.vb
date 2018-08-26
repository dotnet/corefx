' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Runtime.InteropServices
Imports Xunit
' Do not Imports System.IO

Namespace Microsoft.VisualBasic.Tests
    Public NotInheritable Class FileIOTests
        ReadOnly DestData() As Char = {"x"c, "X"c, "y"c}

        ReadOnly SourceData() As Char = {"a"c, "A"c, "b"c}
        ''' <summary>
        ''' All "Public" tests are Named for the FileIO function they test followed by _ParameterName for each Parameter and if there are options
        ''' they are separated into additional test and the Option Value is the last  part of the name.
        ''' For example CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse tests CopyDirectory with 3 arguments
        ''' SourceDirectoryName, DestinationDirectoryName and Overwrite and the value of Overwrite being tested is False
        ''' </summary>

        Sub New()
        End Sub

        Private Function CreateTestFile(TestBase As FileIOTestBase, ByVal TestData() As Char, Optional TestFileNameWithPath As String = "") As String
            If TestFileNameWithPath.Length = 0 Then
                TestFileNameWithPath = TestBase.GetTestFilePath()
            Else
                Assert.True(IO.Path.IsPathRooted(TestFileNameWithPath))
                Assert.True(TestFileNameWithPath.StartsWith(TestBase.TestDirectory))
            End If
            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(TestFileNameWithPath))
                sourceStream.Write(TestData, 0, TestData.Length)
            End Using

            Return TestFileNameWithPath
        End Function

        <Fact>
        Public Sub CombinePathTest_BadBaseDirectory_RelativePath()
            Assert.Throws(Of ArgumentNullException)(Function() FileSystem.CombinePath("", "Test2"))
            Assert.Equal(FileSystem.CombinePath("C:\", ""), "C:\")
        End Sub

        <Fact>
        Public Sub CombinePathTest_BaseDirectory_RelativePath()
            Assert.Equal(FileSystem.CombinePath("C:\", "Test2"), IO.Path.Combine("C:\", "Test2"))
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory)
            Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure copy transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            IO.Directory.Delete(FullPathToTargetDirectory, recursive:=True)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToTargetDirectory, $"NewFile0"))
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False)
            Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure copy transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            IO.Directory.Delete(FullPathToTargetDirectory, recursive:=True)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            Dim NewFileNameWithPath As String = IO.Path.Combine(FullPathToTargetDirectory, $"NewFile0")
            CreateTestFile(TestBase, DestData, NewFileNameWithPath)
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False))
            Assert.Equal(IO.Directory.GetFiles(FullPathToTargetDirectory).Count, IO.Directory.GetFiles(FullPathToSourceDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                If CurrentFile.EndsWith("0") Then
                    ' Make sure file 0 is unchanged with DestDate
                    Using stream As New IO.StreamReader(CurrentFile)
                        Dim ReadData(DestData.Length - 1) As Char
                        stream.Read(ReadData, 0, DestData.Length)
                        Assert.Equal(DestData, ReadData)
                    End Using
                Else
                    ' Ensure file 1 - 5 transferred SourData
                    Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                        Dim ReadData(SourceData.Length - 1) As Char
                        stream.Read(ReadData, 0, SourceData.Length)
                        Assert.Equal(SourceData, ReadData)
                    End Using
                End If
            Next
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteTrue()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=True)
            Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure copy transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            TestBase.Dispose()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = TestBase.GetTestFilePath()
            Dim testFileDest As String = TestBase.GetTestFilePath()

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(testFileSource))
                Using destStream As New IO.StreamWriter(IO.File.Create(testFileDest))
                    sourceStream.Write(SourceData, 0, SourceData.Length)
                    destStream.Write(DestData, 0, DestData.Length)
                End Using
            End Using
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyFile(testFileSource, testFileDest))

            ' Ensure copy didn't overwrite existing data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(SourceData.Length - 1) As Char
                stream.Read(readData, 0, SourceData.Length)
                Assert.Equal(DestData, readData)
            End Using

            ' Get a new destination nanme
            testFileDest = TestBase.GetTestFilePath()
            FileSystem.CopyFile(testFileSource, testFileDest)

            ' Ensure copy transferred written data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(SourceData.Length - 1) As Char
                stream.Read(readData, 0, SourceData.Length)
                Assert.Equal(SourceData, readData)
            End Using

            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName_OverwriteFalse()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = TestBase.GetTestFilePath()
            Dim testFileDest As String = TestBase.GetTestFilePath()

            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(testFileSource))
                Using destStream As New IO.StreamWriter(IO.File.Create(testFileDest))
                    sourceStream.Write(SourceData, 0, SourceData.Length)
                    destStream.Write(DestData, 0, DestData.Length)
                End Using
            End Using
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyFile(testFileSource, testFileDest, overwrite:=False))

            ' Ensure copy didn't overwrite existing data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(SourceData.Length - 1) As Char
                stream.Read(readData, 0, SourceData.Length)
                Assert.Equal(DestData, readData)
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
                    sourceStream.Write(SourceData, 0, SourceData.Length)
                    destStream.Write(DestData, 0, DestData.Length)
                End Using
            End Using
            FileSystem.CopyFile(testFileSource, testFileDest, overwrite:=True)

            ' Ensure copy transferred written data
            Using stream As New IO.StreamReader(IO.File.OpenRead(testFileDest))
                Dim readData(SourceData.Length - 1) As Char
                stream.Read(readData, 0, SourceData.Length)
                Assert.Equal(SourceData, readData)
            End Using
            TestBase.Dispose()
        End Sub
        <Fact(Skip:="Not Implemented")>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CreateDirectory_Directory()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToNewDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "NewDirectory")
            Assert.False(IO.Directory.Exists(FullPathToNewDirectory))
            FileSystem.CreateDirectory(FullPathToNewDirectory)
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub CurrentDirectoryGet()
            Dim CurrentDirectory As String = IO.Directory.GetCurrentDirectory()
            Assert.Equal(FileSystem.CurrentDirectory, CurrentDirectory)
        End Sub

        <Fact>
        Public Sub CurrentDirectorySet()
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
            Dim FullPathToNewDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "NewDirectory")
            IO.Directory.CreateDirectory(FullPathToNewDirectory)
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            Dim testFileSource As String = CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToNewDirectory, "TestFile"))
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
            Dim testFileSource As String = CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToNewDirectory, "TestFile"))

            Assert.True(IO.File.Exists(testFileSource))
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.DeleteDirectory(FullPathToNewDirectory, DeleteDirectoryOption.ThrowIfDirectoryNonEmpty))
            Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            Assert.True(IO.File.Exists(testFileSource))
            TestBase.Dispose()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DeleteFile_File()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = CreateTestFile(TestBase, SourceData)

            Assert.True(IO.File.Exists(testFileSource))
            FileSystem.DeleteFile(testFileSource)
            Assert.False(IO.File.Exists(testFileSource))
            TestBase.Dispose()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteFile_File_UIOption_RecycleOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub DeleteFile_File_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DirectoryExists_directory()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory()
            Assert.True(FileSystem.DirectoryExists(TestDirectory))
            Assert.False(FileSystem.DirectoryExists(IO.Path.Combine(TestDirectory, "NewDirectory")))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub FileExists_File()
            Dim TestBase As New FileIOTestBase
            Dim testFileSource As String = CreateTestFile(TestBase, SourceData)
            Assert.True(FileSystem.FileExists(testFileSource))
            FileSystem.FileExists(testFileSource)
            IO.File.Delete(testFileSource)
            Assert.False(FileSystem.FileExists(testFileSource))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchOption_FileWildcards()

        End Sub

        <Fact>
        Public Sub FindInFiles_Directory_ContainsText_IgnoreCase_SearchType_SearchOption()

        End Sub

        <Fact>
        Public Sub GetDirectories_Directory()
            Dim TestBase As New FileIOTestBase
            Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory)
            Assert.True(DirectoryList.Count = 0)
            For i As Integer = 0 To 5
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory)
            Assert.True(DirectoryList.Count = 6)
            For i As Integer = 0 To 5
                Assert.Equal(DirectoryList(i), IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory0", $"NewSubSubDirectory"))
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory)
            Assert.True(DirectoryList.Count = 6)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetDirectories_Directory_SearchOption()
            Dim TestBase As New FileIOTestBase
            Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
            Assert.True(DirectoryList.Count = 0)
            For i As Integer = 0 To 5
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
            Assert.True(DirectoryList.Count = 6)
            For i As Integer = 0 To 5
                Assert.Equal(DirectoryList(i), IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory0", $"NewSubSubDirectory"))
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
            Assert.True(DirectoryList.Count = 6)
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories)
            Assert.True(DirectoryList.Count = 7)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetDirectories_Directory_SearchOption_Wildcards()
            Dim TestBase As New FileIOTestBase
            Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*0", "*1")
            Assert.True(DirectoryList.Count = 0)
            For i As Integer = 0 To 5
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*0", "*1")
            Assert.True(DirectoryList.Count = 2)
            For i As Integer = 0 To 1
                Assert.Equal(DirectoryList(i), IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory0", $"NewSubSubDirectory0"))
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*0")
            Assert.True(DirectoryList.Count = 1)
            DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories, "*0")
            Assert.True(DirectoryList.Count = 2)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetDirectoryInfo_Directory()
            Dim StartTime As Date = Now
            Threading.Thread.Sleep(1000)

            Dim TestBase As New FileIOTestBase
            For i As Integer = 0 To 5
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
            Next
            IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory0", $"NewSubSubDirectory"))
            Dim info As IO.DirectoryInfo = FileSystem.GetDirectoryInfo(TestBase.TestDirectory)
            Assert.True(info.Attributes = IO.FileAttributes.Directory)
            Assert.True(info.CreationTime > StartTime)
            Assert.True(info.Extension = IO.Path.GetExtension(TestBase.TestDirectory))
            Assert.True(info.FullName = TestBase.TestDirectory)
            Assert.True(info.LastAccessTime > StartTime)
            Assert.True(info.Name = IO.Path.GetFileName(TestBase.TestDirectory))
            Assert.True(TestBase.RemoveEndingSeparator(info.Parent.ToString) = TestBase.RemoveEndingSeparator(IO.Path.GetTempPath()))
            Assert.True(info.Root.Name = IO.Path.GetPathRoot(TestBase.TestDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetDriveInfo_Drive()
            Dim TestBase As New FileIOTestBase
            Dim Rootname As String = FileSystem.GetDirectoryInfo(TestBase.TestDirectory).Root.Name
            Dim Info As IO.DriveInfo = FileSystem.GetDriveInfo(Rootname)
            Assert.NotNull(Info)
            Dim DriveInfoFromSystemIO As IO.DriveInfo = Nothing

            For Each Drive As IO.DriveInfo In IO.DriveInfo.GetDrives()
                If Drive.Name = Rootname Then
                    DriveInfoFromSystemIO = Drive
                End If
            Next
            Assert.NotNull(DriveInfoFromSystemIO)

            With DriveInfoFromSystemIO
                Assert.True(Info.DriveFormat = .DriveFormat)
                Assert.True(Info.DriveType = .DriveType)
                Assert.True(Info.Name = .Name)
                Assert.True(Info.RootDirectory.Name = .RootDirectory.Name)
                Assert.True(Info.VolumeLabel = .VolumeLabel)
            End With
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetFileInfo_File()
            Dim StartTime As Date = Now
            Threading.Thread.Sleep(1000)

            Dim TestBase As New FileIOTestBase
            Dim TestFile As String = CreateTestFile(TestBase, SourceData)

            Dim FileInfoFromSystemIO As IO.FileInfo = New IO.FileInfo(TestFile)
            Assert.NotNull(FileInfoFromSystemIO)

            Dim info As IO.FileInfo = FileSystem.GetFileInfo(TestFile)
            Assert.NotNull(info)
            With FileInfoFromSystemIO
                Assert.True(info.Exists)
                Assert.True(info.Attributes = .Attributes)
                Assert.True(info.CreationTime > StartTime)
                Assert.True(info.DirectoryName = .DirectoryName)
                Assert.True(info.Extension = .Extension)
                Assert.True(info.FullName = .FullName)
                Assert.True(info.IsReadOnly = .IsReadOnly)
                Assert.True(info.LastAccessTime = .LastAccessTime)
                Assert.True(info.LastWriteTime = .LastWriteTime)
                Assert.True(info.Length = .Length)
                Assert.True(info.Name = .Name)
            End With

            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetFiles_Directory()
            Dim TestBase As New FileIOTestBase
            Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
            Assert.True(FileList.Count = 0)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}"))
            Next
            FileList = FileSystem.GetFiles(TestBase.TestDirectory)
            Assert.True(FileList.Count = 6)
            For i As Integer = 0 To 5
                Assert.Equal(FileList(i), IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}"))
            Next
            IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory"))
            CreateTestFile(TestBase, SourceData, IO.Path.Combine(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory", $"NewFile")))
            FileList = FileSystem.GetFiles(TestBase.TestDirectory)
            Assert.True(FileList.Count = 6)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetFilesDirectory_SearchOption()
            Dim TestBase As New FileIOTestBase
            Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
            Assert.True(FileList.Count = 0)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}"))
            Next
            FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
            Assert.True(FileList.Count = 6)
            For i As Integer = 0 To 5
                Assert.Equal(FileList(i), IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}"))
            Next
            Dim NewSubDirectoryPath As String = IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory")
            IO.Directory.CreateDirectory(NewSubDirectoryPath)
            CreateTestFile(TestBase, SourceData, IO.Path.Combine(NewSubDirectoryPath, $"NewFile"))
            FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories)
            Assert.True(FileList.Count = 7)
            For i As Integer = 0 To 6
                Assert.True(IO.File.Exists(FileList(i)))
            Next
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetFilesDirectory_SearchOption_Wildcards()
            Dim TestBase As New FileIOTestBase
            Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
            Assert.True(FileList.Count = 0)
            Dim TestFileList As New List(Of String)
            For i As Integer = 0 To 5
                TestFileList.Add(IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}{If(i Mod 2 = 0, ".vb", ".cs")}"))
                CreateTestFile(TestBase, SourceData, TestFileList(i))
            Next
            FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*.vb")
            Assert.True(FileList.Count = 3)
            For i As Integer = 0 To 2
                Assert.Equal(FileList(i), TestFileList(i * 2))
            Next
            Dim NewSubDirectoryPath As String = IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory")
            IO.Directory.CreateDirectory(NewSubDirectoryPath)
            TestFileList.Add(CreateTestFile(TestBase, SourceData, IO.Path.Combine(NewSubDirectoryPath, $"NewFile.cs")))
            FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories, "*.cs")
            Assert.True(FileList.Count = 4)
            Assert.True(IO.File.Exists(FileList.Last))
            Assert.Equal(FileList.Last, TestFileList.Last)
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetName_Path()
            Dim TestBase As New FileIOTestBase
            Assert.Equal(FileSystem.GetName(TestBase.TestDirectory), IO.Path.GetFileName(TestBase.TestDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetParentPath_Path()
            Dim TestBase As New FileIOTestBase
            Dim TestDirectory As String = TestBase.TestDirectory
            Assert.Equal(FileSystem.GetParentPath(TestDirectory), IO.Path.GetDirectoryName(TestDirectory.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub GetTempFileName()
            Dim TempFile As String = FileSystem.GetTempFileName
            Assert.True(IO.File.Exists(TempFile))
            Assert.True((New IO.FileInfo(TempFile)).Length = 0)
            IO.File.Delete(TempFile)
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveDirectory_Sourc_eDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveDirectory_Source_DirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory)
            Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure move transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            IO.Directory.Move(FullPathToTargetDirectory, FullPathToSourceDirectory)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToTargetDirectory, $"NewFile0"))
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False)
            Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure move transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            IO.Directory.Move(FullPathToTargetDirectory, FullPathToSourceDirectory)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            Dim NewTestFileNameWithPath As String = IO.Path.Combine(FullPathToTargetDirectory, $"NewFile0")
            CreateTestFile(TestBase, DestData, NewTestFileNameWithPath)
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False))
            ' We couldn't move one file
            Assert.Equal(1, IO.Directory.GetFiles(FullPathToSourceDirectory).Count)
            ' Ensure the file left has correct data
            Using stream As New IO.StreamReader(IO.File.OpenRead(IO.Path.Combine(FullPathToSourceDirectory, $"NewFile0")))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                If CurrentFile.EndsWith("0") Then
                    ' Make sure file 0 is unchanged with DestDate
                    Using stream As New IO.StreamReader(CurrentFile)
                        Dim ReadData(DestData.Length - 1) As Char
                        stream.Read(ReadData, 0, DestData.Length)
                        Assert.Equal(DestData, ReadData)
                    End Using
                Else
                    ' Ensure file 1 - 5 transferred SourData
                    Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                        Dim ReadData(SourceData.Length - 1) As Char
                        stream.Read(ReadData, 0, SourceData.Length)
                        Assert.Equal(SourceData, ReadData)
                    End Using
                End If
            Next
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteTrue()
            Dim TestBase As New FileIOTestBase
            Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
            Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
            IO.Directory.CreateDirectory(FullPathToSourceDirectory)
            IO.Directory.CreateDirectory(FullPathToTargetDirectory)
            For i As Integer = 0 To 5
                CreateTestFile(TestBase, SourceData, IO.Path.Combine(FullPathToSourceDirectory, $"NewFile{i}"))
            Next
            FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=True)
            Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
            Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
            For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                ' Ensure copy transferred written data
                Using stream As New IO.StreamReader(IO.File.OpenRead(CurrentFile))
                    Dim ReadData(SourceData.Length - 1) As Char
                    stream.Read(ReadData, 0, SourceData.Length)
                    Assert.Equal(SourceData, ReadData)
                End Using
            Next
            TestBase.Dispose()
        End Sub
        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName()
            Dim TestBase As New FileIOTestBase
            Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
            Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
            FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath)
            Assert.False(IO.File.Exists(SourceFileNameWithPath))
            Assert.True(IO.File.Exists(DestinationFileNameWithPath))
            Using stream As New IO.StreamReader(IO.File.OpenRead(DestinationFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            CreateTestFile(TestBase, DestData, SourceFileNameWithPath)
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveFile(sourceFileName:=DestinationFileNameWithPath, destinationFileName:=SourceFileNameWithPath))
            ' Make sure we did not override existing file
            Using stream As New IO.StreamReader(IO.File.OpenRead(DestinationFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName_OverwriteFalse()
            Dim TestBase As New FileIOTestBase
            Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
            Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
            FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath, overwrite:=False)
            Assert.False(IO.File.Exists(SourceFileNameWithPath))
            Assert.True(IO.File.Exists(DestinationFileNameWithPath))
            Using stream As New IO.StreamReader(IO.File.OpenRead(DestinationFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            CreateTestFile(TestBase, DestData, SourceFileNameWithPath)
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveFile(sourceFileName:=DestinationFileNameWithPath, destinationFileName:=SourceFileNameWithPath, overwrite:=False))
            ' Make sure SourceFile has not chnaged
            Using stream As New IO.StreamReader(IO.File.OpenRead(SourceFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(DestData, ReadData)
            End Using
            Assert.True(IO.File.Exists(DestinationFileNameWithPath))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName_OverwriteTrue()
            Dim TestBase As New FileIOTestBase
            Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
            Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
            FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath, overwrite:=True)
            Assert.False(IO.File.Exists(SourceFileNameWithPath))
            Assert.True(IO.File.Exists(DestinationFileNameWithPath))
            Using stream As New IO.StreamReader(IO.File.OpenRead(DestinationFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            CreateTestFile(TestBase, DestData, SourceFileNameWithPath)
            FileSystem.MoveFile(sourceFileName:=DestinationFileNameWithPath, destinationFileName:=SourceFileNameWithPath, overwrite:=True)
            Assert.True(IO.File.Exists(SourceFileNameWithPath))
            Assert.False(IO.File.Exists(DestinationFileNameWithPath))
            Using stream As New IO.StreamReader(IO.File.OpenRead(SourceFileNameWithPath))
                Dim ReadData(SourceData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Assert.Equal(SourceData, ReadData)
            End Using
            TestBase.Dispose()
        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <Fact(Skip:="Not Implemented")>
        Public Sub MoveFile_SourceFileNameDestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub OpenTextFieldParser_File()
            While (Not System.Diagnostics.Debugger.IsAttached)
                System.Threading.Thread.Sleep(1000)
            End While

            Dim TestBase As New FileIOTestBase
            Dim T As New FileIO.TextFieldParser(TestBase.TestDirectory)
            TestBase.Dispose()
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
            Dim TestBase As New FileIOTestBase
            ''' <exception cref="IO.FileNotFoundException">If directory does not point to an existing directory.</exception>
            Assert.Throws(Of IO.DirectoryNotFoundException)(Sub() FileSystem.RenameDirectory(IO.Path.Combine(TestBase.TestDirectory, "DoesNotExistDirectory"), "NewDirectory"))
            Dim OrigDirectoryWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "OriginalDirectory")
            IO.Directory.CreateDirectory(OrigDirectoryWithPath)
            ''' <exception cref="System.ArgumentException">If newName is Nothing or Empty String.</exception>
            Assert.Throws(Of ArgumentNullException)(Sub() FileSystem.RenameDirectory(OrigDirectoryWithPath, ""))
            Dim DirectoryNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "DoesNotExist")
            ''' <exception cref="System.ArgumentException">If contains path information.</exception>
            Assert.Throws(Of ArgumentException)(Sub() FileSystem.RenameDirectory(OrigDirectoryWithPath, DirectoryNameWithPath))
            FileSystem.RenameDirectory(OrigDirectoryWithPath, "NewFDirectory")
            Dim NewFDirectoryPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewFDirectory")
            Assert.True(IO.Directory.Exists(NewFDirectoryPath))
            Assert.False(IO.Directory.Exists(OrigDirectoryWithPath))
            ''' <exception cref="IO.IOException">If directory points to a root directory or if there's an existing directory or an existing file with the same name.</exception>
            IO.Directory.CreateDirectory(OrigDirectoryWithPath)
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.RenameDirectory(NewFDirectoryPath, "OriginalDirectory"))
            TestBase.Dispose()
        End Sub

        <Fact>
        Public Sub RenameFile_File_NewName()
            Dim TestBase As New FileIOTestBase
            ''' <exception cref="IO.FileNotFoundException">If file does not point to an existing file.</exception>
            Assert.Throws(Of IO.FileNotFoundException)(Sub() FileSystem.RenameFile(IO.Path.Combine(TestBase.TestDirectory, "DoesNotExistFile"), "NewFile"))
            Dim OrigFileWithPath As String = CreateTestFile(TestBase, SourceData)
            Dim ExistingFileWithPath As String = CreateTestFile(TestBase, DestData)
            ''' <exception cref="System.ArgumentException">If newName is Nothing or Empty String.</exception>
            Assert.Throws(Of ArgumentNullException)(Sub() FileSystem.RenameFile(OrigFileWithPath, ""))
            ''' <exception cref="System.ArgumentException">If contains path information.</exception>
            Assert.Throws(Of ArgumentException)(Sub() FileSystem.RenameFile(OrigFileWithPath, ExistingFileWithPath))
            FileSystem.RenameFile(OrigFileWithPath, "NewFile")
            Dim NewFileWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewFile")
            Assert.True(IO.File.Exists(NewFileWithPath))
            Assert.False(IO.File.Exists(OrigFileWithPath))
            ''' <exception cref="IO.IOException">If there's an existing directory or an existing file with the same name.</exception>
            Assert.Throws(Of IO.IOException)(Sub() FileSystem.RenameFile(NewFileWithPath, "NewFile"))
            TestBase.Dispose()
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
