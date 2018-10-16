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
            Assert.True(SpecialDirectories.AllUsersApplicationData.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).TrimEnd(Separators)))
        End Sub

        <Fact>
        Public Shared Sub CurrentUserApplicationDataFolderTest()
            Assert.Equal(expected:=Environment.GetFolderPath(SpecialFolder.LocalApplicationData), actual:=SpecialDirectories.CurrentUserApplicationData,)
        End Sub

        <Fact>
        Public Shared Sub DesktopFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.Desktop).TrimEnd(Separators), SpecialDirectories.Desktop.TrimEnd(Separators))
        End Sub

        <Fact>
        Public Shared Sub MyDocumentsFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.Personal).TrimEnd(Separators), SpecialDirectories.MyDocuments.TrimEnd(Separators))
        End Sub

        <Fact>
        Public Shared Sub MyMusicFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.MyMusic).TrimEnd(Separators), SpecialDirectories.MyMusic.TrimEnd(Separators))
        End Sub

        <Fact>
        Public Shared Sub MyPicturesFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.MyPictures).TrimEnd(Separators), SpecialDirectories.MyPictures.TrimEnd(Separators))
        End Sub

        <Fact>
        Public Shared Sub ProgramFilesFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.ProgramFiles).TrimEnd(Separators), SpecialDirectories.ProgramFiles.TrimEnd(Separators))
        End Sub

        <Fact>
        Public Shared Sub ProgramsFolderTest()
            Assert.Equal(Environment.GetFolderPath(SpecialFolder.Programs).TrimEnd(Separators), SpecialDirectories.Programs.TrimEnd(Separators))
        End Sub


        <Fact>
        Public Shared Sub TempFolderTest()
            Assert.Equal(IO.Path.GetTempPath.TrimEnd(Separators), SpecialDirectories.Temp.TrimEnd(Separators))
        End Sub

    End Class
End Namespace
