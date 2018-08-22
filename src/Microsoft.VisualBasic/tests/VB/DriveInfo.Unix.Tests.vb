' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Linq
Imports Xunit

Namespace Microsoft.VisualBasic.Tests

    Public NotInheritable Class DriveInfoUnixTests

        <Fact>
        <PlatformSpecific(TestPlatforms.AnyUnix)>
        Public Shared Sub TestGetDrives()
            Dim DriveList() As IO.DriveInfo = FileSystem.Drives().ToArray
            Assert.NotNull(DriveList)
            Assert.[True](DriveList.Length > 0, "Expected at least one drive")
            Assert.All(DriveList, Sub(d) Assert.NotNull(d))
            Assert.Contains(DriveList, Function(d) d.Name = "/")
            Assert.All(DriveList, Sub(d)
                                      ' None of these should throw
                                      Dim dt As IO.DriveType = d.DriveType
                                      Dim isReady As Boolean = d.IsReady
                                      Dim di As IO.DirectoryInfo = d.RootDirectory
                                  End Sub)
        End Sub

    End Class
End Namespace
