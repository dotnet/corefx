' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic
Imports System
Imports Xunit

Namespace Microsoft.VisualBasic.Tests

    Public NotInheritable Class Dummy

        <Fact>
        Public Shared Sub Dummy()

            Dim dateTimeNowBefore As Date = Date.Now()
            Dim now As Date = DateAndTime.Now()
            Dim dateTimeNowAfter As Date = Date.Now()

            Assert.InRange(now, dateTimeNowBefore, dateTimeNowAfter)

        End Sub

    End Class

End Namespace
