' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System.Environment
Imports Microsoft.VisualBasic.FileIO
Imports Xunit
Namespace Microsoft.VisualBasic.Tests.VB
    Public NotInheritable Class SpecialDirectoriesTests
        Private Shared ReadOnly Separators() As Char = {
            IO.Path.DirectorySeparatorChar,
            IO.Path.AltDirectorySeparatorChar
            }

        <Fact>
        Public Shared Sub AllUsersApplicationDataFolderTest()
            Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.AllUsersApplicationData)
        End Sub

        <Fact>
        Public Shared Sub CurrentUserApplicationDataFolderTest()
            Assert.Throws(Of PlatformNotSupportedException)(Function() SpecialDirectories.CurrentUserApplicationData)
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
