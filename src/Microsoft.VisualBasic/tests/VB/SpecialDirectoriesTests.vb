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
        ''' <summary>
        ''' Remove any OS specific invalid characters, then trim whitespace, then remove any leading "." because they hide the directory on Unix and don't
        ''' need them on other Os's, lastly trim remaining whitespace. Specifically deal with names ". Net Foundation" which become "Net Foundation"
        ''' </summary>
        ''' <param name="InputName"></param>
        ''' <returns>A valid directory name on hosted OS</returns>
        Private Shared Function MakeValidFileName(InputName As String) As String
            Dim invalidFileChars() As Char = IO.Path.GetInvalidFileNameChars()
            For Each c As Char In InputName
                InputName = InputName.Replace(c.ToString(), "")
            Next c
            Return InputName.Trim.TrimStart("."c).TrimStart
        End Function

        Private Shared Function GetAssemblyName(FullName As String) As String
            Dim AssemblyName As String
            'Find the text up to the first comma. Note, this fails if the assembly has a comma in its name
            Dim FirstCommaLocation As Integer = FullName.IndexOf(","c)
            If FirstCommaLocation >= 0 Then
                AssemblyName = FullName.Substring(0, FirstCommaLocation)
            Else
                'The name is not in the format we're expecting so return an empty string
                AssemblyName = ""
            End If

            Return AssemblyName
        End Function

        ''' <summary>
        ''' This function separates applications by CompanyName, ProductName, ProductVersion.
        ''' The only catch is that CompanyName, ProductName has to be specified in the AssemblyInfo.vb file,
        ''' otherwise the name of the assembly will be used instead (which still has a level of separation).
        ''' This function must be kept in sync with the one in coreFx\src\Microsoft.VisualBasic\src\Microsoft\VisualBasic\FileIO\SpecialDirectories.vb
        ''' </summary>
        ''' <returns>[CompanyName]\[ProductName]\[ProductVersion] or name of the assembly</returns>
        Private Shared Function GetCompanyProductVersionPath() As String
            Dim DefaultLocation As String = MakeValidFileName(GetAssemblyName(System.Reflection.Assembly.GetExecutingAssembly.FullName))
            Try
                Dim assm As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
                If assm Is Nothing Then
                    Return DefaultLocation
                End If
                Dim r() As Object = assm.GetCustomAttributes(GetType(System.Reflection.AssemblyCompanyAttribute), False)
                Dim ct As System.Reflection.AssemblyCompanyAttribute = (DirectCast(r(0), System.Reflection.AssemblyCompanyAttribute))
                Dim CompanyName As String = MakeValidFileName(ct.Company)
                Dim ProductName As String = MakeValidFileName(GetAssemblyName(assm.FullName))
                Dim Version As String = MakeValidFileName(assm.GetName().Version.ToString)
                If CompanyName.Length > 0 Then
                    If ProductName.Length = 0 Then
                        Return DefaultLocation
                    End If
                    Return IO.Path.Combine(CompanyName, ProductName, Version)
                Else
                    Return DefaultLocation
                End If
            Catch
            End Try
            Return DefaultLocation
        End Function

        <Fact>
        Public Shared Sub AllUsersApplicationDataFolderTest()
            Dim AllUsersApplicationDataRoot As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)

            If AllUsersApplicationDataRoot.Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.AllUsersApplicationData)
            Else
                Assert.Equal(IO.Path.Combine(AllUsersApplicationDataRoot.TrimEnd(Separators), GetCompanyProductVersionPath).TrimEnd(Separators), SpecialDirectories.AllUsersApplicationData)
            End If
        End Sub

        <Fact>
        Public Shared Sub CurrentUserApplicationDataFolderTest()
            If PlatformDetection.IsWindowsNanoServer OrElse Environment.GetFolderPath(SpecialFolder.ApplicationData, [option]:=SpecialFolderOption.DoNotVerify).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.CurrentUserApplicationData)
            Else
                Assert.Equal(IO.Path.Combine(Environment.GetFolderPath(SpecialFolder.ApplicationData).TrimEnd(Separators), GetCompanyProductVersionPath).TrimEnd(Separators), SpecialDirectories.CurrentUserApplicationData)
            End If
        End Sub

        <Fact>
        Public Shared Sub DesktopFolderTest()
            If PlatformDetection.IsUbuntu1710OrHigher Then
                Assert.Equal(SpecialDirectories.Desktop, "")
            Else
                If Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Length = 0 Then
                    Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.Desktop)
                Else
                    Assert.Equal(Environment.GetFolderPath(SpecialFolder.Desktop).TrimEnd(Separators), SpecialDirectories.Desktop.TrimEnd(Separators))
                End If

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
