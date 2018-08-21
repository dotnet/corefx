' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Imports System
Imports System.IO
Imports System.Linq
Imports Xunit

Namespace Microsoft.VisualBasic.Tests

    Partial Public Class DriveInfoUnixTests

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub TestConstructor()
            Assert.All(
                {"", vbNullChar, vbNullChar & "/"},
                Function(driveName) AssertExtensions.Throws(Of ArgumentException)("driveName", Sub()
                                                                                                   Dim tempVar As New DriveInfo(driveName)
                                                                                               End Sub))

            AssertExtensions.Throws(Of ArgumentNullException)("driveName", Sub()
                                                                               Dim tempVar1 As New DriveInfo(Nothing)
                                                                           End Sub)

            Assert.Equal("/", New DriveInfo("/").Name)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub TestGetDrives()
            Dim drives() As DriveInfo = DriveInfo.GetDrives()
            Assert.NotNull(drives)
            Assert.[True](drives.Length > 0, "Expected at least one drive")
            Assert.All(drives, Sub(d) Assert.NotNull(d))
            Assert.Contains(drives, Function(d) d.Name = "/")
            Assert.All(drives, Sub(d)
                                   ' None of these should throw
                                   Dim dt As DriveType = d.DriveType
                                   Dim isReady As Boolean = d.IsReady
                                   Dim di As DirectoryInfo = d.RootDirectory
                               End Sub)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub PropertiesOfInvalidDrive()
            Dim invalidDriveName As String = "NonExistentDriveName"
            Dim invalidDrive As DriveInfo = New DriveInfo(invalidDriveName)

            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.AvailableFreeSpace)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.DriveFormat)
            Assert.Equal(DriveType.NoRootDirectory, invalidDrive.DriveType)
            Assert.[False](invalidDrive.IsReady)
            Assert.Equal(invalidDriveName, invalidDrive.Name)
            Assert.Equal(invalidDriveName, invalidDrive.ToString())
            Assert.Equal(invalidDriveName, invalidDrive.RootDirectory.Name)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.TotalFreeSpace)
            Assert.Throws(Of DriveNotFoundException)(Function() invalidDrive.TotalSize)
            Assert.Equal(invalidDriveName, invalidDrive.VolumeLabel)   ' VolumeLabel is equivalent to Name on Unix
        End Sub

        <ConditionalFact(GetType(PlatformDetection), NameOf(PlatformDetection.IsNotWindowsSubsystemForLinux))> ' https://github.com/dotnet/corefx/issues/11570
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub PropertiesOfValidDrive()
            Dim root As DriveInfo = New DriveInfo("/")
            Assert.[True](root.AvailableFreeSpace > 0)
            Dim format As String = root.DriveFormat
            Assert.Equal(DriveType.Fixed, root.DriveType)
            Assert.[True](root.IsReady)
            Assert.Equal("/", root.Name)
            Assert.Equal("/", root.ToString())
            Assert.Equal("/", root.RootDirectory.FullName)
            Assert.[True](root.TotalFreeSpace > 0)
            Assert.[True](root.TotalSize > 0)
            Assert.Equal("/", root.VolumeLabel)
        End Sub

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Sub SetVolumeLabel_Throws_PlatformNotSupportedException()
            Dim root As DriveInfo = New DriveInfo("/")
            Assert.Throws(Of PlatformNotSupportedException)(Sub() root.VolumeLabel = root.Name)
        End Sub
    End Class
End Namespace
