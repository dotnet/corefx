' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Reflection

Imports Microsoft.VisualBasic.CompilerServices.Symbols
Imports Microsoft.VisualBasic.CompilerServices.ConversionResolution
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils
Imports Container = Microsoft.VisualBasic.CompilerServices.Symbols.Container

Namespace Global.Microsoft.VisualBasic.CompilerServices
    <Global.System.Diagnostics.DebuggerNonUserCode()>
    <Global.System.ComponentModel.EditorBrowsable(Global.System.ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class Conversions
        Private Sub New()
        End Sub
        Private Shared Function GetEnumValue(Value As Object) As Object
            Dim underlyingType As Type = System.Enum.GetUnderlyingType(Value.GetType())
            If underlyingType.Equals(GetType(SByte)) Then
                Return DirectCast(Value, SByte)
            ElseIf underlyingType.Equals(GetType(Byte)) Then
                Return DirectCast(Value, Byte)
            ElseIf underlyingType.Equals(GetType(Global.System.Int16)) Then
                Return DirectCast(Value, Global.System.Int16)
            ElseIf underlyingType.Equals(GetType(Global.System.UInt16)) Then
                Return DirectCast(Value, Global.System.UInt16)
            ElseIf underlyingType.Equals(GetType(Global.System.Int32)) Then
                Return DirectCast(Value, Global.System.Int32)
            ElseIf underlyingType.Equals(GetType(Global.System.UInt32)) Then
                Return DirectCast(Value, Global.System.UInt32)
            ElseIf underlyingType.Equals(GetType(Global.System.Int64)) Then
                Return DirectCast(Value, Global.System.Int64)
            ElseIf underlyingType.Equals(GetType(Global.System.UInt64)) Then
                Return DirectCast(Value, Global.System.UInt64)
            Else
                Throw New Global.System.InvalidCastException
            End If
        End Function
        Public Shared Function ToBoolean(Value As String) As Boolean
            If Value Is Nothing Then
                Value = ""
            End If
            Try
                Dim loc As Global.System.Globalization.CultureInfo = GetCultureInfo()
                If loc.CompareInfo.Compare(Value, Boolean.FalseString, Global.System.Globalization.CompareOptions.IgnoreCase) = 0 Then
                    Return False
                ElseIf loc.CompareInfo.Compare(Value, Boolean.TrueString, Global.System.Globalization.CompareOptions.IgnoreCase) = 0 Then
                    Return True
                End If
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CBool(i64Value)
                End If
                Return CBool(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToBoolean(Value As Object) As Boolean
            If Value Is Nothing Then
                Return False
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CBool(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CBool(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CBool(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CBool(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CBool(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CBool(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CBool(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CBool(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CBool(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CBool(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CBool(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CBool(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CBool(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToByte(Value As String) As Byte
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CByte(i64Value)
                End If
                Return CByte(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToByte(Value As Object) As Byte
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CByte(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CByte(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CByte(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CByte(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CByte(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CByte(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CByte(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CByte(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CByte(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CByte(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CByte(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CByte(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CByte(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToSByte(Value As String) As SByte
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CSByte(i64Value)
                End If
                Return CSByte(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToSByte(Value As Object) As SByte
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CSByte(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CSByte(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CSByte(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CSByte(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CSByte(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CSByte(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CSByte(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CSByte(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CSByte(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CSByte(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CSByte(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CSByte(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CSByte(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToShort(Value As String) As Short
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CShort(i64Value)
                End If
                Return CShort(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToShort(Value As Object) As Short
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CShort(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CShort(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CShort(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CShort(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CShort(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CShort(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CShort(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CShort(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CShort(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CShort(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CShort(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CShort(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CShort(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToUShort(Value As String) As UShort
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CUShort(i64Value)
                End If
                Return CUShort(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToUShort(Value As Object) As UShort
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CUShort(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CUShort(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CUShort(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CUShort(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CUShort(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CUShort(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CUShort(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CUShort(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CUShort(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CUShort(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CUShort(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CUShort(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CUShort(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToInteger(Value As String) As Integer
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CInt(i64Value)
                End If
                Return CInt(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToInteger(Value As Object) As Integer
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CInt(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CInt(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CInt(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CInt(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CInt(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CInt(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CInt(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CInt(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CInt(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CInt(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CInt(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CInt(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CInt(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToUInteger(Value As String) As UInteger
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CUInt(i64Value)
                End If
                Return CUInt(ParseDouble(Value))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToUInteger(Value As Object) As UInteger
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CUInt(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CUInt(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CUInt(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CUInt(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CUInt(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CUInt(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CUInt(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CUInt(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CUInt(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CUInt(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CUInt(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CUInt(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CUInt(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToLong(Value As String) As Long
            If (Value Is Nothing) Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CLng(i64Value)
                End If
                Return CLng(ParseDecimal(Value, Nothing))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToLong(Value As Object) As Long
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CLng(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CLng(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CLng(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CLng(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CLng(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CLng(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CLng(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CLng(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CLng(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CLng(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CLng(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CLng(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CLng(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToULong(Value As String) As ULong
            If (Value Is Nothing) Then
                Return 0
            End If
            Try
                Dim ui64Value As Global.System.UInt64
                If IsHexOrOctValue(Value, ui64Value) Then
                    Return CULng(ui64Value)
                End If
                Return CULng(ParseDecimal(Value, Nothing))
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Function ToULong(Value As Object) As ULong
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CULng(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CULng(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CULng(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CULng(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CULng(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CULng(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CULng(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CULng(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CULng(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CULng(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CULng(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CULng(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CULng(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToDecimal(Value As Boolean) As Decimal
            If Value Then
                Return -1D
            Else
                Return 0D
            End If
        End Function
        Public Shared Function ToDecimal(Value As String) As Decimal
            If Value Is Nothing Then
                Return 0D
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CDec(i64Value)
                End If
                Return ParseDecimal(Value, Nothing)
            Catch e1 As Global.System.OverflowException
                Throw e1
            Catch e2 As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e2.Message, e2)
            End Try
        End Function
        Public Shared Function ToDecimal(Value As Object) As Decimal
            If Value Is Nothing Then
                Return 0D
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CDec(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CDec(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CDec(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CDec(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CDec(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CDec(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CDec(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CDec(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CDec(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CDec(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CDec(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CDec(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CDec(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Private Shared Function ParseDecimal(value As String, numberFormat As Global.System.Globalization.NumberFormatInfo) As Decimal
            Dim normalizedNumberFormat As Global.System.Globalization.NumberFormatInfo
            Dim culture As Global.System.Globalization.CultureInfo = GetCultureInfo()
            If numberFormat Is Nothing Then
                numberFormat = culture.NumberFormat
            End If
            normalizedNumberFormat = GetNormalizedNumberFormat(numberFormat)
            Const flags As Global.System.Globalization.NumberStyles =
                    Global.System.Globalization.NumberStyles.AllowDecimalPoint Or
                    Global.System.Globalization.NumberStyles.AllowExponent Or
                    Global.System.Globalization.NumberStyles.AllowLeadingSign Or
                    Global.System.Globalization.NumberStyles.AllowLeadingWhite Or
                    Global.System.Globalization.NumberStyles.AllowThousands Or
                    Global.System.Globalization.NumberStyles.AllowTrailingSign Or
                    Global.System.Globalization.NumberStyles.AllowParentheses Or
                    Global.System.Globalization.NumberStyles.AllowTrailingWhite Or
                    Global.System.Globalization.NumberStyles.AllowCurrencySymbol
            value = ToHalfwidthNumbers(value, culture)
            Try
                Return Global.System.Decimal.Parse(value, flags, normalizedNumberFormat)
            Catch formatEx As Global.System.FormatException When Not (numberFormat Is normalizedNumberFormat)
                Return Global.System.Decimal.Parse(value, flags, numberFormat)
            Catch ex As Global.System.Exception
                Throw ex
            End Try
        End Function
        Private Shared Function GetNormalizedNumberFormat(inNumberFormat As Global.System.Globalization.NumberFormatInfo) As Global.System.Globalization.NumberFormatInfo
            Dim outNumberFormat As Global.System.Globalization.NumberFormatInfo
            With inNumberFormat
                If (Not .CurrencyDecimalSeparator Is Nothing) AndAlso
                   (Not .NumberDecimalSeparator Is Nothing) AndAlso
                   (Not .CurrencyGroupSeparator Is Nothing) AndAlso
                   (Not .NumberGroupSeparator Is Nothing) AndAlso
                   (.CurrencyDecimalSeparator.Length = 1) AndAlso
                   (.NumberDecimalSeparator.Length = 1) AndAlso
                   (.CurrencyGroupSeparator.Length = 1) AndAlso
                   (.NumberGroupSeparator.Length = 1) AndAlso
                   (.CurrencyDecimalSeparator.Chars(0) = .NumberDecimalSeparator.Chars(0)) AndAlso
                   (.CurrencyGroupSeparator.Chars(0) = .NumberGroupSeparator.Chars(0)) AndAlso
                   (.CurrencyDecimalDigits = .NumberDecimalDigits) Then
                    Return inNumberFormat
                End If
            End With
            With inNumberFormat
                If (Not .CurrencyDecimalSeparator Is Nothing) AndAlso
                   (Not .NumberDecimalSeparator Is Nothing) AndAlso
                   (.CurrencyDecimalSeparator.Length = .NumberDecimalSeparator.Length) AndAlso
                   (Not .CurrencyGroupSeparator Is Nothing) AndAlso
                   (Not .NumberGroupSeparator Is Nothing) AndAlso
                   (.CurrencyGroupSeparator.Length = .NumberGroupSeparator.Length) Then
                    Dim i As Integer
                    For i = 0 To .CurrencyDecimalSeparator.Length - 1
                        If (.CurrencyDecimalSeparator.Chars(i) <> .NumberDecimalSeparator.Chars(i)) Then GoTo MisMatch
                    Next
                    For i = 0 To .CurrencyGroupSeparator.Length - 1
                        If (.CurrencyGroupSeparator.Chars(i) <> .NumberGroupSeparator.Chars(i)) Then GoTo MisMatch
                    Next
                    Return inNumberFormat
                End If
            End With
MisMatch:
            outNumberFormat = DirectCast(inNumberFormat.Clone, Global.System.Globalization.NumberFormatInfo)
            With outNumberFormat
                .CurrencyDecimalSeparator = .NumberDecimalSeparator
                .CurrencyGroupSeparator = .NumberGroupSeparator
                .CurrencyDecimalDigits = .NumberDecimalDigits
            End With
            Return outNumberFormat
        End Function
        Public Shared Function ToSingle(Value As String) As Single
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CSng(i64Value)
                End If
                Dim result As Double = ParseDouble(Value)
                If (result < Global.System.Single.MinValue OrElse result > Global.System.Single.MaxValue) AndAlso
                   Not Global.System.Double.IsInfinity(result) Then
                    Throw New Global.System.OverflowException
                End If
                Return CSng(result)
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToSingle(Value As Object) As Single
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CSng(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CSng(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CSng(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CSng(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CSng(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CSng(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CSng(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CSng(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CSng(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CSng(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CSng(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CSng(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CSng(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToDouble(Value As String) As Double
            If Value Is Nothing Then
                Return 0
            End If
            Try
                Dim i64Value As Global.System.Int64
                If IsHexOrOctValue(Value, i64Value) Then
                    Return CDbl(i64Value)
                End If
                Return ParseDouble(Value)
            Catch e As Global.System.FormatException
                Throw New Global.System.InvalidCastException(e.Message, e)
            End Try
        End Function
        Public Shared Function ToDouble(Value As Object) As Double
            If Value Is Nothing Then
                Return 0
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CDbl(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CDbl(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CDbl(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CDbl(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CDbl(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CDbl(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CDbl(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CDbl(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CDbl(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CDbl(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CDbl(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CDbl(DirectCast(Value, Double))
            ElseIf TypeOf Value Is String Then
                Return CDbl(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Private Shared Function ParseDouble(value As String) As Double
            Dim normalizedNumberFormat As Global.System.Globalization.NumberFormatInfo
            Dim culture As Global.System.Globalization.CultureInfo = GetCultureInfo()
            Dim numberFormat As Global.System.Globalization.NumberFormatInfo = culture.NumberFormat
            normalizedNumberFormat = GetNormalizedNumberFormat(numberFormat)
            Const flags As Global.System.Globalization.NumberStyles =
                    Global.System.Globalization.NumberStyles.AllowDecimalPoint Or
                    Global.System.Globalization.NumberStyles.AllowExponent Or
                    Global.System.Globalization.NumberStyles.AllowLeadingSign Or
                    Global.System.Globalization.NumberStyles.AllowLeadingWhite Or
                    Global.System.Globalization.NumberStyles.AllowThousands Or
                    Global.System.Globalization.NumberStyles.AllowTrailingSign Or
                    Global.System.Globalization.NumberStyles.AllowParentheses Or
                    Global.System.Globalization.NumberStyles.AllowTrailingWhite Or
                    Global.System.Globalization.NumberStyles.AllowCurrencySymbol
            value = ToHalfwidthNumbers(value, culture)
            Try
                Return Global.System.Double.Parse(value, flags, normalizedNumberFormat)
            Catch formatEx As Global.System.FormatException When Not (numberFormat Is normalizedNumberFormat)
                Return Global.System.Double.Parse(value, flags, numberFormat)
            Catch ex As Global.System.Exception
                Throw ex
            End Try
        End Function

        Friend Shared Function TryParseDouble(ByVal Value As String, ByRef Result As Double) As Boolean
            Dim numberFormat As NumberFormatInfo
            Dim normalizedNumberFormat As NumberFormatInfo
            Dim culture As CultureInfo = GetCultureInfo()

            numberFormat = culture.NumberFormat
            normalizedNumberFormat = GetNormalizedNumberFormat(numberFormat)

            Const flags As NumberStyles =
                NumberStyles.AllowDecimalPoint Or
                NumberStyles.AllowExponent Or
                NumberStyles.AllowLeadingSign Or
                NumberStyles.AllowLeadingWhite Or
                NumberStyles.AllowThousands Or
                NumberStyles.AllowTrailingSign Or
                NumberStyles.AllowParentheses Or
                NumberStyles.AllowTrailingWhite Or
                NumberStyles.AllowCurrencySymbol

            Value = ToHalfwidthNumbers(Value, culture)

            ' The below code handles the 80% case efficiently and is inefficient only when the numeric and currency settings
            ' are different

            If numberFormat Is normalizedNumberFormat Then
                Return System.Double.TryParse(Value, flags, normalizedNumberFormat, Result)
            Else
                Try
                    ' Use numeric settings to parse
                    ' Note that we use Parse instead of TryParse in order to distinguish whether the conversion failed
                    ' due to FormatException or other exception like OverFlowException, etc.
                    Result = System.Double.Parse(Value, flags, normalizedNumberFormat)
                    Return True
                Catch FormatEx As FormatException
                    ' Use currency settings to parse
                    Try
                        Return System.Double.TryParse(Value, flags, numberFormat, Result)
                    Catch ex As ArgumentException
                        Return False
                    End Try
                Catch ex As StackOverflowException
                    Throw ex
                Catch ex As OutOfMemoryException
                    Throw ex
                Catch ex As System.Threading.ThreadAbortException
                    Throw ex
                Catch Ex As Exception
                    Return False
                End Try
            End If

        End Function

        Public Shared Function ToDate(Value As String) As Date
            Dim parsedDate As Global.System.DateTime
            Const parseStyle As Global.System.Globalization.DateTimeStyles =
                Global.System.Globalization.DateTimeStyles.AllowWhiteSpaces Or
                Global.System.Globalization.DateTimeStyles.NoCurrentDateDefault
            Dim culture As Global.System.Globalization.CultureInfo = GetCultureInfo()
            Dim result As Boolean = Global.System.DateTime.TryParse(ToHalfwidthNumbers(Value, culture), culture, parseStyle, parsedDate)
            If result Then
                Return parsedDate
            Else
                Throw New Global.System.InvalidCastException()
            End If
        End Function
        Public Shared Function ToDate(Value As Object) As Date
            If Value Is Nothing Then
                Return Nothing
            End If
            If TypeOf Value Is Global.System.DateTime Then
                Return CDate(DirectCast(Value, Global.System.DateTime))
            ElseIf TypeOf Value Is String Then
                Return CDate(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function

        Friend Shared Function TryParseDate(ByVal Value As String, ByRef Result As System.DateTime) As Boolean
            Const ParseStyle As DateTimeStyles =
                    DateTimeStyles.AllowWhiteSpaces Or
                    DateTimeStyles.NoCurrentDateDefault
            Dim Culture As CultureInfo = GetCultureInfo()
            Return System.DateTime.TryParse(ToHalfwidthNumbers(Value, Culture), Culture, ParseStyle, Result)
        End Function

        Public Shared Function ToChar(Value As String) As Char
            If (Value Is Nothing) OrElse (Value.Length = 0) Then
                Return Global.System.Convert.ToChar(0 And &HFFFFI)
            End If
            Return Value.Chars(0)
        End Function
        Public Shared Function ToChar(Value As Object) As Char
            If Value Is Nothing Then
                Return Global.System.Convert.ToChar(0 And &HFFFFI)
            End If
            If TypeOf Value Is Char Then
                Return CChar(DirectCast(Value, Char))
            ElseIf TypeOf Value Is String Then
                Return CChar(DirectCast(Value, String))
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Function ToCharArrayRankOne(Value As String) As Char()
            If Value Is Nothing Then
                Value = ""
            End If
            Return Value.ToCharArray()
        End Function
        Public Shared Function ToCharArrayRankOne(Value As Object) As Char()
            If Value Is Nothing Then
                Return "".ToCharArray()
            End If
            Dim arrayValue As Char() = TryCast(Value, Char())
            If arrayValue IsNot Nothing AndAlso arrayValue.Rank = 1 Then
                Return arrayValue
            ElseIf TypeOf Value Is String Then
                Return DirectCast(Value, String).ToCharArray()
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Shadows Function ToString(Value As Short) As String
            Return Value.ToString()
        End Function
        Public Shared Shadows Function ToString(Value As Integer) As String
            Return Value.ToString()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Shadows Function ToString(Value As UInteger) As String
            Return Value.ToString()
        End Function
        Public Shared Shadows Function ToString(Value As Long) As String
            Return Value.ToString()
        End Function
        <Global.System.CLSCompliant(False)>
        Public Shared Shadows Function ToString(Value As ULong) As String
            Return Value.ToString()
        End Function
        Public Shared Shadows Function ToString(Value As Single) As String
            Return Value.ToString()
        End Function
        Public Shared Shadows Function ToString(Value As Double) As String
            Return Value.ToString("G")
        End Function
        Public Shared Shadows Function ToString(Value As Date) As String
            Dim timeTicks As Long = Value.TimeOfDay.Ticks
            If (timeTicks = Value.Ticks) OrElse
                (Value.Year = 1899 AndAlso Value.Month = 12 AndAlso Value.Day = 30) Then
                Return Value.ToString("T")
            ElseIf timeTicks = 0 Then
                Return Value.ToString("d")
            Else
                Return Value.ToString("G")
            End If
        End Function
        Public Shared Shadows Function ToString(Value As Decimal) As String
            Return Value.ToString("G")
        End Function
        Public Shared Shadows Function ToString(Value As Object) As String
            If Value Is Nothing Then
                Return Nothing
            Else
                Dim stringValue As String = TryCast(Value, String)
                If stringValue IsNot Nothing Then
                    Return stringValue
                End If
            End If
            If TypeOf Value Is Global.System.Enum Then
                Value = GetEnumValue(Value)
            End If
            If TypeOf Value Is Boolean Then
                Return CStr(DirectCast(Value, Boolean))
            ElseIf TypeOf Value Is SByte Then
                Return CStr(DirectCast(Value, SByte))
            ElseIf TypeOf Value Is Byte Then
                Return CStr(DirectCast(Value, Byte))
            ElseIf TypeOf Value Is Global.System.Int16 Then
                Return CStr(DirectCast(Value, Global.System.Int16))
            ElseIf TypeOf Value Is Global.System.UInt16 Then
                Return CStr(DirectCast(Value, Global.System.UInt16))
            ElseIf TypeOf Value Is Global.System.Int32 Then
                Return CStr(DirectCast(Value, Global.System.Int32))
            ElseIf TypeOf Value Is Global.System.UInt32 Then
                Return CStr(DirectCast(Value, Global.System.UInt32))
            ElseIf TypeOf Value Is Global.System.Int64 Then
                Return CStr(DirectCast(Value, Global.System.Int64))
            ElseIf TypeOf Value Is Global.System.UInt64 Then
                Return CStr(DirectCast(Value, Global.System.UInt64))
            ElseIf TypeOf Value Is Decimal Then
                Return CStr(DirectCast(Value, Global.System.Decimal))
            ElseIf TypeOf Value Is Single Then
                Return CStr(DirectCast(Value, Single))
            ElseIf TypeOf Value Is Double Then
                Return CStr(DirectCast(Value, Double))
            ElseIf TypeOf Value Is Char Then
                Return CStr(DirectCast(Value, Char))
            ElseIf TypeOf Value Is Date Then
                Return CStr(DirectCast(Value, Date))
            Else
                Dim charArray As Char() = TryCast(Value, Char())
                If charArray IsNot Nothing Then
                    Return New String(charArray)
                End If
            End If
            Throw New Global.System.InvalidCastException()
        End Function
        Public Shared Shadows Function ToString(Value As Boolean) As String
            If Value Then
                Return Global.System.Boolean.TrueString
            Else
                Return Global.System.Boolean.FalseString
            End If
        End Function
        Public Shared Shadows Function ToString(Value As Byte) As String
            Return Value.ToString()
        End Function
        Public Shared Shadows Function ToString(Value As Char) As String
            Return Value.ToString()
        End Function
        Friend Shared Function GetCultureInfo() As Global.System.Globalization.CultureInfo
            Return Global.System.Globalization.CultureInfo.CurrentCulture
        End Function
        Friend Shared Function ToHalfwidthNumbers(s As String, culture As Global.System.Globalization.CultureInfo) As String
            Return s
        End Function
        Friend Shared Function IsHexOrOctValue(value As String, ByRef i64Value As Global.System.Int64) As Boolean
            Dim ch As Char
            Dim length As Integer
            Dim firstNonspace As Integer
            Dim tmpValue As String
            length = value.Length
            Do While (firstNonspace < length)
                ch = value.Chars(firstNonspace)
                If ch = "&"c AndAlso firstNonspace + 2 < length Then
                    GoTo GetSpecialValue
                End If
                If ch <> Strings.ChrW(32) AndAlso ch <> Strings.ChrW(&H3000) Then
                    Return False
                End If
                firstNonspace += 1
            Loop
            Return False
GetSpecialValue:
            ch = Global.System.Char.ToLowerInvariant(value.Chars(firstNonspace + 1))
            tmpValue = ToHalfwidthNumbers(value.Substring(firstNonspace + 2), GetCultureInfo())
            If ch = "h"c Then
                i64Value = Global.System.Convert.ToInt64(tmpValue, 16)
            ElseIf ch = "o"c Then
                i64Value = Global.System.Convert.ToInt64(tmpValue, 8)
            Else
                Throw New Global.System.FormatException
            End If
            Return True
        End Function
        <Global.System.CLSCompliant(False)>
        Friend Shared Function IsHexOrOctValue(value As String, ByRef ui64Value As Global.System.UInt64) As Boolean
            Dim ch As Char
            Dim length As Integer
            Dim firstNonspace As Integer
            Dim tmpValue As String
            length = value.Length
            Do While (firstNonspace < length)
                ch = value.Chars(firstNonspace)
                If ch = "&"c AndAlso firstNonspace + 2 < length Then
                    GoTo GetSpecialValue
                End If
                If ch <> Strings.ChrW(32) AndAlso ch <> Strings.ChrW(&H3000) Then
                    Return False
                End If
                firstNonspace += 1
            Loop
            Return False
GetSpecialValue:
            ch = Global.System.Char.ToLowerInvariant(value.Chars(firstNonspace + 1))
            tmpValue = ToHalfwidthNumbers(value.Substring(firstNonspace + 2), GetCultureInfo())
            If ch = "h"c Then
                ui64Value = Global.System.Convert.ToUInt64(tmpValue, 16)
            ElseIf ch = "o"c Then
                ui64Value = Global.System.Convert.ToUInt64(tmpValue, 8)
            Else
                Throw New Global.System.FormatException
            End If
            Return True
        End Function
        Public Shared Function ToGenericParameter(Of T)(Value As Object) As T
            If Value Is Nothing Then
                Return Nothing
            End If
            Dim reflectedType As Global.System.Type = GetType(T)
            If Global.System.Type.Equals(reflectedType, GetType(Global.System.Boolean)) Then
                Return DirectCast(CObj(CBool(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.SByte)) Then
                Return DirectCast(CObj(CSByte(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Byte)) Then
                Return DirectCast(CObj(CByte(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Int16)) Then
                Return DirectCast(CObj(CShort(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.UInt16)) Then
                Return DirectCast(CObj(CUShort(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Int32)) Then
                Return DirectCast(CObj(CInt(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.UInt32)) Then
                Return DirectCast(CObj(CUInt(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Int64)) Then
                Return DirectCast(CObj(CLng(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.UInt64)) Then
                Return DirectCast(CObj(CULng(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Decimal)) Then
                Return DirectCast(CObj(CDec(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Single)) Then
                Return DirectCast(CObj(CSng(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Double)) Then
                Return DirectCast(CObj(CDbl(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.DateTime)) Then
                Return DirectCast(CObj(CDate(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.Char)) Then
                Return DirectCast(CObj(CChar(Value)), T)
            ElseIf Global.System.Type.Equals(reflectedType, GetType(Global.System.String)) Then
                Return DirectCast(CObj(CStr(Value)), T)
            Else
                Return DirectCast(Value, T)
            End If
        End Function

        Private Shared Function CastSByteEnum(ByVal expression As SByte, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastByteEnum(ByVal expression As Byte, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastInt16Enum(ByVal expression As Int16, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastUInt16Enum(ByVal expression As UInt16, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastInt32Enum(ByVal expression As Int32, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastUInt32Enum(ByVal expression As UInt32, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastInt64Enum(ByVal expression As Int64, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Private Shared Function CastUInt64Enum(ByVal expression As UInt64, ByVal targetType As Type) As Object
            If IsEnum(targetType) Then Return System.Enum.ToObject(targetType, expression)
            Return expression
        End Function

        Friend Shared Function ForceValueCopy(ByVal expression As Object, ByVal targetType As Type) As Object
            'Is there any way to get this faster?  It's called every time we pass a valuetype to a byref parameter.

            If expression Is Nothing Then
                Return expression
            End If

            Dim tyCode As TypeCode = expression.GetType().GetTypeCode

            Debug.Assert(tyCode = GetTypeCode(targetType), "expected types to match")

            Select Case tyCode

                Case TypeCode.Boolean
                    Return Convert.ToBoolean(expression)
                Case TypeCode.SByte
                    Return CastSByteEnum(Convert.ToSByte(expression), targetType)
                Case TypeCode.Byte
                    Return CastByteEnum(Convert.ToByte(expression), targetType)
                Case TypeCode.Int16
                    Return CastInt16Enum(Convert.ToInt16(expression), targetType)
                Case TypeCode.UInt16
                    Return CastUInt16Enum(Convert.ToUInt16(expression), targetType)
                Case TypeCode.Int32
                    Return CastInt32Enum(Convert.ToInt32(expression), targetType)
                Case TypeCode.UInt32
                    Return CastUInt32Enum(Convert.ToUInt32(expression), targetType)
                Case TypeCode.Int64
                    Return CastInt64Enum(Convert.ToInt64(expression), targetType)
                Case TypeCode.UInt64
                    Return CastUInt64Enum(Convert.ToUInt64(expression), targetType)
                Case TypeCode.Decimal
                    Return Convert.ToDecimal(expression)
                Case TypeCode.Single
                    Return Convert.ToSingle(expression)
                Case TypeCode.Double
                    Return Convert.ToDouble(expression)
                Case TypeCode.DateTime
                    Return Convert.ToDateTime(expression)
                Case TypeCode.Char
                    Return Convert.ToChar(expression)

                Case TypeCode.Empty
                    Debug.Assert(False, "shouldn't reach here")

                Case TypeCode.Object,
                     TypeCode.String

                    'fall through
            End Select

            Return expression
        End Function

        Private Shared Function ChangeIntrinsicType(ByVal expression As Object, ByVal targetType As Type) As Object

            'This function will not handle user-defined conversion resolution, and so handles
            'only conversions between intrinsic types.
            Debug.Assert(IsIntrinsicType(expression.GetType) OrElse IsEnum(expression.GetType), "this function converts between intrinsic types only")

            Select Case GetTypeCode(targetType)

                Case TypeCode.Boolean : Return CBool(expression)
                Case TypeCode.SByte : Return CastSByteEnum(CSByte(expression), targetType)
                Case TypeCode.Byte : Return CastByteEnum(CByte(expression), targetType)
                Case TypeCode.Int16 : Return CastInt16Enum(CShort(expression), targetType)
                Case TypeCode.UInt16 : Return CastUInt16Enum(CUShort(expression), targetType)
                Case TypeCode.Int32 : Return CastInt32Enum(CInt(expression), targetType)
                Case TypeCode.UInt32 : Return CastUInt32Enum(CUInt(expression), targetType)
                Case TypeCode.Int64 : Return CastInt64Enum(CLng(expression), targetType)
                Case TypeCode.UInt64 : Return CastUInt64Enum(CULng(expression), targetType)
                Case TypeCode.Decimal : Return CDec(expression)
                Case TypeCode.Single : Return CSng(expression)
                Case TypeCode.Double : Return CDbl(expression)
                Case TypeCode.DateTime : Return CDate(expression)
                Case TypeCode.Char : Return CChar(expression)
                Case TypeCode.String : Return CStr(expression)

                Case TypeCode.Empty,
                     TypeCode.Object
                    'fall through to error
            End Select
            Debug.Assert(False, "Expected intrinsic type only, not: " & targetType.Name)
            Throw New Exception  'would be nice to have an internal runtime exception or something like that

        End Function


        Public Shared Function ChangeType(ByVal Expression As Object, ByVal TargetType As System.Type) As Object
            Return ChangeType(Expression, TargetType, False)
        End Function


        Friend Shared Function ChangeType(ByVal expression As Object, ByVal targetType As System.Type, ByVal dynamic As Boolean) As Object
            If targetType Is Nothing Then
                Throw New ArgumentException(GetResourceString(SR.Argument_InvalidNullValue1, "TargetType"))
            End If

            If expression Is Nothing Then
                If IsValueType(targetType) Then
                    'method.invoke will do this for us, so when casting arguments to param types during 
                    'latebinding, the createinstance call isn't needed, but ChangeType used in a generalized
                    'manner should return a default instance.
                    Return Activator.CreateInstance(targetType)
                Else
                    Return Nothing
                End If
            End If

            Dim sourceType As Type = expression.GetType
            Debug.Assert(Not sourceType.IsByRef, "never expected to see byref source type")

            'Dig through ByRef types which might come in.
            If targetType.IsByRef Then targetType = targetType.GetElementType

            If targetType Is sourceType OrElse IsRootObjectType(targetType) Then
                Return expression
            End If

            Dim targetTypeCode As TypeCode = GetTypeCode(targetType)

            'All conversions between intrinsic types are natively built-in
            'and require no user-defined conversion resolution.
            If IsIntrinsicType(targetTypeCode) Then
                Dim sourceTypeCode As TypeCode = GetTypeCode(sourceType)
                If IsIntrinsicType(sourceTypeCode) Then
                    Return ChangeIntrinsicType(expression, targetType)
                End If
            End If

            If targetType.IsInstanceOfType(expression) Then
                Return expression
            End If

            If IsCharArrayRankOne(targetType) AndAlso IsStringType(sourceType) Then
                Return CType(DirectCast(expression, String), Char())
            End If
            If IsStringType(targetType) AndAlso IsCharArrayRankOne(sourceType) Then
                Return CStr(DirectCast(expression, Char()))
            End If

            Debug.Assert(ClassifyPredefinedConversion(targetType, sourceType) = ConversionClass.None OrElse
                         ClassifyPredefinedConversion(targetType, sourceType) = ConversionClass.Narrowing,
                         "expected all predefined conversions handled by this point")

            If dynamic Then
                Dim idmop As System.Dynamic.IDynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(expression)
                If idmop IsNot Nothing Then
                    Return IDOBinder.UserDefinedConversion(idmop, targetType)
                End If
            End If
            Return ObjectUserDefinedConversion(expression, targetType)
        End Function

        <ObsoleteAttribute("do not use this method", True)>
        <DebuggerHiddenAttribute()> <DebuggerStepThroughAttribute()>
        Public Shared Function FallbackUserDefinedConversion(
                ByVal expression As Object, ByVal targetType As Type) As Object

            Return ObjectUserDefinedConversion(expression, targetType)
        End Function

        <DebuggerHiddenAttribute()> <DebuggerStepThroughAttribute()>
        Private Shared Function ObjectUserDefinedConversion(
                ByVal expression As Object, ByVal targetType As Type) As Object

            Dim sourceType As Type = expression.GetType
            If ClassifyPredefinedConversion(targetType, sourceType) = ConversionClass.None AndAlso
               (IsClassOrValueType(sourceType) OrElse IsClassOrValueType(targetType)) AndAlso
               Not (IsIntrinsicType(sourceType) AndAlso IsIntrinsicType(targetType)) Then

                'Conversions of the form S-->T use only one user-defined conversion at a time, i.e.,
                'user-defined conversions are not chained together.  It may be necessary to convert to and
                'from intermediate types using predefined conversions to match the signature of the
                'user-defined conversion exactly, so the conversion "path" is comprised of at most three
                'parts:
                '
                '    1) [ predefined conversion  S-->Sx ]
                '    2) User-defined conversion Sx-->Tx
                '    3) [ predefined conversion Tx-->T  ]
                '
                '    Where Sx is the intermediate source type
                '      and Tx is the intermediate target type
                '
                '    Steps 1 and 3 are optional given S == Sx or Tx == T.
                '
                'Given the source operand and target type, resolve the conversion operator and invoke it.
                'When invoking the conversion operator, the conversion from S-->Sx is done when matching
                'the arguments.  After invocation, we must handle the conversion from the result of the
                'invocation to the target type, i.e., Tx-->T.

                'Resolve the operator.
                Dim operatorMethod As Method = Nothing
                Dim result As ConversionClass =
                    ClassifyUserDefinedConversion(targetType, sourceType, operatorMethod)

                If operatorMethod IsNot Nothing Then
                    Debug.Assert(result = ConversionClass.Widening OrElse result = ConversionClass.Narrowing,
                                 "operator method not expected for invalid conversion")

                    'Invoke the operator. This handles the conversion S-->Sx.
                    Dim baseReference As Container = New Container(operatorMethod.DeclaringType)
                    Dim invocationResult As Object =
                        baseReference.InvokeMethod(
                            operatorMethod,
                            New Object() {expression},
                            Nothing,
                            BindingFlagsInvokeMethod)

                    'Now convert the result of the invocation to the target type, Tx-->T. 

#If DEBUG Then
                    If invocationResult IsNot Nothing Then


                        'Disabling the assert below when we're converting to Nullable(Of T)
                        'since the Runtime hasn't been updated yet to handle Nullable.  In this case the assert 
                        'is harmless, but ClassifyPredefinedConversion hasn't been updated to consider Nullable conversions,
                        'and updating this across the entire runtime would be significant feature work.

                        If Not (targetType.IsGenericType AndAlso
                                Not targetType.IsGenericTypeDefinition AndAlso
                                targetType.GetGenericTypeDefinition().Equals(GetType(Nullable(Of ))) AndAlso
                                targetType.GetGenericArguments().Length > 0 AndAlso
                                invocationResult.GetType().Equals(targetType.GetGenericArguments()(0))) Then


                            Dim PostConversion As ConversionClass = ClassifyPredefinedConversion(targetType, invocationResult.GetType)

                            Debug.Assert(
                                PostConversion = ConversionClass.Narrowing OrElse
                                PostConversion = ConversionClass.Identity OrElse
                                PostConversion = ConversionClass.Widening,
                                "User defined conversion returned unexpected result")
                        End If
                    End If
#End If
                    Return ChangeType(invocationResult, targetType)

                ElseIf result = ConversionClass.Ambiguous Then
                    Throw New InvalidCastException(
                        GetResourceString(
                            SR.AmbiguousCast2,
                            VBFriendlyName(sourceType),
                            VBFriendlyName(targetType)))
                End If

            End If

            Throw New InvalidCastException(
                GetResourceString(
                    SR.InvalidCast_FromTo,
                    VBFriendlyName(sourceType),
                    VBFriendlyName(targetType)))

        End Function

        ' Simplied version of ObjectUserDefinedConversion, above
        ' Determines if conversion is possible
        Friend Shared Function CanUserDefinedConvert(ByVal expression As Object, ByVal targetType As Type) As Boolean

            Dim sourceType As Type = expression.GetType
            If ClassifyPredefinedConversion(targetType, sourceType) = ConversionClass.None AndAlso
               (IsClassOrValueType(sourceType) OrElse IsClassOrValueType(targetType)) AndAlso
               Not (IsIntrinsicType(sourceType) AndAlso IsIntrinsicType(targetType)) Then

                'Resolve the operator.
                Dim operatorMethod As Method = Nothing
                Dim result As ConversionClass =
                    ClassifyUserDefinedConversion(targetType, sourceType, operatorMethod)

                Return operatorMethod IsNot Nothing
            End If

            Return False
        End Function

    End Class
End Namespace
