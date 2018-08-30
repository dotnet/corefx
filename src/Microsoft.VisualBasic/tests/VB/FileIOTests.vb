' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports Xunit
' Do not Imports System.IO
#Const ManualTestsEnabled = False
Namespace Microsoft.VisualBasic.Tests
    Public NotInheritable Class FileIOTests
        Public Shared ReadOnly Property ManualTestsEnabled() As Boolean
            Get
                Return Not String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MANUAL_TESTS"))
            End Get
        End Property

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

        Private Function CreateTestFile(TestBase As FileIOTestBase, ByVal TestData() As Char, <CallerMemberName> Optional memberName As String = Nothing, <CallerLineNumber> Optional lineNumber As Integer = 0) As String
            Return CreateTestFile(TestBase, TestData, "", "", memberName, lineNumber)
        End Function

        Private Function CreateTestFile(TestBase As FileIOTestBase, ByVal TestData() As Char, TestFileName As String, <CallerMemberName> Optional memberName As String = Nothing, <CallerLineNumber> Optional lineNumber As Integer = 0) As String
            Return CreateTestFile(TestBase, TestData, "", TestFileName, memberName, lineNumber)
        End Function

        ''' <summary>
        ''' Create a new file with TestData
        ''' </summary>
        ''' <param name="TestBase">Opbject to manage temporary Files</param>
        ''' <param name="TestData">Data to be written to file</param>
        ''' <param name="PathFromBase">Optional additional subdirectories that file will be created under</param>
        ''' <param name="TestFileName">Optional Filename, if none a randon one based on TestName will be created</param>
        ''' <returns>Full Path to New File</returns>
        Private Function CreateTestFile(TestBase As FileIOTestBase, ByVal TestData() As Char, PathFromBase As String, TestFileName As String, <CallerMemberName> Optional memberName As String = Nothing, <CallerLineNumber> Optional lineNumber As Integer = 0) As String
            Dim TempFileNameWithPath As String
            If TestFileName.Length = 0 Then
                TempFileNameWithPath = TestBase.GetTestFilePath(memberName:=memberName, lineNumber:=lineNumber)
            Else
                Assert.False(IO.Path.IsPathRooted(TestFileName))
                If PathFromBase.Length = 0 Then
                    TempFileNameWithPath = IO.Path.Combine(TestBase.TestDirectory, TestFileName)
                Else
                    ' If we have a Base we must have a filename
                    Assert.False(String.IsNullOrWhiteSpace(TestFileName))
                    TempFileNameWithPath = IO.Path.Combine(TestBase.TestDirectory, PathFromBase, TestFileName)
                End If
            End If
            Assert.False(IO.File.Exists(TempFileNameWithPath), $"File {TempFileNameWithPath} should not exist!")
            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(TempFileNameWithPath))
                sourceStream.Write(TestData, 0, TestData.Length)
            End Using

            Return TempFileNameWithPath
        End Function

        Private Function DirectoryListToString(DirectoryList As ReadOnlyCollection(Of String)) As String
            Dim S As New StringBuilder
            For Each d As String In DirectoryList
                S.Append($"{d} ")
            Next
            Return S.ToString
        End Function

        Private Function FileHasExpectedDate(FileNameWithPath As String, ExpectedData() As Char) As Boolean
            Using stream As New IO.StreamReader(IO.File.OpenRead(FileNameWithPath))
                Dim ReadData(ExpectedData.Length - 1) As Char
                stream.Read(ReadData, 0, SourceData.Length)
                Return ExpectedData = ReadData
            End Using
        End Function

        <Fact>
        Public Sub CombinePathTest_BadBaseDirectory_RelativePath()
            Assert.Throws(Of ArgumentNullException)(Function() FileSystem.CombinePath("", "Test2"))
        End Sub

        <Fact>
        Public Sub CombinePathTest_BaseDirectory_RelativePath()
            Using TestBase As New FileIOTestBase
                Dim TestDirInfo As New IO.DirectoryInfo(TestBase.TestDirectory)
                Dim Root As String = TestDirInfo.Root.Name
                Assert.Equal(FileSystem.CombinePath(Root, "Test2"), IO.Path.Combine(Root, "Test2"))
            End Using
        End Sub

        <Fact>
        Public Sub CombinePathTest_RootDirectory_RelativePath()
            Using TestBase As New FileIOTestBase
                Assert.Equal(FileSystem.CombinePath(TestBase.TestDirectory, ""), TestBase.TestDirectory)
                Assert.Equal(FileSystem.CombinePath(TestBase.TestDirectory, "Test"), IO.Path.Combine(TestBase.TestDirectory, "Test"))
            End Using
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory)
                Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure copy transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
                IO.Directory.Delete(FullPathToTargetDirectory, recursive:=True)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                CreateTestFile(TestBase, TestData:=SourceData, PathFromBase:="TargetDirectory", TestFileName:=$"NewFile0")
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory))
            End Using
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False)
                Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure copy transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
                IO.Directory.Delete(FullPathToTargetDirectory, recursive:=True)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                CreateTestFile(TestBase, DestData, PathFromBase:="TargetDirectory", TestFileName:=$"NewFile0")
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False))
                Assert.Equal(IO.Directory.GetFiles(FullPathToTargetDirectory).Count, IO.Directory.GetFiles(FullPathToSourceDirectory).Count)
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    If CurrentFile.EndsWith("0") Then
                        ' Make sure file 0 is unchanged with DestDate
                        Assert.True(FileHasExpectedDate(CurrentFile, DestData))
                    Else
                        ' Ensure file 1 - 5 transferred SourData
                        Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                    End If
                Next
            End Using
        End Sub

        <Fact>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteTrue()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                FileSystem.CopyDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=True)
                Assert.Equal(IO.Directory.GetFiles(FullPathToSourceDirectory).Count, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure copy transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
            End Using
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub CopyDirectory_SourceDirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName()
            Using TestBase As New FileIOTestBase
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
                Assert.True(FileHasExpectedDate(testFileDest, DestData))

                ' Get a new destination nanme
                testFileDest = TestBase.GetTestFilePath()
                FileSystem.CopyFile(testFileSource, testFileDest)

                ' Ensure copy transferred written data
                Assert.True(FileHasExpectedDate(testFileDest, SourceData))
            End Using
        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName_OverwriteFalse()
            Using TestBase As New FileIOTestBase
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
                Assert.True(FileHasExpectedDate(testFileDest, DestData))
            End Using
        End Sub

        <Fact>
        Public Sub CopyFile_FileSourceFileName_DestinationFileName_OverwriteTrue()
            Using TestBase As New FileIOTestBase
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
                Assert.True(FileHasExpectedDate(testFileDest, SourceData))
            End Using
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub CopyFile_SourceFileName_DestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub CreateDirectory_Directory()
            Using TestBase As New FileIOTestBase
                Dim FullPathToNewDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "NewDirectory")
                Assert.False(IO.Directory.Exists(FullPathToNewDirectory))
                FileSystem.CreateDirectory(FullPathToNewDirectory)
                Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
            End Using
        End Sub

        <Fact>
        Public Sub CurrentDirectoryGet()
            If Not RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
                ' Can't get current Directory on Mac before setting it.
                Dim CurrentDirectory As String = IO.Directory.GetCurrentDirectory()
                Assert.Equal(FileSystem.CurrentDirectory, CurrentDirectory)
            End If
        End Sub

        <Fact>
        Public Sub CurrentDirectorySet()
            If Not RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
                Dim SavedCurrentDirectory As String = IO.Directory.GetCurrentDirectory()
                Using TestBase As New FileIOTestBase
                    ' On OSX, the temp directory /tmp/ is a symlink to /private/tmp, so setting the current
                    ' directory to a symlinked path will result in GetCurrentDirectory returning the absolute
                    ' path that followed the symlink.
                    FileSystem.CurrentDirectory = TestBase.TestDirectory
                    Assert.Equal(TestBase.TestDirectory, FileSystem.CurrentDirectory())
                    FileSystem.CurrentDirectory = SavedCurrentDirectory
                    Assert.Equal(FileSystem.CurrentDirectory, SavedCurrentDirectory)
                End Using
            End If
        End Sub

        <Fact>
        Public Sub DeleteDirectory_Directory_DeleteAllContents()
            Using TestBase As New FileIOTestBase
                Dim FullPathToNewDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "NewDirectory")
                IO.Directory.CreateDirectory(FullPathToNewDirectory)
                Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
                Dim testFileSource As String = CreateTestFile(TestBase, SourceData, PathFromBase:="NewDirectory", TestFileName:="TestFile")
                Assert.True(IO.File.Exists(testFileSource))
                FileSystem.DeleteDirectory(FullPathToNewDirectory, DeleteDirectoryOption.DeleteAllContents)
                Assert.False(IO.Directory.Exists(FullPathToNewDirectory))
            End Using
        End Sub

        <Fact>
        Public Sub DeleteDirectory_Directory_ThrowIfDirectoryNonEmpty()
            Using TestBase As New FileIOTestBase
                Dim FullPathToNewDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "NewDirectory")
                FileSystem.CreateDirectory(FullPathToNewDirectory)
                Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
                Dim testFileSource As String = CreateTestFile(TestBase, SourceData, PathFromBase:="NewDirectory", TestFileName:="TestFile")

                Assert.True(IO.File.Exists(testFileSource))
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.DeleteDirectory(FullPathToNewDirectory, DeleteDirectoryOption.ThrowIfDirectoryNonEmpty))
                Assert.True(IO.Directory.Exists(FullPathToNewDirectory))
                Assert.True(IO.File.Exists(testFileSource))
            End Using
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub DeleteDirectory_Directory_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DeleteFile_File()
            Using TestBase As New FileIOTestBase
                Dim testFileSource As String = CreateTestFile(TestBase, SourceData)

                Assert.True(IO.File.Exists(testFileSource))
                FileSystem.DeleteFile(testFileSource)
                Assert.False(IO.File.Exists(testFileSource))
            End Using
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub DeleteFile_File_UIOption_RecycleOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub DeleteFile_File_UIOption_RecycleOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub DirectoryExists_directory()
            Using TestBase As New FileIOTestBase
                Dim TestDirectory As String = TestBase.TestDirectory()
                Assert.True(FileSystem.DirectoryExists(TestDirectory))
                Assert.False(FileSystem.DirectoryExists(IO.Path.Combine(TestDirectory, "NewDirectory")))
            End Using
        End Sub

        <Fact>
        Public Sub FileExists_File()
            Using TestBase As New FileIOTestBase
                Dim testFileSource As String = CreateTestFile(TestBase, SourceData)
                Assert.True(FileSystem.FileExists(testFileSource))
                FileSystem.FileExists(testFileSource)
                IO.File.Delete(testFileSource)
                Assert.False(FileSystem.FileExists(testFileSource))
            End Using
        End Sub

        <Fact>
        Public Sub GetDirectories_Directory()
            Using TestBase As New FileIOTestBase
                Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory)
                Assert.True(DirectoryList.Count = 0)
                For i As Integer = 0 To 5
                    IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_DirectoryNewSubDirectory{i}"))
                Next
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory)
                Assert.True(DirectoryList.Count = 6)
                For i As Integer = 0 To 5
                    Assert.True(DirectoryList.Contains(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_DirectoryNewSubDirectory{i}")))
                Next
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_DirectoryNewSubDirectory0", $"NewSubSubDirectory"))
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory)
                Assert.True(DirectoryList.Count = 6)
            End Using
        End Sub

        <Fact>
        Public Sub GetDirectories_Directory_SearchOption()
            Using TestBase As New FileIOTestBase
                Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
                Assert.True(DirectoryList.Count = 0)
                For i As Integer = 0 To 5
                    IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_Directory_SearchOptionNewSubDirectory{i}"))
                Next
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
                Assert.True(DirectoryList.Count = 6)
                For i As Integer = 0 To 5
                    Assert.True(DirectoryList.Contains(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_Directory_SearchOptionNewSubDirectory{i}")))
                Next
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"GetDirectories_Directory_SearchOptionNewSubDirectory0", $"NewSubSubDirectory"))
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
                Assert.True(DirectoryList.Count = 6)
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories)
                Assert.True(DirectoryList.Count = 7)
            End Using
        End Sub

        <Fact>
        Public Sub GetDirectories_Directory_SearchOption_Wildcards()
            Using TestBase As New FileIOTestBase
                Dim DirectoryList As ReadOnlyCollection(Of String) = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*")
                Assert.True(DirectoryList.Count = 0)
                Dim CreatedDirectories As New List(Of String)
                For i As Integer = 0 To 5
                    CreatedDirectories.Add(IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory00{i}")).Name)
                Next
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*000", "*001")
                Assert.True(DirectoryList.Count = 2, $"Search results Expected 2 Actual {DirectoryList.Count} DirectoryList = {DirectoryListToString(DirectoryList)} Created List = {DirectoryListToString(New ReadOnlyCollection(Of String)(CreatedDirectories))}")
                For i As Integer = 0 To 1
                    Dim DirectoryName As String = IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory00{i}")
                    Assert.True(DirectoryList.Contains(DirectoryName), $"{DirectoryName} Is missing from Wildcard Search")
                Next
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory000", $"NewSubSubDirectory000"))
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*000")
                Assert.True(DirectoryList.Count = 1, $"Search results Expected 1 Actual {DirectoryList.Count} {DirectoryListToString(DirectoryList)}")
                DirectoryList = FileSystem.GetDirectories(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories, "*000")
                Assert.True(DirectoryList.Count = 2, $"Search results Expected 2 Actual {DirectoryList.Count} {DirectoryListToString(DirectoryList)}")
            End Using
        End Sub

        <Fact>
        Public Sub GetDirectoryInfo_Directory()
            Using TestBase As New FileIOTestBase
                For i As Integer = 0 To 5
                    IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory{i}"))
                Next
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, $"NewSubDirectory0", $"NewSubSubDirectory"))
                Dim info As IO.DirectoryInfo = FileSystem.GetDirectoryInfo(TestBase.TestDirectory)
                Dim infoFromIO As IO.DirectoryInfo = New IO.DirectoryInfo(TestBase.TestDirectory)
                Assert.True(info.CreationTime = infoFromIO.CreationTime, $"Creation Time info ({info.CreationTime}) <> IO.DriveInfo.CreateTime {infoFromIO.CreationTime})")
                Assert.True(info.Extension = infoFromIO.Extension, $"Extension {info.Extension} <> IO.DriveInfo.Extension {infoFromIO.Extension}")
                Assert.True(info.FullName = TestBase.TestDirectory, $"Fullname {info.FullName} <> TestBase.TestDirectory {TestBase.TestDirectory}")
                Assert.True(info.LastAccessTime = infoFromIO.LastAccessTime)
                Assert.True(info.Name = infoFromIO.Name, $"Name {info.Name} Doesn't match {infoFromIO.Name}")
                Assert.True(TestBase.RemoveEndingSeparator(info.Parent.ToString) = TestBase.RemoveEndingSeparator(infoFromIO.Parent.ToString), "info.Parent<>infoFromIO.Parent")
                Assert.True(info.Root.Name = infoFromIO.Root.Name, $"info.Root.Name {info.Root.Name} <> IO.DriveInfo.Root.Name){infoFromIO.Root.Name}")
            End Using
        End Sub

        <Fact>
        Public Sub GetDriveInfo_Drive()
            ' This test is unneccessory becuase DriveInfo is just Passthrough of IO.GetDrives
        End Sub

        <Fact>
        Public Sub GetFileInfo_File()
            Using TestBase As New FileIOTestBase
                Dim TestFile As String = CreateTestFile(TestBase, SourceData)

                Dim FileInfoFromSystemIO As IO.FileInfo = New IO.FileInfo(TestFile)
                Assert.NotNull(FileInfoFromSystemIO)

                Dim info As IO.FileInfo = FileSystem.GetFileInfo(TestFile)
                Assert.NotNull(info)
                With FileInfoFromSystemIO
                    Assert.True(info.Exists)
                    Assert.True(info.Attributes = .Attributes)
                    Assert.True(info.CreationTime > Date.MinValue, "Starttime = 0")
                    Assert.True(info.DirectoryName = .DirectoryName)
                    Assert.True(info.Extension = .Extension)
                    Assert.True(info.FullName = .FullName)
                    Assert.True(info.IsReadOnly = .IsReadOnly)
                    Assert.True(info.LastAccessTime = .LastAccessTime)
                    Assert.True(info.LastWriteTime = .LastWriteTime)
                    Assert.True(info.Length = .Length)
                    Assert.True(info.Name = .Name)
                End With

            End Using
        End Sub

        <Fact>
        Public Sub GetFiles_Directory()
            Using TestBase As New FileIOTestBase
                Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
                Assert.True(FileList.Count = 0)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, TestFileName:=$"NewFile{i}")
                Next
                FileList = FileSystem.GetFiles(TestBase.TestDirectory)
                Assert.True(FileList.Count = 6)
                For i As Integer = 0 To 5
                    Assert.True(FileList.Contains(IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}")))
                Next
                IO.Directory.CreateDirectory(IO.Path.Combine(TestBase.TestDirectory, "GetFiles_DirectoryNewSubDirectory"))
                CreateTestFile(TestBase, SourceData, PathFromBase:="GetFiles_DirectoryNewSubDirectory", TestFileName:="NewFile")
                FileList = FileSystem.GetFiles(TestBase.TestDirectory)
                Assert.True(FileList.Count = 6)
            End Using
        End Sub

        <Fact>
        Public Sub GetFiles_Directory_SearchOption()
            Using TestBase As New FileIOTestBase
                Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
                Assert.True(FileList.Count = 0)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, TestFileName:=$"NewFile{i}")
                Next
                FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly)
                Assert.True(FileList.Count = 6)
                For i As Integer = 0 To 5
                    Assert.True(FileList.Contains(IO.Path.Combine(TestBase.TestDirectory, $"NewFile{i}")))
                Next
                Dim NewSubDirectoryPath As String = IO.Path.Combine(TestBase.TestDirectory, "GetFiles_Directory_SearchOptionNewSubDirectory")
                IO.Directory.CreateDirectory(NewSubDirectoryPath)
                CreateTestFile(TestBase, SourceData, TestFileName:="NewFile")
                FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories)
                Assert.True(FileList.Count = 7)
                For i As Integer = 0 To 6
                    Assert.True(IO.File.Exists(FileList(i)))
                Next
            End Using
        End Sub

        <Fact>
        Public Sub GetFiles_Directory_SearchOption_Wildcards()
            Using TestBase As New FileIOTestBase
                Dim FileList As ReadOnlyCollection(Of String) = FileSystem.GetFiles(TestBase.TestDirectory)
                Assert.True(FileList.Count = 0)
                Dim TestFileList As New List(Of String)
                For i As Integer = 0 To 5
                    TestFileList.Add(CreateTestFile(TestBase, SourceData, TestFileName:=$"NewFile{i}{If(i Mod 2 = 0, ".vb", ".cs")}"))
                Next
                FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchTopLevelOnly, "*.vb")
                Assert.True(FileList.Count = 3)
                For i As Integer = 0 To 2
                    Assert.True(TestFileList.Contains(FileList(i)))
                Next
                Dim NewSubDirectoryPath As String = IO.Path.Combine(TestBase.TestDirectory, "GetFiles_Directory_SearchOption_WildcardsNewSubDirectory")
                IO.Directory.CreateDirectory(NewSubDirectoryPath)
                TestFileList.Add(CreateTestFile(TestBase, SourceData, PathFromBase:="GetFiles_Directory_SearchOption_WildcardsNewSubDirectory", TestFileName:="NewFile.cs"))
                FileList = FileSystem.GetFiles(TestBase.TestDirectory, SearchOption.SearchAllSubDirectories, "*.cs")
                Assert.True(FileList.Contains(TestFileList.Last), "File in Subdirectory not found")
                Assert.True(FileList.Count = 4, $"4 files expected, {FileList.Count} returned from FileSystem.GetFiles")
            End Using
        End Sub

        <Fact>
        Public Sub GetName_Path()
            Using TestBase As New FileIOTestBase
                Assert.Equal(FileSystem.GetName(TestBase.TestDirectory), IO.Path.GetFileName(TestBase.TestDirectory))
            End Using
        End Sub

        <Fact>
        Public Sub GetParentPath_Path()
            Using TestBase As New FileIOTestBase
                Dim TestDirectory As String = TestBase.TestDirectory
                Assert.Equal(FileSystem.GetParentPath(TestDirectory), IO.Path.GetDirectoryName(TestDirectory.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)))
            End Using
        End Sub

        <Fact>
        Public Sub GetTempFileName()
            Dim TempFile As String = FileSystem.GetTempFileName
            Assert.True(IO.File.Exists(TempFile))
            Assert.True((New IO.FileInfo(TempFile)).Length = 0)
            IO.File.Delete(TempFile)
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub MoveDirectory_Sourc_eDirectoryName_DestinationDirectoryName_UIOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub MoveDirectory_Source_DirectoryName_DestinationDirectoryName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory)
                Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure move transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
                IO.Directory.Move(FullPathToTargetDirectory, FullPathToSourceDirectory)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                CreateTestFile(TestBase, SourceData, PathFromBase:="TargetDirectory", TestFileName:="NewFile0")
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory))
            End Using
        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteFalse()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False)
                Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure move transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
                IO.Directory.Move(FullPathToTargetDirectory, FullPathToSourceDirectory)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                Dim NewFile0WithPath As String = CreateTestFile(TestBase, DestData, PathFromBase:="TargetDirectory", TestFileName:="NewFile0")
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=False))
                Dim RemainingSourceFilesWithPath As String() = IO.Directory.GetFiles(FullPathToSourceDirectory)
                ' We couldn't move one file
                Assert.Equal(1, RemainingSourceFilesWithPath.Count)
                ' Ensure the file left has correct data
                Assert.True(FileHasExpectedDate(RemainingSourceFilesWithPath(0), SourceData))

                Dim DestinationFilesWithPath As String() = IO.Directory.GetFiles(FullPathToTargetDirectory)
                Assert.Equal(6, DestinationFilesWithPath.Count)
                For Each CurrentFile As String In DestinationFilesWithPath
                    If CurrentFile.EndsWith("0") Then
                        ' Make sure file 0 is unchanged with DestDate
                        Assert.True(FileHasExpectedDate(CurrentFile, DestData))
                    Else
                        ' Ensure file 1 - 5 transferred SourData
                        Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                    End If
                Next
            End Using
        End Sub

        <Fact>
        Public Sub MoveDirectory_SourceDirectoryName_DestinationDirectoryName_OverwriteTrue()
            Using TestBase As New FileIOTestBase
                Dim FullPathToSourceDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "SourceDirectory")
                Dim FullPathToTargetDirectory As String = IO.Path.Combine(TestBase.TestDirectory(), "TargetDirectory")
                IO.Directory.CreateDirectory(FullPathToSourceDirectory)
                IO.Directory.CreateDirectory(FullPathToTargetDirectory)
                For i As Integer = 0 To 5
                    CreateTestFile(TestBase, SourceData, PathFromBase:="SourceDirectory", TestFileName:=$"NewFile{i}")
                Next
                FileSystem.MoveDirectory(FullPathToSourceDirectory, FullPathToTargetDirectory, overwrite:=True)
                Assert.False(IO.Directory.Exists(FullPathToSourceDirectory))
                Assert.Equal(6, IO.Directory.GetFiles(FullPathToTargetDirectory).Count)
                For Each CurrentFile As String In IO.Directory.GetFiles(FullPathToTargetDirectory)
                    ' Ensure copy transferred written data
                    Assert.True(FileHasExpectedDate(CurrentFile, SourceData))
                Next
            End Using
        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName()
            Using TestBase As New FileIOTestBase
                Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
                Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
                FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath)
                Assert.False(IO.File.Exists(SourceFileNameWithPath))
                Assert.True(IO.File.Exists(DestinationFileNameWithPath))
                Assert.True(FileHasExpectedDate(DestinationFileNameWithPath, SourceData))

                SourceFileNameWithPath = DestinationFileNameWithPath
                DestinationFileNameWithPath = CreateTestFile(TestBase, DestData)
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath))
                ' Make sure we did not override existing file
                Assert.True(FileHasExpectedDate(DestinationFileNameWithPath, DestData))
                Assert.True(IO.File.Exists(SourceFileNameWithPath))
            End Using
        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName_OverwriteFalse()
            Using TestBase As New FileIOTestBase
                Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
                Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
                FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath, overwrite:=False)
                Assert.False(IO.File.Exists(SourceFileNameWithPath))
                Assert.True(IO.File.Exists(DestinationFileNameWithPath))
                Assert.True(FileHasExpectedDate(DestinationFileNameWithPath, SourceData))
                SourceFileNameWithPath = DestinationFileNameWithPath
                DestinationFileNameWithPath = CreateTestFile(TestBase, DestData)
                Assert.Throws(Of IO.IOException)(Sub() FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath, overwrite:=False))
                ' Make sure we did not override existing file
                Assert.True(FileHasExpectedDate(DestinationFileNameWithPath, DestData))
                Assert.True(IO.File.Exists(SourceFileNameWithPath))
            End Using
        End Sub

        <Fact>
        Public Sub MoveFile_SourceFileName_DestinationFileName_OverwriteTrue()
            Using TestBase As New FileIOTestBase
                Dim SourceFileNameWithPath As String = CreateTestFile(TestBase, SourceData)
                Dim DestinationFileNameWithPath As String = IO.Path.Combine(TestBase.TestDirectory, "NewName")
                FileSystem.MoveFile(SourceFileNameWithPath, DestinationFileNameWithPath, overwrite:=True)
                Assert.False(IO.File.Exists(SourceFileNameWithPath))
                Assert.True(IO.File.Exists(DestinationFileNameWithPath))
                Assert.True(FileHasExpectedDate(DestinationFileNameWithPath, SourceData))
                CreateTestFile(TestBase, DestData, TestFileName:=(New IO.FileInfo(SourceFileNameWithPath)).Name)
                FileSystem.MoveFile(sourceFileName:=DestinationFileNameWithPath, destinationFileName:=SourceFileNameWithPath, overwrite:=True)
                Assert.True(IO.File.Exists(SourceFileNameWithPath))
                Assert.False(IO.File.Exists(DestinationFileNameWithPath))
                Assert.True(FileHasExpectedDate(SourceFileNameWithPath, SourceData))
            End Using
        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub MoveFile_SourceFileName_DestinationFileName_UIOption()

        End Sub

        <ConditionalTheory(NameOf(ManualTestsEnabled))>
        Public Sub MoveFile_SourceFileNameDestinationFileName_UIOption_UICancelOption()

        End Sub

        <Fact>
        Public Sub RenameDirectory_Directory_NewName()
            Using TestBase As New FileIOTestBase
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
            End Using
        End Sub

        <Fact>
        Public Sub RenameFile_File_NewName()
            Using TestBase As New FileIOTestBase
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
            End Using
        End Sub

    End Class
End Namespace
