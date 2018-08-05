' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System
Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Global.Microsoft.VisualBasic

    Public Module Strings

        '============================================================================
        ' Character manipulation functions.
        '============================================================================
        Public Function Asc(ByVal [String] As Char) As Integer

            'The IConvertible.ToInt32 implementation on Char
            '   just calls Convert.ToInt32()
            Dim CharValue As Integer = Convert.ToInt32([String])

            If CharValue < 128 Then
                Return CharValue
            End If

            Try
                Dim enc As Encoding
                Dim b() As Byte
                Dim c() As Char
                Dim iByteCount As Integer

                enc = GetFileIOEncoding()

                c = New Char() {[String]}

                If enc.IsSingleByte Then
                    'SBCS
                    b = New Byte(0) {}
                    iByteCount = enc.GetBytes(c, 0, 1, b, 0)
                    Return b(0)
                End If

                'DBCS char
                b = New Byte(1) {}
                iByteCount = enc.GetBytes(c, 0, 1, b, 0)
                If iByteCount = 1 Then
                    Return b(0)
                End If
                If BitConverter.IsLittleEndian Then
                    'Swap the bytes since storage is big-endian
                    Dim byt As Byte
                    byt = b(0)
                    b(0) = b(1)
                    b(1) = byt
                End If
                Return BitConverter.ToInt16(b, 0)

            Catch ex As Exception
                Throw ex

            End Try

        End Function

        Public Function Asc(ByVal [String] As String) As Integer

            If ([String] Is Nothing) OrElse ([String].Length = 0) Then
                Throw New ArgumentException(SR.Format(SR.Argument_LengthGTZero1, NameOf([String])), NameOf([String]))
            End If

            Dim ch As Char = [String].Chars(0)
            Return Asc(ch)

        End Function

        Public Function AscW([String] As String) As Integer
            If ([String] Is Nothing) OrElse ([String].Length = 0) Then
                Throw New Global.System.ArgumentException(SR.Format(SR.Argument_LengthGTZero1, NameOf([String])), NameOf([String]))
            End If
            Return AscW([String].Chars(0))
        End Function

        Public Function AscW([String] As Char) As Integer
            Return AscW([String])
        End Function

        Public Function Chr(ByVal CharCode As Integer) As Char
            ' Documentation claims that < 0 or > 255 gives an ArgumentException
            If CharCode < -32768 OrElse CharCode > 65535 Then
                Throw New ArgumentException(SR.Format(SR.Argument_RangeTwoBytes1, NameOf(CharCode)), NameOf(CharCode))
            End If

            If CharCode >= 0 AndAlso CharCode <= 127 Then
                Return Convert.ToChar(CharCode)
            End If

            Try
                Dim enc As Encoding

                enc = Encoding.GetEncoding(GetLocaleCodePage())

                If enc.IsSingleByte Then
                    If CharCode < 0 OrElse CharCode > 255 Then
                        Throw VbMakeException(vbErrors.IllegalFuncCall)
                    End If
                End If

                Dim dec As Decoder
                Dim CharCount As Integer
                Dim c(1) As Char 'Use 2 char array, but only return first Char if two returned
                Dim b(1) As Byte

                dec = enc.GetDecoder()
                If CharCode >= 0 AndAlso CharCode <= 255 Then
                    b(0) = CByte(CharCode And &HFFS)
                    CharCount = dec.GetChars(b, 0, 1, c, 0)

                Else
                    'Bytes must be swapped in memory to HI/LO
                    b(0) = CByte((CharCode And &HFF00I) >> 8)
                    b(1) = CByte(CharCode And &HFFI)
                    CharCount = dec.GetChars(b, 0, 2, c, 0)

                End If

                'VB6 ignored the lobyte if it hibyte was not a valid lead character
                'CharCount will be zero if the hibyte was not a lead character

                Return c(0)

            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function ChrW(CharCode As Integer) As Char
            If CharCode < -32768 OrElse CharCode > 65535 Then
                Throw New ArgumentException(SR.Format(SR.Argument_RangeTwoBytes1, NameOf(CharCode)), NameOf(CharCode))
            End If
            Return Global.System.Convert.ToChar(CharCode And &HFFFFI)
        End Function

        '============================================================================
        ' String manipulation functions.
        '============================================================================
        Public Function Filter(ByVal Source() As Object, ByVal Match As String, Optional ByVal Include As Boolean = True, <Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute()> Optional ByVal [Compare] As CompareMethod = CompareMethod.Binary) As String()

            Dim Size As Integer = UBound(Source)
            Dim StringSource(Size) As String

            Try
                For i As Integer = 0 To Size
                    StringSource(i) = CStr(Source(i))
                Next i
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As System.Threading.ThreadAbortException
                Throw ex
            Catch
                Throw New ArgumentException(SR.Format(SR.Argument_InvalidValueType2, NameOf(Source), "String"), NameOf(Source))
            End Try

            Return Filter(StringSource, Match, Include, [Compare])
        End Function

        Public Function Filter(ByVal Source() As String, ByVal Match As String, Optional ByVal Include As Boolean = True, <Microsoft.VisualBasic.CompilerServices.OptionCompareAttribute()> Optional ByVal [Compare] As CompareMethod = CompareMethod.Binary) As String()
            Try
                Dim TmpResult() As String
                Dim lNumElements As Integer
                Dim lSourceIndex As Integer
                Dim lResultIndex As Integer
                Dim sStringElement As String
                Dim iFlags As CompareOptions
                Dim CompInfo As CompareInfo
                Dim Loc As CultureInfo

                'Do error checking
                If Source.Rank <> 1 Then
                    Throw New ArgumentException(SR.Argument_RankEQOne1, NameOf(Source))
                End If

                If Match Is Nothing OrElse Match.Length = 0 Then
                    Return Nothing
                End If

                lNumElements = Source.Length

                'up the globalization info
                Loc = GetCultureInfo()
                CompInfo = Loc.CompareInfo

                If [Compare] = CompareMethod.Text Then
                    iFlags = CompareOptions.IgnoreCase
                End If

                'Compare each element and build the result array
                ReDim TmpResult(lNumElements - 1)

                For lSourceIndex = 0 To lNumElements - 1
                    sStringElement = Source(lSourceIndex)

                    If (sStringElement Is Nothing) Then
                        'Skip
                    ElseIf (CompInfo.IndexOf(sStringElement, Match, iFlags) >= 0) = Include Then
                        TmpResult(lResultIndex) = sStringElement
                        lResultIndex = lResultIndex + 1
                    End If
                Next lSourceIndex

                If lResultIndex = 0 Then
                    ReDim TmpResult(-1)
                    Return TmpResult
                End If

                If lResultIndex = TmpResult.Length Then
                    'No redim required
                    Return TmpResult
                End If

                ReDim Preserve TmpResult(lResultIndex - 1)
                Return TmpResult
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        '============================================================================
        ' Left/Right/Mid/Trim functions.
        '============================================================================
        Public Function Left(ByVal [str] As String, ByVal Length As Integer) As String
            '-------------------------------------------------------------
            '   lLen < 0 throws InvalidArgument exception
            '   lLen > Len([str]) let lLen = Len([str])
            '   returned computed string
            '-------------------------------------------------------------
            If Length < 0 Then
                Throw New ArgumentException(SR.Format(SR.Argument_GEZero1, NameOf(Length)), NameOf(Length))
            ElseIf Length = 0 OrElse [str] Is Nothing Then
                Return ""
            Else
                If Length >= [str].Length Then
                    Return [str]
                Else
                    Return [str].Substring(0, Length)
                End If
            End If
        End Function

        Public Function LTrim(ByVal str As String) As String
            If str Is Nothing OrElse str.Length = 0 Then
                Return ""
            Else
                Dim ch As Char
                ch = str.Chars(0)

                If ch = chSpace OrElse ch = chIntlSpace Then
                    Return str.TrimStart(m_achIntlSpace)
                End If
                Return str
            End If
        End Function

        Public Function Mid(ByVal [str] As String, ByVal Start As Integer) As String
            Try
                If [str] Is Nothing Then
                    Return Nothing
                Else
                    Return Mid([str], Start, [str].Length)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function Mid(ByVal [str] As String, ByVal Start As Integer, ByVal Length As Integer) As String
            '-------------------------------------------------------------
            '  Notes:
            '   VB6 order of execution
            '   verify Start > 0 ==> vbIllegalFuncCall
            '   verify lLen > 0 ==> vbIllegalFuncCall
            '   return computed string
            '-------------------------------------------------------------
            If Start <= 0 Then
                Throw New ArgumentException(SR.Format(SR.Argument_GTZero1, NameOf(Start)), NameOf(Start))
            ElseIf Length < 0 Then
                Throw New ArgumentException(SR.Format(SR.Argument_GEZero1, NameOf(Length)), NameOf(Length))
            ElseIf Length = 0 OrElse [str] Is Nothing Then
                Return ""
            End If

            Dim lStrLen As Integer

            lStrLen = [str].Length

            If Start > lStrLen Then
                Return ""
            ElseIf (Start + Length) > lStrLen Then
                Return [str].Substring(Start - 1)
            Else
                Return [str].Substring(Start - 1, Length)
            End If
        End Function

        Public Function Right(ByVal [str] As String, ByVal Length As Integer) As String
            If Length < 0 Then
                Throw New ArgumentException(SR.Format(SR.Argument_GEZero1, NameOf(Length)), NameOf(Length))
            End If

            If Length = 0 OrElse [str] Is Nothing Then
                Return ""
            End If

            Dim lStrLen As Integer

            lStrLen = [str].Length

            If Length >= lStrLen Then
                Return [str]
            End If

            Return [str].Substring(lStrLen - Length, Length)
        End Function

        Public Function RTrim(ByVal [str] As String) As String
            Try
                If [str] Is Nothing OrElse str.Length = 0 Then
                    Return ""
                End If

                Dim ch As Char
                ch = str.Chars(str.Length - 1)

                If ch = chSpace OrElse ch = chIntlSpace Then
                    Return [str].TrimEnd(m_achIntlSpace)
                End If
                Return str
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function Trim(ByVal str As String) As String
            Try
                If str Is Nothing OrElse str.Length = 0 Then
                    Return ""
                End If

                Dim ch As Char = str.Chars(0)
                If ch = chSpace OrElse ch = chIntlSpace Then
                    Return str.Trim(m_achIntlSpace)
                Else
                    ch = str.Chars(str.Length - 1)
                    If ch = chSpace OrElse ch = chIntlSpace Then
                        Return str.Trim(m_achIntlSpace)
                    End If
                End If
                Return str

            Catch ex As Exception
                Throw ex
            End Try
        End Function
    End Module
End Namespace
