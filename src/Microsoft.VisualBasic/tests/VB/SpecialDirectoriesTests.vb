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
        ''' If a path does not exist, one is created in the following format
        ''' C:\Documents and Settings\[UserName]\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        ''' The first function separates applications by CompanyName, ProductName, ProductVersion.
        ''' The only catch is that CompanyName, ProductName has to be specified in the AssemblyInfo.vb file,
        ''' otherwise the name of the assembly will be used instead (which still has a level of separation).
        ''' </summary>
        ''' <returns>[CompanyName]\[ProductName]\[ProductVersion] </returns>
        Private Shared Function GetCompanyProductVersionPath() As String
            Dim ApplicationDataPath As String = String.Empty
            Try
                Dim assm As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
                If assm Is Nothing Then
                    assm = System.Reflection.Assembly.GetExecutingAssembly
                    Return GetAssemblyName(assm.FullName)
                End If
                Dim at As Type = GetType(System.Reflection.AssemblyCompanyAttribute)
                Dim r() As Object = assm.GetCustomAttributes(at, False)
                Dim ct As System.Reflection.AssemblyCompanyAttribute = (DirectCast(r(0), System.Reflection.AssemblyCompanyAttribute))
                ApplicationDataPath = IO.Path.Combine(ct.Company, GetAssemblyName(assm.FullName), assm.GetName().Version.ToString)
            Catch
            End Try
            Return ApplicationDataPath
        End Function

        <Fact>
        Public Shared Sub AllUsersApplicationDataFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.AllUsersApplicationData)
            Else
                Assert.Equal(IO.Path.Combine(Environment.GetFolderPath(SpecialFolder.CommonApplicationData).TrimEnd(Separators), GetCompanyProductVersionPath).TrimEnd(Separators), SpecialDirectories.AllUsersApplicationData)
            End If
        End Sub

        <Fact>
        Public Shared Sub CurrentUserApplicationDataFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.CurrentUserApplicationData)
            Else
                Assert.True(SpecialDirectories.CurrentUserApplicationData.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd(Separators)))
            End If
        End Sub

        <Fact>
        Public Shared Sub DesktopFolderTest()
            If Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Length = 0 Then
                Assert.Throws(Of IO.DirectoryNotFoundException)(Function() SpecialDirectories.Desktop)
            Else
                Assert.Equal(Environment.GetFolderPath(SpecialFolder.Desktop).TrimEnd(Separators), SpecialDirectories.Desktop.TrimEnd(Separators))
            End If
        End Sub

        <Fact>
        Public Shared Sub MyDocumentsFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.Personal).TrimEnd(Separators), SpecialDirectories.MyDocuments.TrimEnd(Separators))
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
