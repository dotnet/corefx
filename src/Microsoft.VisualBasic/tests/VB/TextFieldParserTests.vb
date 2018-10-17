' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Tests.VB
    Public NotInheritable Class TextFieldParserTests
        Inherits IO.FileCleanupTestBase

        Private Const HeaderLine As String = "seq,oneword,age,street,city,state,zip,dollar,pick,date,paragraph"

        Private Const CSVData As String =
                    HeaderLine & vbCrLf &
            "1,Test,99,Two Words,Rumnoman,KS,80586,$8333.85,GREEN,09/28/1929,""Ipmadaw udoteac vu ote ozehop ane lujat, lar buh cugi lef owici tat liz cogde.
Fatigiug kojitfu taapi iz alujok me zimipa koz latkekij vem fo si cepzizhub li por ejcirlu.
Ewujajlu mivec tooju ad bu cowic irtafit ehaoca vojhehfo aztidun zo wezecmo abe muz wikhutwen idce.
Vesu sejawga tef lahi dirueg si uwmac bidiw nowidza daime sapmim ki casdun urokir tawdac rahaw beiweed."""

        Private Shared FixedFieldWidths() As Integer = {5, 10, 6, -1}
        Private Shared ReadOnly FixedFormatData() As String = {
            "Err  1001                  Cannot access resource.",
            "Err  2014                  Resource not found.",
            "Acc  10/03/2009 User1      Administrator.",
            "Err  0323                  Warning: Invalid access attempt.",
            "Acc  10/03/2009 User2      Standard user.",
            "Acc  10/04/2009 User2      Standard user."
        }

        Private Shared ErrorFormat As Integer() = {5, 5, -1}
        Private Shared StdFormat As Integer() = {5, 10, 11, -1}
        Enum FormatType
            ErrorFormat = 0
            StdFormat = 1
        End Enum
        Private Shared ReadOnly MultipleFormatData() As String = {
            "Err  1001 Cannot access resource.",
            "Err  2014 Resource not found.",
            "Acc  10/03/2009User1      Administrator.",
            "Err  0323 Warning: Invalid access attempt.",
            "Acc  10/03/2009User2      Standard user.",
            "Acc  10/04/2009User2      Standard user."
        }


        ''' <summary>
        ''' Create a new file with TestData
        ''' </summary>
        ''' <param name="TestBase">Opbject to manage temporary Files</param>
        ''' <param name="TestData">Data to be written to file</param>
        ''' <param name="PathFromBase">Optional additional subdirectories that file will be created under</param>
        ''' <param name="TestFileName">Optional Filename, if none a randon one based on TestName will be created</param>
        ''' <returns>Full Path to New File</returns>
        Private Shared Function CreateTestFile(TestBase As TextFieldParserTests, ByVal TestData As String, PathFromBase As String, TestFileName As String, <CallerMemberName> Optional memberName As String = Nothing, <CallerLineNumber> Optional lineNumber As Integer = 0) As String
            Dim TempFileNameWithPath As String
            If TestFileName.Length = 0 Then
                TempFileNameWithPath = TestBase.GetTestFilePath(memberName:=memberName, lineNumber:=lineNumber)
            Else
                Assert.False(IO.Path.IsPathRooted(TestFileName))
                If PathFromBase.Length = 0 Then
                    TempFileNameWithPath = IO.Path.Combine(TestBase.TestDirectory, TestFileName)
                Else
                    ' If we have a Base we must have a filename
                    Assert.False(String.IsNullOrWhiteSpace(TestFileName))
                    TempFileNameWithPath = IO.Path.Combine(TestBase.TestDirectory, PathFromBase, TestFileName)
                End If
            End If
            Assert.False(IO.File.Exists(TempFileNameWithPath), $"File {TempFileNameWithPath} should not exist!")
            ' Write and copy file
            Using sourceStream As New IO.StreamWriter(IO.File.Create(TempFileNameWithPath))
                sourceStream.Write(TestData, 0, TestData.Length)
            End Using

            Return TempFileNameWithPath
        End Function

        <Fact>
        Public Shared Sub BadPathTest()
            Dim TestFile As String = ""
            Using TestBase As New TextFieldParserTests
                TestFile = CreateTestFile(TestBase, HeaderLine & vbCrLf & """" & vbCrLf, PathFromBase:="", TestFileName:="")
                Assert.Throws(Of IO.FileNotFoundException)(Function() New TextFieldParser(IO.Path.ChangeExtension(TestFile, ".txt")))
            End Using
        End Sub

        <Fact>
        Public Shared Sub CSVBadDataTest()
            Dim TestFile As String = ""
            Using TestBase As New TextFieldParserTests
                TestFile = CreateTestFile(TestBase, HeaderLine & ",""", PathFromBase:="", TestFileName:="")
                Using MyReader As New TextFieldParser(TestFile)
                    MyReader.TextFieldType = FieldType.Delimited
                    MyReader.Delimiters = New String() {","}
                    While Not MyReader.EndOfData
                        Assert.Throws(Of MalformedLineException)(Function() MyReader.ReadFields())
                    End While
                End Using
            End Using
        End Sub

        <Fact>
        Public Shared Sub CSVDataTest()
            Dim TestFile As String = ""
            Using TestBase As New TextFieldParserTests
                TestFile = CreateTestFile(TestBase, CSVData, "", "")
                Using MyReader As New TextFieldParser(TestFile)
                    MyReader.TextFieldType = FieldType.Delimited
                    MyReader.Delimiters = New String() {","}
                    Dim currentRow As String()
                    Assert.False(MyReader.EndOfData)
                    Dim HeaderSplit() As String = HeaderLine.Split(CType(",", Char()))
                    currentRow = MyReader.ReadFields()
                    For i As Integer = 0 To HeaderSplit.Length - 1
                        Assert.Equal(HeaderSplit(i), currentRow(i))
                    Next

                    Assert.False(MyReader.EndOfData)
                    currentRow = MyReader.ReadFields()
                    Assert.Equal(11, currentRow.Length)
                    Assert.Equal("1", currentRow(0))
                    Assert.Equal("Test", currentRow(1))
                    Assert.Equal("99", currentRow(2))
                    Assert.Equal("Two Words", currentRow(3))
                    Assert.Equal("Rumnoman", currentRow(4))
                    Assert.Equal("KS", currentRow(5))
                    Assert.Equal("80586", currentRow(6))
                    Assert.Equal("$8333.85", currentRow(7))
                    Assert.Equal("GREEN", currentRow(8))
                    Assert.Equal("09/28/1929", currentRow(9))
                    Assert.Equal("Ipmadaw udoteac vu ote ozehop ane lujat, lar buh cugi lef owici tat liz cogde.
Fatigiug kojitfu taapi iz alujok me zimipa koz latkekij vem fo si cepzizhub li por ejcirlu.
Ewujajlu mivec tooju ad bu cowic irtafit ehaoca vojhehfo aztidun zo wezecmo abe muz wikhutwen idce.
Vesu sejawga tef lahi dirueg si uwmac bidiw nowidza daime sapmim ki casdun urokir tawdac rahaw beiweed.", currentRow(10))
                    Assert.True(MyReader.EndOfData)
                End Using
            End Using
        End Sub

        <Fact>
        Public Shared Sub FixedFormatTest()
            Dim TestFile As String = ""
            Using TestBase As New TextFieldParserTests
                TestFile = CreateTestFile(TestBase, String.Join(vbCrLf, FixedFormatData), PathFromBase:="", TestFileName:="")
                Using MyReader As New TextFieldParser(TestFile)
                    MyReader.TextFieldType = FieldType.FixedWidth
                    MyReader.SetFieldWidths(FixedFieldWidths)

                    Dim CurrentRow As String()
                    Dim CurrentRowIndex As Integer = 0
                    While Not MyReader.EndOfData
                        CurrentRow = MyReader.ReadFields()
                        Assert.Equal(FixedFieldWidths.Length, CurrentRow.Length)
                        Assert.Equal(CurrentRow(0), FixedFormatData(CurrentRowIndex).Substring(0, FixedFieldWidths(0)).Trim)
                        Assert.Equal(CurrentRow(1), FixedFormatData(CurrentRowIndex).Substring(FixedFieldWidths(0), FixedFieldWidths(1)).Trim)
                        Assert.Equal(CurrentRow(2), FixedFormatData(CurrentRowIndex).Substring(FixedFieldWidths(0) + FixedFieldWidths(1), FixedFieldWidths(2)).Trim)
                        Assert.Equal(CurrentRow(3), FixedFormatData(CurrentRowIndex).Substring(FixedFieldWidths(0) + FixedFieldWidths(1) + FixedFieldWidths(2)).Trim)
                        CurrentRowIndex += 1
                    End While
                End Using
            End Using
        End Sub

        <Fact>
        Public Shared Sub MultiFormatTest()
            Dim TestFile As String = ""
            Using TestBase As New TextFieldParserTests
                TestFile = CreateTestFile(TestBase, String.Join(vbCrLf, FixedFormatData), "", "")
                Using MyReader As New TextFieldParser(TestFile)
                    MyReader.TextFieldType = FieldType.FixedWidth
                    MyReader.FieldWidths = StdFormat

                    Dim CurrentFormatType As FormatType
                    Dim CurrentRow As String()
                    Dim CurrentRowIndex As Integer = 0
                    While Not MyReader.EndOfData
                        Dim rowType = MyReader.PeekChars(3)
                        If String.Compare(rowType, "Err") = 0 Then
                            ' If this line describes an error, the format of the row will be different.
                            CurrentFormatType = FormatType.ErrorFormat
                            MyReader.SetFieldWidths(ErrorFormat)
                        Else
                            ' Otherwise parse the fields normally
                            CurrentFormatType = FormatType.StdFormat
                            MyReader.SetFieldWidths(StdFormat)
                        End If

                        CurrentRow = MyReader.ReadFields
                        If CurrentFormatType = FormatType.ErrorFormat Then
                            Assert.Equal(ErrorFormat.Length, CurrentRow.Length)
                            Assert.Equal(CurrentRow(0), MultipleFormatData(CurrentRowIndex).Substring(0, ErrorFormat(0)).Trim)
                            Assert.Equal(CurrentRow(1), MultipleFormatData(CurrentRowIndex).Substring(ErrorFormat(0), ErrorFormat(1)).Trim)
                            Assert.Equal(CurrentRow(2), MultipleFormatData(CurrentRowIndex).Substring(ErrorFormat(0) + ErrorFormat(1)).Trim)
                        Else
                            Assert.Equal(StdFormat.Length, CurrentRow.Length)
                            Assert.Equal(CurrentRow(0), MultipleFormatData(CurrentRowIndex).Substring(0, StdFormat(0)).Trim)
                            Assert.Equal(CurrentRow(1), MultipleFormatData(CurrentRowIndex).Substring(StdFormat(0), StdFormat(1)).Trim)
                            Assert.Equal(CurrentRow(2), MultipleFormatData(CurrentRowIndex).Substring(StdFormat(0) + StdFormat(1), StdFormat(2)).Trim)
                            Assert.Equal(CurrentRow(3), MultipleFormatData(CurrentRowIndex).Substring(StdFormat(0) + StdFormat(1) + StdFormat(2)).Trim)
                        End If
                        CurrentRowIndex += 1
                    End While
                End Using
            End Using
        End Sub
    End Class
End Namespace
