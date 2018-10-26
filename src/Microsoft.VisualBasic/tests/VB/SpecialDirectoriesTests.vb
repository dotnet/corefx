' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System.Environment
Imports System.Security
Imports Microsoft.VisualBasic.FileIO
Imports Xunit
Namespace Microsoft.VisualBasic.Tests.VB
    Public NotInheritable Class SpecialDirectoriesTests
        Private Shared ReadOnly Separators() As Char = {
            IO.Path.DirectorySeparatorChar,
            IO.Path.AltDirectorySeparatorChar
            }

        Private Shared Function CreateValidFullPath(FullPath As String) As String
            For Each d As String In GetCompanyProductVersionList()
                FullPath = IO.Path.Combine(FullPath, d)
            Next
            Return FullPath
        End Function

        ''' <summary>
        ''' If a path does not exist, one is created in the following format
        ''' C:\Documents and Settings\[UserName]\Application Data\[CompanyName]\[ProductName]\ProductVersion
        ''' The first function separates applications by CompanyName, ProductName, ProductVersion.
        ''' The only catch is that CompanyName and/or ProductName has to be specified in the AssemblyInfo.vb file,
        ''' otherwise the name of the assembly will be used instead (which still has a level of separation).
        ''' This function must be kept in sync with the one in coreFx\src\Microsoft.VisualBasic\src\Microsoft\VisualBasic\FileIO\SpecialDirectories.vb
        ''' </summary>
        ''' <returns>[CompanyName]\[ProductName]\ProductVersion or Assembly.Name </returns>
        Private Shared Function GetCompanyProductVersionList() As List(Of String)
            Dim CurrentProcess As Process = Process.GetCurrentProcess
            Dim PathList As New List(Of String)
            Dim CompanyName As String = MakeValidFileName(CurrentProcess.MainModule?.FileVersionInfo?.CompanyName)
            If CompanyName <> "" Then
                PathList.Add(CompanyName)
            End If
            Dim ProductName As String = MakeValidFileName(CurrentProcess.MainModule?.FileVersionInfo?.ProductName)
            If ProductName <> "" Then
                PathList.Add(ProductName)
            End If
            If PathList.Count = 0 Then
                Try
                    Dim CallingAssembly As Reflection.Assembly = Reflection.Assembly.GetCallingAssembly
                    Dim CallingAssemblyName As String = ""
                    Try
                        CallingAssemblyName = CallingAssembly.GetName().Name
                        If CallingAssemblyName = "" Then
                            Return PathList
                        End If
                        PathList.Add(MakeValidFileName(CallingAssemblyName))
                    Catch ex As SecurityException
                        Dim CallingAssemblyFullName As String = CallingAssembly.FullName
                        If CallingAssemblyFullName = "" Then
                            Return PathList
                        End If
                        PathList.Add(MakeValidFileName(GetTitleFromAssemblyFullName(CallingAssemblyFullName)))
                    End Try
                    Return PathList
                Catch
                End Try
            End If
            Dim Version As String = ExtractBuildNumber(CurrentProcess.MainModule.FileVersionInfo.ProductVersion)
            If Version = "" Then
                Version = "0.0.0.0"
            End If
            PathList.Add(Version)
            Return PathList
        End Function

        ''' <summary>
        ''' We just want numbers and "."
        ''' </summary>
        ''' <param name="productVersion"></param>
        ''' <returns></returns>
        Private Shared Function ExtractBuildNumber(productVersion As String) As String
            Dim Version As String = ""
            For Each c As Char In productVersion
                If IsNumeric(c) OrElse c = "." Then
                    Version &= c
                Else
                    Exit For
                End If
            Next
            Return Version
        End Function

        ''' <summary>
        ''' This takes a full assembly name which includes version and other information and extracts just the name upto the comma
        ''' </summary>
        ''' <param name="AssemblyFullName"></param>
        ''' <returns></returns>
        Private Shared Function GetTitleFromAssemblyFullName(AssemblyFullName As String) As String
            'Find the text up to the first comma. Note, this fails if the assembly has a comma in its name
            Dim FirstCommaLocation As Integer = AssemblyFullName.IndexOf(","c)
            If FirstCommaLocation >= 0 Then
                Return AssemblyFullName.Substring(0, FirstCommaLocation)
            End If
            'The name is not in the format we're expecting so return an empty string
            Return ""
        End Function

        ''' <summary>
        ''' Remove any OS specific invalid characters, then trim whitespace, then remove any leading "." because they hide the directory on Unix and don't
        ''' need them on other Os's, lastly trim remaining whitespace. Specifically deal with names ". Net Foundation" which become "Net Foundation"
        ''' </summary>
        ''' <param name="InputName"></param>
        ''' <returns>A valid directory name on hosted OS</returns>
        Private Shared Function MakeValidFileName(InputName As String) As String
            If InputName = "" Then
                Return ""
            End If
            Dim invalidFileChars() As Char = IO.Path.GetInvalidFileNameChars()
            For Each c As Char In invalidFileChars
                InputName = InputName.Replace(c.ToString(), "")
            Next c
            Return InputName.Trim.TrimStart("."c).TrimStart
        End Function

        <Fact>
        Public Shared Sub AllUsersApplicationDataFolderTest()
            Dim AllUsersApplicationDataRoot As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)

            If AllUsersApplicationDataRoot = "" Then
                Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.AllUsersApplicationData)
            Else
                If GetCompanyProductVersionList.Any Then
                    Assert.Equal(CreateValidFullPath(AllUsersApplicationDataRoot.TrimEnd(Separators)).TrimEnd(Separators), SpecialDirectories.AllUsersApplicationData)
                Else
                    Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.AllUsersApplicationData)
                End If
            End If
        End Sub

        <Fact>
        Public Shared Sub CurrentUserApplicationDataFolderTest()
            Dim Env_ApplicationData As String = Environment.GetFolderPath(SpecialFolder.ApplicationData, [option]:=SpecialFolderOption.Create).Trim.TrimEnd(Separators).Trim

            If PlatformDetection.IsWindowsNanoServer OrElse Env_ApplicationData = "" Then
                Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.CurrentUserApplicationData)
            ElseIf GetCompanyProductVersionList.Any Then
                Assert.Equal(expected:=CreateValidFullPath(Env_ApplicationData), actual:=SpecialDirectories.CurrentUserApplicationData)
            Else
                Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.CurrentUserApplicationData)
            End If
        End Sub

        <Fact>
        Public Shared Sub DesktopFolderTest()
            Dim Env_Desktop As String = Environment.GetFolderPath(SpecialFolder.Desktop).Trim.TrimEnd(Separators).TrimEnd
            If Env_Desktop.Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.Desktop)
            Else
                Dim FileIO_Desktop As String = SpecialDirectories.Desktop.TrimEnd(Separators)
                Assert.True(Env_Desktop = FileIO_Desktop, $"{Env_Desktop} <> {FileIO_Desktop}")
            End If
        End Sub

        <Fact>
        Public Shared Sub MyDocumentsFolderTest()
            If PlatformDetection.IsWindowsNanoServer Then
                Exit Sub
            End If

            If Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.MyDocuments)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.Personal).TrimEnd(Separators), SpecialDirectories.MyDocuments.TrimEnd(Separators))
            End If
        End Sub

        <PlatformSpecific(TestPlatforms.Windows), ConditionalFact(GetType(PlatformDetection), NameOf(PlatformDetection.IsWindowsNanoServer))>
        Public Shared Sub MyDocumentsFolderTestForNano()
            Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.MyDocuments)
        End Sub


        <Fact>
        Public Shared Sub MyMusicFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.MyMusic)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.MyMusic).TrimEnd(Separators), SpecialDirectories.MyMusic.TrimEnd(Separators))
            End If
        End Sub

        <Fact>
        Public Shared Sub MyPicturesFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.MyPictures)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.MyPictures).TrimEnd(Separators), SpecialDirectories.MyPictures.TrimEnd(Separators))
            End If
        End Sub

        <Fact>
        Public Shared Sub ProgramFilesFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.ProgramFiles)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.ProgramFiles).TrimEnd(Separators), SpecialDirectories.ProgramFiles.TrimEnd(Separators))
            End If
        End Sub

        <Fact>
        Public Shared Sub ProgramsFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.Programs).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.Programs)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.Programs).TrimEnd(Separators), SpecialDirectories.Programs.TrimEnd(Separators))
            End If
        End Sub

        <Fact>
        Public Shared Sub TempFolderTest()
            Assert.Equal(IO.Path.GetTempPath.TrimEnd(Separators), SpecialDirectories.Temp.TrimEnd(Separators))
        End Sub

    End Class
End Namespace
