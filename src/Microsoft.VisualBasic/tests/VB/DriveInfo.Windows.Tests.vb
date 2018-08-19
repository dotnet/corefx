' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security
Imports Xunit
Imports System.Text
Imports System.IO

Namespace System.IO.FileSystem.DriveInfoTests

    Public Class DriveInfoWindowsTests

        <Theory>
        <InlineData(":")>
        <InlineData("://")>
        <InlineData(":\")>
        <InlineData(":/")>
        <InlineData(":\\")>
        <InlineData("Az")>
        <InlineData("1")>
        <InlineData("a1")>
        <InlineData("\\share")>
        <InlineData("\\")>
        <InlineData("c ")>
        <InlineData("")>
        <InlineData(" c")>
        Private Sub DoDriveCheck()
            ' Get Volume Label - valid drive
            Dim serialNumber, maxFileNameLen, fileSystemFlags As Integer
            Dim volNameLen As Integer = 50
            Dim fileNameLen As Integer = 50
            Dim volumeName As StringBuilder = New StringBuilder(volNameLen)
            Dim fileSystemName As StringBuilder = New StringBuilder(fileNameLen)

            Dim validDrive As DriveInfo = Drives().First(Function(d) d.DriveType = DriveType.Fixed)
            Dim volumeInformationSuccess As Boolean = GetVolumeInformation(validDrive.Name, volumeName, volNameLen, serialNumber, maxFileNameLen, fileSystemFlags, fileSystemName, fileNameLen)

            If volumeInformationSuccess Then
                Assert.Equal(volumeName.ToString(), validDrive.VolumeLabel)
            Else
                Dim name As String = validDrive.VolumeLabel
            End If
        End Sub

        Public Sub Ctor_InvalidPath_ThrowsArgumentException(driveName As String)
            AssertExtensions.Throws(Of ArgumentException)("driveName", Nothing, Function() New DriveInfo(driveName))
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub TestConstructor()
            Dim variableInput As String() = {"{0}", "{0}", "{0}:", "{0}:", "{0}:\", "{0}:\\", "{0}://"}

            ' Test Null
            Try
                Dim tempVar As New DriveInfo(Nothing)
                Assert.NotNull(tempVar)
            Catch ex As Exception

            End Try

            ' Test Valid DriveLetter
            Dim validDriveLetter As Char = GetValidDriveLettersOnMachine().First()
            For Each input As String In variableInput
                Dim name As String = String.Format(input, validDriveLetter)
                Dim dInfo As DriveInfo = New DriveInfo(name)
                Assert.Equal(String.Format("{0}:\", validDriveLetter), dInfo.Name)
            Next
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub TestGetDrives()
            Dim validExpectedDrives As IEnumerable(Of Char) = GetValidDriveLettersOnMachine()
            Dim validActualDrives() As DriveInfo = DriveInfo.GetDrives()

            ' Test count
            Assert.Equal(validExpectedDrives.Count(), validActualDrives.Count())

            For i As Integer = 0 To validActualDrives.Count() - 1
                ' Test if the driveletter is correct
                Assert.Contains(validActualDrives(i).Name(0), validExpectedDrives)
            Next
        End Sub

        <Fact>
        Public Sub TestDriveProperties_AppContainer()
            Dim validDrive As DriveInfo = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Fixed).First()
            Dim isReady As Boolean = validDrive.IsReady
            Assert.NotNull(validDrive.Name)
            Assert.NotNull(validDrive.RootDirectory.Name)

            If PlatformDetection.IsInAppContainer Then
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.AvailableFreeSpace)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.DriveFormat)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.TotalFreeSpace)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.TotalSize)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.VolumeLabel)
            Else
                Assert.NotNull(validDrive.DriveFormat)
                Assert.[True](validDrive.AvailableFreeSpace > 0)
                Assert.[True](validDrive.TotalFreeSpace > 0)
                Assert.[True](validDrive.TotalSize > 0)
                Assert.NotNull(validDrive.VolumeLabel)
            End If
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        <SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Accessing drive format is not permitted inside an AppContainer.")>
        Public Sub TestDriveFormat()
            Dim validDrive As DriveInfo = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Fixed).First()
            Const volNameLen As Integer = 50
            Dim volumeName As StringBuilder = New StringBuilder(volNameLen)
            Const fileSystemNameLen As Integer = 50
            Dim fileSystemName As StringBuilder = New StringBuilder(fileSystemNameLen)
            Dim serialNumber, maxFileNameLen, fileSystemFlags As Integer
            Dim r As Boolean = GetVolumeInformation(validDrive.Name, volumeName, volNameLen, serialNumber, maxFileNameLen, fileSystemFlags, fileSystemName, fileSystemNameLen)
            Dim fileSystem As String = fileSystemName.ToString

            If r Then
                Assert.Equal(fileSystem, validDrive.DriveFormat)
            Else
                Assert.Throws(Of IOException)(Function() validDrive.DriveFormat)
            End If

            ' Test Invalid drive
            Dim invalidDrive As DriveInfo = New DriveInfo(GetInvalidDriveLettersOnMachine().First().ToString())
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.DriveFormat)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub TestDriveType()
            Dim validDrive As DriveInfo = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Fixed).First()
            Dim expectedDriveType As Integer = GetDriveType(validDrive.Name)
            Assert.Equal(CType(expectedDriveType, DriveType), validDrive.DriveType)

            ' Test Invalid drive
            Dim invalidDrive As DriveInfo = New DriveInfo(GetInvalidDriveLettersOnMachine().First().ToString())
            Assert.Equal(invalidDrive.DriveType, DriveType.NoRootDirectory)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        <SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "GetDiskFreeSpaceEx blocked in AC")>
        Public Sub TestValidDiskSpaceProperties()
            Dim win32Result As Boolean
            Dim fbUser As Long = -1
            Dim tbUser As Long
            Dim fbTotal As Long
            Dim drive As DriveInfo

            drive = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Fixed).First()
            If drive.IsReady Then
                win32Result = GetDiskFreeSpaceEx(drive.Name, fbUser, tbUser, fbTotal)
                Assert.[True](win32Result)

                If fbUser <> drive.AvailableFreeSpace Then
                    Assert.[True](drive.AvailableFreeSpace >= 0)
                End If

                ' valid property getters shouldn't throw
                Dim name As String = drive.Name
                Dim format As String = drive.DriveFormat
                Assert.Equal(name, drive.ToString())

                ' totalsize should not change for a fixed drive.
                Assert.Equal(tbUser, drive.TotalSize)

                If fbTotal <> drive.TotalFreeSpace Then
                    Assert.[True](drive.TotalFreeSpace >= 0)
                End If
            End If
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub TestInvalidDiskProperties()
            Dim invalidDriveName As String = GetInvalidDriveLettersOnMachine().First().ToString()
            Dim invalidDrive As DriveInfo = New DriveInfo(invalidDriveName)

            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.AvailableFreeSpace)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.DriveFormat)
            Assert.Equal(DriveType.NoRootDirectory, invalidDrive.DriveType)
            Assert.[False](invalidDrive.IsReady)
            Assert.Equal(invalidDriveName & $":\", invalidDrive.Name)
            Assert.Equal(invalidDriveName & $":\", invalidDrive.ToString())
            Assert.Equal(invalidDriveName & $":\", invalidDrive.RootDirectory.FullName)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.TotalFreeSpace)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.TotalSize)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.VolumeLabel)
            Assert.Throws(Of DriveNotFoundException)(Sub() invalidDrive.VolumeLabel = Nothing)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub GetVolumeLabel_Returns_CorrectLabel()
            If PlatformDetection.IsInAppContainer Then
                Assert.Throws(Of UnauthorizedAccessException)(Sub() DoDriveCheck())
            Else
                DoDriveCheck()
            End If
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub SetVolumeLabel_Roundtrips()
            Dim drive As DriveInfo = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Fixed).First()
            ' Inside an AppContainer access to VolumeLabel is denied.
            If PlatformDetection.IsInAppContainer Then
                Assert.Throws(Of UnauthorizedAccessException)(Function() drive.VolumeLabel)
                Return
            End If

            Dim currentLabel As String = drive.VolumeLabel
            Try
                drive.VolumeLabel = currentLabel ' shouldn't change the state of the drive regardless of success
            Catch __unusedUnauthorizedAccessException1__ As UnauthorizedAccessException

            End Try

            Assert.Equal(drive.VolumeLabel, currentLabel)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Sub VolumeLabelOnNetworkOrCdRom_Throws()
            ' Test setting the volume label on a Network or CD-ROM
            Dim noAccessDrive As IEnumerable(Of DriveInfo) = DriveInfo.GetDrives().Where(Function(d) d.DriveType = DriveType.Network OrElse d.DriveType = DriveType.CDRom)
            For Each adrive As DriveInfo In noAccessDrive
                If adrive.IsReady Then
                    Dim e As Exception = Assert.ThrowsAny(Of Exception)(Sub()
                                                                            adrive.VolumeLabel = Nothing
                                                                        End Sub)
                    Assert.[True](
                        TypeOf e Is UnauthorizedAccessException OrElse
                    TypeOf e Is IOException OrElse
                    TypeOf e Is SecurityException)
                End If
            Next
        End Sub

        <DllImport("kernel32.dll", SetLastError:=True)>
        Friend Shared Function GetLogicalDrives() As Integer
        End Function

        <DllImport("kernel32.dll", EntryPoint:="GetVolumeInformationW", CharSet:=CharSet.Unicode, SetLastError:=True, BestFitMapping:=False)>
        Friend Shared Function GetVolumeInformation(drive As String, volumeName As StringBuilder, volumeNameBufLen As Integer, <Out> ByRef volSerialNumber As Integer, <Out> ByRef maxFileNameLen As Integer, <Out> ByRef fileSystemFlags As Integer, fileSystemName As StringBuilder, fileSystemNameBufLen As Integer) As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, EntryPoint:="GetDriveTypeW", CharSet:=CharSet.Unicode)>
        Friend Shared Function GetDriveType(drive As String) As Integer
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Friend Shared Function GetDiskFreeSpaceEx(drive As String, <Out> ByRef freeBytesForUser As Long, <Out> ByRef totalBytes As Long, <Out> ByRef freeBytes As Long) As Boolean
        End Function

        Private Iterator Function GetValidDriveLettersOnMachine() As IEnumerable(Of Char)
            Dim mask As UInteger = CUInt(GetLogicalDrives())
            Assert.NotEqual(Of UInteger)(mask, 0)

            Dim bits As BitArray = New BitArray(New Integer() {CInt(mask)})
            For i As Integer = 0 To bits.Length - 1
                Dim letter As Char = ChrW(CInt("A"c & i))
                If bits(i) Then
                    Yield letter
                End If
            Next
        End Function

        Private Iterator Function GetInvalidDriveLettersOnMachine() As IEnumerable(Of Char)
            Dim mask As UInteger = CUInt(GetLogicalDrives())
            Assert.NotEqual(Of UInteger)(mask, 0)

            Dim bits As BitArray = New BitArray(New Integer() {CInt(mask)})
            For i As Integer = 0 To bits.Length - 1
                Dim letter As Char = ChrW(CInt("A"c & i))
                If Not bits(i) Then
                    If Char.IsLetter(letter) Then
                        Yield letter
                    End If
                End If
            Next
        End Function
    End Class
End Namespace
