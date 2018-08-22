' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic.FileIO

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security
Imports Xunit
Imports System.Text
' DO NOT IMPORT System.IO
Namespace Microsoft.VisualBasic.Tests

    Public NotInheritable Class DriveInfoWindowsTests
        Private Shared Sub DoDriveCheck()
            ' Get Volume Label - valid drive
            Dim serialNumber, maxFileNameLen, fileSystemFlags As Integer
            Dim volNameLen As Integer = 50
            Dim fileNameLen As Integer = 50
            Dim volumeName As StringBuilder = New StringBuilder(volNameLen)
            Dim fileSystemName As StringBuilder = New StringBuilder(fileNameLen)

            Dim validDrive As IO.DriveInfo = FileSystem.Drives().First(Function(d) d.DriveType = IO.DriveType.Fixed)
            Dim volumeInformationSuccess As Boolean = GetVolumeInformation(validDrive.Name, volumeName, volNameLen, serialNumber, maxFileNameLen, fileSystemFlags, fileSystemName, fileNameLen)

            If volumeInformationSuccess Then
                Assert.Equal(volumeName.ToString(), validDrive.VolumeLabel)
            Else
                Dim name As String = validDrive.VolumeLabel
            End If
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Shared Sub TestGetDrives()
            Dim validExpectedDrives As IEnumerable(Of Char) = GetValidDriveLettersOnMachine()
            Dim validActualDrives() As IO.DriveInfo = FileSystem.Drives().ToArray

            ' Test count
            Assert.Equal(validExpectedDrives.Count(), validActualDrives.Count())

            For i As Integer = 0 To validActualDrives.Count() - 1
                ' Test if the drive-letter is correct
                Assert.Contains(validActualDrives(i).Name(0), validExpectedDrives)
            Next
        End Sub

        <Fact>
        Public Shared Sub TestDriveProperties_AppContainer()
            Dim validDrive As IO.DriveInfo = FileSystem.Drives().Where(Function(d) d.DriveType = IO.DriveType.Fixed).First()
            Dim isReady As Boolean = validDrive.IsReady
            If PlatformDetection.IsInAppContainer Then
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.AvailableFreeSpace)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.DriveFormat)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.TotalFreeSpace)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.TotalSize)
                Assert.Throws(Of UnauthorizedAccessException)(Function() validDrive.VolumeLabel)
            Else
                Assert.NotNull(validDrive.Name)
                Assert.NotNull(validDrive.RootDirectory.Name)
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
        Public Shared Sub TestDriveFormat()
            Dim validDrive As IO.DriveInfo = FileSystem.Drives().Where(Function(d) d.DriveType = IO.DriveType.Fixed).First()
            Const VolNameLen As Integer = 50
            Dim VolumeName As StringBuilder = New StringBuilder(VolNameLen)
            Const FileSystemNameLen As Integer = 50
            Dim FileSystemName As StringBuilder = New StringBuilder(FileSystemNameLen)
            Dim serialNumber, maxFileNameLen, fileSystemFlags As Integer
            Dim r As Boolean = GetVolumeInformation(validDrive.Name, VolumeName, VolNameLen, serialNumber, maxFileNameLen, fileSystemFlags, FileSystemName, FileSystemNameLen)
            Dim FileSystemType As String = FileSystemName.ToString

            If r Then
                Assert.Equal(FileSystemType, validDrive.DriveFormat)
            Else
                Assert.Throws(Of IO.IOException)(Function() validDrive.DriveFormat)
            End If

        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        Public Shared Sub TestDriveType()
            Dim validDrive As IO.DriveInfo = FileSystem.Drives().Where(Function(d) d.DriveType = IO.DriveType.Fixed).First()
            Dim expectedDriveType As Integer = GetDriveType(validDrive.Name)
            Assert.Equal(CType(expectedDriveType, IO.DriveType), validDrive.DriveType)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.Windows)>
        <SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "GetDiskFreeSpaceEx blocked in AC")>
        Public Shared Sub TestValidDiskSpaceProperties()
            Dim win32Result As Boolean
            Dim fbUser As Long = -1
            Dim tbUser As Long
            Dim fbTotal As Long
            Dim drive As IO.DriveInfo
            drive = FileSystem.Drives().Where(Function(d) d.DriveType = IO.DriveType.Fixed).First()
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
        Public Shared Sub GetVolumeLabel_Returns_CorrectLabel()
            If PlatformDetection.IsInAppContainer Then
                Assert.Throws(Of UnauthorizedAccessException)(Sub() DoDriveCheck())
            Else
                DoDriveCheck()
            End If
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

        Private Shared Iterator Function GetValidDriveLettersOnMachine() As IEnumerable(Of Char)
            Dim mask As UInteger = CUInt(GetLogicalDrives())
            Assert.NotEqual(Of UInteger)(mask, 0)

            Dim bits As BitArray = New BitArray(New Integer() {CInt(mask)})
            For i As Integer = 0 To bits.Length - 1
                Dim letter As Char = ChrW(AscW("A"c) + i)
                If bits(i) Then
                    Yield letter
                End If
            Next
        End Function
    End Class
End Namespace
